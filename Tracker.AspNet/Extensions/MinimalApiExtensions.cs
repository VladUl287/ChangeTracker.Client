using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tracker.AspNet.Filters;

namespace Tracker.AspNet.Extensions;

public static class MinimalApiExtensions
{
    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint)
    {
        return endpoint.AddEndpointFilter<IEndpointConventionBuilder, ETagEndpointFilter>();
    }

    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint, string[] tables)
    {
        return endpoint.AddEndpointFilterFactory((serviceProvider, next) =>
        {
            var logger = serviceProvider.ApplicationServices.GetRequiredService<ILogger<ETagEndpointFilter>>();
            var filter = new ETagEndpointFilter(tables);
            return (context) => filter.InvokeAsync(context, next);
        });
    }

    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint, Type[] entities)
    {
        return endpoint.AddEndpointFilterFactory((context, next) =>
        {
            return async (invocCtx) =>
            {
                return await next(invocCtx);
            };
        });
    }

    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint, string[] tables, Type[] entities)
    {
        return endpoint.AddEndpointFilterFactory((context, next) =>
        {
            return async (invocCtx) =>
            {
                return await next(invocCtx);
            };
        });
    }
}
