using Microsoft.AspNetCore.Http;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class TrackerEndpointFilter(IRequestHandler service, TrackOptionsSnapshot opts) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext filterCtx, EndpointFilterDelegate next)
    {
        var ctx = filterCtx.HttpContext;

        if (opts.Filter(ctx) && await service.HandleRequest(ctx, opts))
            return Results.StatusCode(StatusCodes.Status304NotModified);

        return await next(filterCtx);
    }
}
