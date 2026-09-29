# PostgreSQL usage

[![NuGet Status](https://img.shields.io/nuget/v/ChangeTracker.Npgsql.svg?label=ChangeTracker.Npgsql)](https://www.nuget.org/packages/ChangeTracker.Npgsql/)

Official documentation: [PostgreSQL Npgsql](https://www.npgsql.org).

## Overview

This implementation provides two complementary mechanisms for tracking database changes:

Global transaction tracking uses PostgreSQL built-in [track_commit_timestamp](https://www.postgresql.org/docs/17/runtime-config-replication.html#GUC-TRACK-COMMIT-TIMESTAMP) setting to track transactions across the entire database.

Per-table tracking uses the custom [table_change_tracker](https://github.com/VladUl287/table_change_tracker) extension to monitor modifications to specific tables.

## Prerequisites

PostgreSQL requires [track_commit_timestamp](https://www.postgresql.org/docs/17/runtime-config-replication.html#GUC-TRACK-COMMIT-TIMESTAMP) to be enabled for cases when no tables are specified in options.

This can be done using:

```sql
ALTER SYSTEM SET track_commit_timestamp = 'on';
```

Then restart the PostgreSQL service.

For cases when you need to track **specific tables**, use a custom extension developed specifically for that case: [table_change_tracker](https://github.com/VladUl287/table_change_tracker).

## Database Usage

Global tracking:

```sql
SELECT pg_last_committed_xact();
```

Specific Table/Tables Tracking:

```sql
SELECT get_timestamp(@table_name);
SELECT get_last_timestamp(@tables_names);
```

Enable table tracking:
```sql
SELECT enable_table_tracking(@table_name);
```

Disable table tracking:
```sql
SELECT disable_table_tracking(@table_name);
```

Check status:
```sql
SELECT is_table_tracking_enabled(@table_name);
```

## Backend Usage

### Registering a provider

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

### Using database logic via ```ISourceProvider```

```cs
public class StartupService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var sourceProvider = scope.ServiceProvider.GetKeyedServices<ISourceProvider>(KeyedService.AnyKey);
        await sourceProvider
            .First()
            .EnableTracking("roles", stoppingToken);
    }
}
```