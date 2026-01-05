using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;
using Tracker.AspNet.Attributes;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;
using Tracker.FastEndpoints.Services.Contracts;

namespace Tracker.FastEndpoints.Services;

public sealed class DefaultTrackerProcessor : ITrackerProcessor
{
    private static readonly ConcurrentDictionary<string, ImmutableGlobalOptions> _actionsOptions = new();

    public async Task ProcessAsync(HttpContext context, CancellationToken token)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null or { DisplayName: null })
            return;

        var options = GetOptions(context, endpoint.DisplayName);

        var reqFilter = context.Resolve<IRequestFilter>();
        if (!reqFilter.ValidRequest(context, options))
            return;

        var reqHandler = context.Resolve<IRequestHandler>();
        if (await reqHandler.HandleRequest(context, options, token))
            await context.Response.SendResultAsync(Results.StatusCode(StatusCodes.Status304NotModified));
    }

    private static ImmutableGlobalOptions GetOptions(HttpContext context, string endpointKey)
    {
        return _actionsOptions.GetOrAdd(endpointKey, (key, ctx) =>
        {
            var attribute = GetAttribute(ctx, key);
            if (attribute is not null)
                return attribute.GetOptions(ctx);

            var scopeFactory = ctx.Resolve<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            return scope.Resolve<ImmutableGlobalOptions>();
        }, context);
    }

    private static TrackAttributeBase? GetAttribute(
        HttpContext context,
        string endpointKey)
    {
        var endpoint = context.GetEndpoint() ??
            throw new InvalidOperationException($"Endpoint not found for key: {endpointKey}");

        var definitions = endpoint.Metadata.GetOrderedMetadata<EndpointDefinition>();
        if (definitions is null || definitions.Count == 0)
            throw new InvalidOperationException($"{nameof(EndpointDefinition)} not found for endpoint: {endpointKey}");

        return definitions[0]
            .EndpointType
            .GetCustomAttribute<TrackAttributeBase>(true);
    }
}
