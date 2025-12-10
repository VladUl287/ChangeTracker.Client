using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Immutable;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;
using Tracker.Core.Extensions;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class GlobalOptionsBuilder(IServiceScopeFactory scopeFactory) : IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>
{
    public ImmutableGlobalOptions Build(GlobalOptions options)
    {
        var cacheControl = options.CacheControl ?? options.CacheControlBuilder?.Build();

        return new ImmutableGlobalOptions
        {
            Source = options.Source,
            Suffix = options.Suffix,
            Filter = options.Filter,
            CacheControl = cacheControl,
            Tables = [.. options.Tables],
            SourceOperations = options.SourceOperations,
            SourceOperationsFactory = options.SourceOperationsFactory,
        };
    }

    public ImmutableGlobalOptions Build<TContext>(GlobalOptions options) where TContext : DbContext
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
        var sourceIdGenerator = scope.ServiceProvider.GetRequiredService<ISourceIdGenerator>();

        var tablesNames = dbContext.GetTablesNames(options.Entities);
        var tables = new HashSet<string>([.. options.Tables, .. tablesNames]).ToImmutableArray();

        var source = options.Source;
        if (options is { Source: null, SourceOperations: null, SourceOperationsFactory: null })
            source = sourceIdGenerator.GenerateId<TContext>();

        var cacheControl = options.CacheControl ?? options.CacheControlBuilder?.Build();

        return new ImmutableGlobalOptions
        {
            Tables = tables,
            Source = source,
            Filter = options.Filter,
            Suffix = options.Suffix,
            CacheControl = cacheControl,
            SourceOperations = options.SourceOperations,
            SourceOperationsFactory = options.SourceOperationsFactory,
        };
    }
}
