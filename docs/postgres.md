# PostgreSQL usage

Official documenation [PostgreSQL Npgsql](https://www.npgsql.org).

## Implementation

Postgres required [track_commit_timestamp](https://www.postgresql.org/docs/17/runtime-config-replication.html#GUC-TRACK-COMMIT-TIMESTAMP)
to be enabled for cases when no tables specified in options.
This can be done using `ALTER SYSTEM SET track_commit_timestamp to "on"` and then restarting the Postgres service

For cases when need track specific tables must be used specific custom extension which developed specifically
for that case <https://github.com/VladUl287/table_change_tracker>

## Timestamp calculation

In case global tracking:

```cs
select pg_last_committed_xact();
```

In case specific table tracking used functions from extension:

```sql
SELECT get_last_timestamp(@table_name);
SELECT get_last_timestamps(@tables_names);
```

When return mutliple timestamps ticks for multple tables it will be hashed with default hasher

```cs
public sealed class DefaultTrackerHasher : ITrackerHasher
{
    public ulong Hash(ReadOnlySpan<long> versions)
    {
        if (BitConverter.IsLittleEndian)
            return XxHash3.HashToUInt64(MemoryMarshal.AsBytes(versions));

        return HashBigEndian(versions);
    }
}
```

## Usage

### Add to WebApplicationBuilder

```cs
var builder = WebApplication.CreateBuilder();
{
    builder.Services
        .AddTracker()
        .AddNpgsqlProvider<DatabaseContext>();
    
    builder.Services
        .AddTracker()
        .AddNpgsqlProvider<DatabaseContext>("my-pg-provider");

    builder.Services
        .AddTracker()
        .AddNpgsqlProvider(
            "my-pg-provider", 
            "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=secret"
        );
}
```

[snippet source](../Tracker.Npgsql/Extensions/ServiceCollectionExtensions.cs)

### Add to a Route Group

To add to a specific [Route Group](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/route-handlers#route-groups):

```cs
app.MapGroup("/group")
    .WithTracking()
    .MapGet("/", () => { });
```

### Source Providers

Any provider implements:

```cs
public interface ISourceProvider : IDisposable
{
    string Id { get; }

    ValueTask<long> GetLastVersion(string key, CancellationToken token = default);

    ValueTask GetLastVersions(ImmutableArray<string> keys, long[] versions, CancellationToken token = default);

    ValueTask<long> GetLastVersion(CancellationToken token = default);

    ValueTask<bool> EnableTracking(string key, CancellationToken token = default);

    ValueTask<bool> DisableTracking(string key, CancellationToken token = default);

    ValueTask<bool> IsTracking(string key, CancellationToken token = default);

    ValueTask<bool> SetLastVersion(string key, long value, CancellationToken token = default);
}
```

[snippet source](../Tracker.Core/Services/Contracts/ISourceProvider.cs)

Source Provider Id used for cases when multiple providers registered. In cases with DbContext full name will be used as Source Provider Id

```cs
typeof(DatabaseContext).FullName
```

[default npgsql provider implementation](../Tracker.Npgsql/Services/NpgsqlOperations.cs)

For resolve provider called default implementation of [IProviderResolver](../Tracker.AspNet/Services/DefaultProviderResolver.cs)
If resolves by provider id from keyed services in case providerId presented in options.
If ProviderId not presented in options it will check if SourceProvider property instance presented.
If SourceProvider not presented it will try to check if factory presented and call it if it is.
If none of that presented it will resolve first ISourceProvider from DI container.

Simplified implementation snippet

```cs
public sealed class DefaultProviderResolver : IProviderResolver
{
    public ISourceProvider ResolveProvider(HttpContext ctx, ImmutableGlobalOptions options, out bool shouldDispose)
    {
        try
        {
            shouldDispose = false;

            if (options.ProviderId is not null)
            {
                return ctx.RequestServices.GetRequiredKeyedService<ISourceProvider>(options.ProviderId);
            }

            if (options.SourceProvider is not null)
            {
                return options.SourceProvider;
            }

            if (options.SourceProviderFactory is not null)
            {
                shouldDispose = true;
                return options.SourceProviderFactory(ctx);
            }

            return GetDefaultProvider(ctx);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to resolve source provider. TraceId: {ctx.TraceIdentifier}", ex);
        }
    }
}
```
