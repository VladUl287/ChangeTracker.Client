using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Extensions;

public static class SerivceCollectionExtensions
{
    public static IServiceCollection AddTracker<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        return services.AddTracker<TContext>(new GlobalOptions());
    }

    public static IServiceCollection AddTracker<TContext>(this IServiceCollection services, GlobalOptions options)
         where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        services.AddSingleton<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>, GlobalOptionsBuilder>();

        services.AddSingleton((provider) =>
        {
            var optionsBuilder = provider.GetRequiredService<IOptionsBuilder<GlobalOptions, ImmutableGlobalOptions>>();
            return optionsBuilder.Build<TContext>(options);
        });

        services.AddSingleton<IETagGenerator, ETagGenerator>();
        services.AddSingleton<IETagService, ETagService>();

        services.AddSingleton<ISourceOperationsResolver, SourceOperationsResolver>();
        services.AddSingleton<ISourceOperations>((provider) =>
        {
            using var scope = provider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();
            var connectionString = dbContext.Database.GetConnectionString() ?? 
                throw new NullReferenceException("Connaction string is null");
            return new NpgsqlOperations(connectionString);
        });

        services.AddSingleton<IRequestFilter, DefaultRequestFilter>();

        return services;
    }

    public static IServiceCollection AddTracker<TContext>(this IServiceCollection services, Action<GlobalOptions> configure)
         where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var options = new GlobalOptions();
        configure(options);
        return services.AddTracker<TContext>(options);
    }
}
