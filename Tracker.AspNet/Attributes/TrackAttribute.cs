using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using Tracker.AspNet.Logging;
using Tracker.AspNet.Models;

namespace Tracker.AspNet.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class TrackAttribute(
    string[]? tables = null,
    string? sourceId = null,
    string? cacheControl = null) : TrackAttributeBase
{
    private ImmutableGlobalOptions? _actionOptions;
    private readonly Lock _lock = new();

    protected override ImmutableGlobalOptions GetOptions(ActionExecutingContext ctx)
    {
        if (_actionOptions is not null)
            return _actionOptions;

        lock (_lock)
        {
            if (_actionOptions is not null)
                return _actionOptions;

            var serviceProvider = ctx.HttpContext.RequestServices;
            var options = serviceProvider.GetRequiredService<ImmutableGlobalOptions>();
            var logger = serviceProvider.GetRequiredService<ILogger<TrackAttribute>>();

            _actionOptions = options with
            {
                CacheControl = cacheControl ?? options.CacheControl,
                Source = sourceId ?? options.Source,
                Tables = tables?.ToImmutableArray() ?? []
            };
            logger.LogOptionsBuilded(ctx.ActionDescriptor.DisplayName ?? ctx.ActionDescriptor.Id);
            return _actionOptions;
        }
    }
}