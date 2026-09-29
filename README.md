# ChangeTracker

[![NuGet Status](https://img.shields.io/nuget/v/ChangeTracker.Core.svg?label=ChangeTracker.Core)](https://www.nuget.org/packages/ChangeTracker.Core/)
[![NuGet Status](https://img.shields.io/nuget/v/ChangeTracker.AspNet.svg?label=ChangeTracker.AspNet)](https://www.nuget.org/packages/ChangeTracker.AspNet/)
[![NuGet Status](https://img.shields.io/nuget/v/ChangeTracker.Npgsql.svg?label=ChangeTracker.Npgsql)](https://www.nuget.org/packages/ChangeTracker.Npgsql/)

Change Tracker is a library for efficient HTTP caching using database change tracking.
It implements [**304 Not Modified**](https://www.keycdn.com/support/304-not-modified) responses
by generating ETags based on database timestamps, reducing server load while ensuring
clients always receive current data.

## Overview

ChangeTracker monitors database changes and generates ETags that combine:

* Assembly write time (when your application was built)
* Database timestamp (last data modification time)
* Custom suffix (optional runtime context)

When a client requests data with a cached ETag, the server compares it with the current state.

* **If unchanged:** it returns **304 Not Modified** and the client uses its cached copy.
* **If changed:** fresh data is returned with a new ETag.

ETags follow this format:

```cs
{AssemblyWriteTime}-{DbTimeStamp}-{Suffix}
```

## Ideal Use Case

* Read-heavy applications where data changes less frequently than it's read
* APIs serving semi-static data that changes periodically
* Applications needing reduced server load without compromising data freshness

## Quick start

#### 1. Enable table tracking

* [PostgreSQL](/docs/postgres.md) docs

#### 2. Configure services

```cs
builder.Services
    .AddTracker()
    .AddNpgsqlProvider<DatabaseContext>();
```

#### 3. Configure endpoint

**Controller action:**

```cs
[HttpGet]
[Track(["roles"])]
public ActionResult<IEnumerable<Role>> GetAll() => dbContext.Roles.ToList();
```

**Minimal api:**

```cs
app.MapGet("/roles/getall", (DatabaseContext dbContext) =>
{
    return dbContext.Roles.ToList();
})
.WithTracking(options =>
{
    options.Tables = ["roles"];
});
```

**Middleware:**

```cs
app.UseTracker(options =>
{
    options.Tables = ["roles"];
    options.Filter = (httpContext) => httpContext.Request.Path.Value.Contains("/api/roles/getall");
});
```

## Verifying behavior

### Testing Cache Hits

* Open your application in a browser
* Open Developer Tools (F12)
* Navigate to the Network tab
* Refresh the page

**Cached responses** will show:

* Status: 304 Not Modified
* Request Header: if-none-match (with ETag value)
* Response Header: etag (current ETag)

### Testing Cache Misses

To test the full request pipeline:

* Open Developer Tools -> Network tab
* Check "Disable cache" in the toolbar
* Refresh the page

This prevents the browser from sending if-none-match, forcing a cache miss and full server execution.
