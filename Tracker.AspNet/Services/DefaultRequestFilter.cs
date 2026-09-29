using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tracker.AspNet.Logging;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class DefaultRequestFilter(ILogger<DefaultRequestFilter> logger) : IRequestFilter
{
    public bool ValidRequest(HttpContext ctx, TrackOptionsSnapshot opts)
    {
        var traceId = new RequestId(ctx);

        logger.LogFilterStarted(traceId, ctx.Request.Path);

        if (!HttpMethods.IsGet(ctx.Request.Method))
        {
            logger.LogNotGetRequest(ctx.Request.Method, traceId);
            return false;
        }

        if (ctx.Response.Headers.ETag.Count > 0)
        {
            logger.LogEtagHeaderPresented(traceId);
            return false;
        }

        if (!opts.Filter(ctx))
        {
            logger.LogFilterRejected(traceId);
            return false;
        }

        logger.LogContextFilterFinished(traceId);
        return true;
    }
}
