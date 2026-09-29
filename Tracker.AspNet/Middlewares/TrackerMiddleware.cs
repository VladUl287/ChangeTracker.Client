using Microsoft.AspNetCore.Http;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Middlewares;

public sealed class TrackerMiddleware(RequestDelegate next, IRequestHandler service, TrackOptionsSnapshot opts)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        if (opts.Filter(ctx) && await service.HandleRequest(ctx, opts))
            return;

        await next(ctx);
    }
}