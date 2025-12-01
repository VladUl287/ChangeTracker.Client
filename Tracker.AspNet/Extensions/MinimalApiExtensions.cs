using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Tracker.AspNet.Extensions;

public static class MinimalApiExtensions
{
    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint)
    {
        return endpoint.AddEndpointFilterFactory((context, next) =>
        {
            return async (invocCtx) =>
            {
                return await next(invocCtx);
            };
        });
    }

    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint, params string[] tables)
    {
        return endpoint.AddEndpointFilterFactory((context, next) =>
        {
            return async (invocCtx) =>
            {
                return await next(invocCtx);
            };
        });
    }

    public static IEndpointConventionBuilder WithTracking(this IEndpointConventionBuilder endpoint, params Type[] entities)
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
