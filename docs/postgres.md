# PostgreSQL usage

[![NuGet Status](https://img.shields.io/nuget/v/ChangeTracker.Npgsql.svg?label=ChangeTracker.Npgsql)](https://www.nuget.org/packages/ChangeTracker.Npgsql/)

Official documentation: [PostgreSQL Npgsql](https://www.npgsql.org).

This implementation uses PostgreSQL's built-in [track_commit_timestamp](https://www.postgresql.org/docs/17/runtime-config-replication.html#GUC-TRACK-COMMIT-TIMESTAMP) setting for global transaction tracking across the database, along with the custom [table_change_tracker](https://github.com/VladUl287/table_change_tracker) extension for monitoring modifications to specific tables.

## Implementation

PostgreSQL requires [track_commit_timestamp](https://www.postgresql.org/docs/17/runtime-config-replication.html#GUC-TRACK-COMMIT-TIMESTAMP) to be enabled for cases when no tables are specified in options.

This can be done using:

```sql
ALTER SYSTEM SET track_commit_timestamp = 'on';
```

Then restart the PostgreSQL service.

For cases when you need to track **specific tables**, use a custom extension developed specifically for that case: [table_change_tracker](https://github.com/VladUl287/table_change_tracker).

## Timestamp calculation

Global tracking:

```sql
SELECT pg_last_committed_xact();
```

Specific Table Tracking:

```sql
SELECT get_timestamp(@table_name);
SELECT get_last_timestamp(@tables_names);
```

## Usage

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
