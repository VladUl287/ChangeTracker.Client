using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services;
using Tracker.AspNet.Services.Contracts;
using Tracker.Core.Services;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTracker(this IServiceCollection services) =>
        services.AddTracker(new TrackOptions());

    public static IServiceCollection AddTracker(this IServiceCollection services, TrackOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        services.AddSingleton((provider) =>
        {
            var optionsBuilder = provider.GetRequiredService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
            return optionsBuilder.Build(options);
        });

        return services.AddTrackerBase();
    }

    public static IServiceCollection AddTracker(this IServiceCollection services, Action<TrackOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var options = new TrackOptions();
        configure(options);
        return services.AddTracker(options);
    }

    public static IServiceCollection AddTracker<TContext>(this IServiceCollection services) where TContext : DbContext =>
        services.AddTracker<TContext>(new TrackOptions());

    public static IServiceCollection AddTracker<TContext>(this IServiceCollection services, TrackOptions options)
         where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        services.AddSingleton((provider) =>
        {
            var optionsBuilder = provider.GetRequiredService<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>>();
            return optionsBuilder.Build<TContext>(options);
        });

        return services.AddTrackerBase();
    }

    public static IServiceCollection AddTracker<TContext>(this IServiceCollection services, Action<TrackOptions> configure)
         where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(configure, nameof(configure));

        var options = new TrackOptions();
        configure(options);
        return services.AddTracker<TContext>(options);
    }

    private static IServiceCollection AddTrackerBase(this IServiceCollection services)
    {
        services.AddSingleton<IOptionsBuilder<TrackOptions, TrackOptionsSnapshot>, DefaultOptionsBuilder>();

        services.AddSingleton<IAssemblyTimestampProvider>(new AssemblyTimestampProvider(Assembly.GetExecutingAssembly()));
        services.AddSingleton<IETagProvider, DefaultETagProvider>();

        services.AddSingleton<IRequestHandler, DefaultRequestHandler>();

        services.AddSingleton<IRequestFilter, DefaultRequestFilter>();

        services.AddSingleton<IProviderResolver, DefaultProviderResolver>();

        services.AddSingleton<ITableNameResolver, DefaultTableNameResolver>();

        return services;
    }
}
