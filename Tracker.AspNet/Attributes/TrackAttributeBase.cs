using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Attributes;

public abstract class TrackAttributeBase : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext execCtx, ActionExecutionDelegate next)
    {
        var ctx = execCtx.HttpContext;

        var options = GetOptions(ctx);

        if (options.Filter(ctx) && await NotModified(ctx, options))
            return;

        await next();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ValueTask<bool> NotModified(HttpContext httpCtx, TrackOptionsSnapshot options) =>
        httpCtx.RequestServices
            .GetRequiredService<IRequestHandler>()
            .HandleRequest(httpCtx, options);

    public abstract TrackOptionsSnapshot GetOptions(HttpContext execContext);
}
