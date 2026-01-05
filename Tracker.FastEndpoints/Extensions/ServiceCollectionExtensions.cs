using Microsoft.Extensions.DependencyInjection;
using Tracker.FastEndpoints.Services;
using Tracker.FastEndpoints.Services.Contracts;

namespace Tracker.FastEndpoints.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrackerFastEndpoints(this IServiceCollection services)
    {
        services.AddSingleton<ITrackerProcessor, DefaultTrackerProcessor>();

        return services;
    }
}
