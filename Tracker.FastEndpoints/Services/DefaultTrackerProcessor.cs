using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;
using Tracker.FastEndpoints.Attributes;
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

            var scopeFactory = ctx.Resolve<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();
            var options = scope.Resolve<ImmutableGlobalOptions>();

            return options with
            {
                Tables = ResolveTables(attribute.Tables, options),
                ProviderId = attribute.ProviderId ?? options.ProviderId,
                CacheControl = attribute.CacheControl ?? options.CacheControl,
            };
        }, context);
    }

    private static TrackerOptionsAttribute GetAttribute(
        HttpContext context,
        string endpointKey)
    {
        var endpoint = context.GetEndpoint() ??
            throw new InvalidOperationException($"Endpoint not found for key: {endpointKey}");

        var definitions = endpoint.Metadata.GetOrderedMetadata<EndpointDefinition>();
        if (definitions is null || definitions.Count == 0)
            throw new InvalidOperationException($"{nameof(EndpointDefinition)} not found for endpoint: {endpointKey}");

        return definitions[0].EndpointType.GetCustomAttribute<TrackerOptionsAttribute>() ??
            throw new InvalidOperationException($"{nameof(TrackerOptionsAttribute)} not found on endpoint: {endpointKey}");
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ImmutableArray<string> ResolveTables(IReadOnlyList<string>? tables, ImmutableGlobalOptions options)
    {
        if (tables is null || tables.Count == 0)
            return options.Tables;

        return new HashSet<string>([.. tables, .. options.Tables])
            .ToImmutableArray();
    }
}
