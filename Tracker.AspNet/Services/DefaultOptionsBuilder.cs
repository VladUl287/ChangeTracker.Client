using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;
using Tracker.AspNet.Utils;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class DefaultOptionsBuilder(IServiceScopeFactory scopeFactory, ITableNameResolver tableNameResolver) :
    IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>
{
    private static readonly string _defaultCacheControl = new CacheControlBuilder().WithNoCache().Combine();

    public TrackOptionsSnapshot Build(TrackOptions options)
    {
        return new TrackOptionsSnapshot
        {
            Suffix = options.Suffix,
            Filter = options.Filter,
            ProviderId = options.ProviderId,
            SourceProvider = options.SourceProvider,
            CacheControl = ResolveCacheControl(options),
            SourceProviderFactory = options.SourceProviderFactory,
            Tables = options.Tables is not null ? [.. options.Tables] : [],
            InvalidRequestDirectives = options.InvalidRequestDirectives is not null ? [.. options.InvalidRequestDirectives] : [],
            InvalidResponseDirectives = options.InvalidResponseDirectives is not null ? [.. options.InvalidResponseDirectives] : [],
        };
    }

    public TrackOptionsSnapshot Build<TContext>(TrackOptions options) where TContext : DbContext
    {
        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var tables = new HashSet<string>([
            .. options.Tables ?? [],
            .. tableNameResolver.GetTablesNames(context, options.Entities ?? [])
        ]);

        return new TrackOptionsSnapshot
        {
            Tables = [.. tables],
            Suffix = options.Suffix,
            Filter = options.Filter,
            ProviderId = options.ProviderId,
            SourceProvider = options.SourceProvider,
            CacheControl = ResolveCacheControl(options),
            SourceProviderFactory = options.SourceProviderFactory,
            InvalidRequestDirectives = options.InvalidRequestDirectives is not null ? [.. options.InvalidRequestDirectives] : [],
            InvalidResponseDirectives = options.InvalidResponseDirectives is not null ? [.. options.InvalidResponseDirectives] : [],
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ResolveCacheControl(TrackOptions options) =>
        options.CacheControl ?? options.CacheControlBuilder?.Combine() ?? _defaultCacheControl;
}
