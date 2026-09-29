using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Tracker.AspNet.Logging;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class DefaultRequestHandler(
    IETagProvider etagProvider, IProviderResolver providerResolver, ILogger<DefaultRequestHandler> logger) : IRequestHandler
{
    public async ValueTask<bool> HandleRequest(HttpContext ctx, TrackOptionsSnapshot options, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(ctx, nameof(ctx));
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        var reqId = new RequestId(ctx);

        logger.LogRequestHandleStarted(reqId);

        var provider = providerResolver.ResolveProvider(ctx, options, out var canDispose);
        try
        {
            var lastTimestamp = await GetLastVersionAsync(options, provider, token);

            var notModified = NotModified(ctx, options, reqId, lastTimestamp, out var suffix);
            if (notModified)
            {
                logger.LogNotModified(reqId);
                return true;
            }

            var etag = etagProvider.Generate(lastTimestamp, suffix);
            ctx.Response.Headers.CacheControl = options.CacheControl;
            ctx.Response.Headers.ETag = etag;

            logger.LogETagAdded(etag, reqId);
            return false;
        }
        finally
        {
            if (canDispose && provider is IDisposable d)
                d.Dispose();

            logger.LogRequestHandleFinished(reqId);
        }
    }

    private bool NotModified(HttpContext ctx, TrackOptionsSnapshot options, RequestId reqId, ulong lastTimestamp, out string suffix)
    {
        suffix = string.Empty;

        if (ctx.Request.Headers.IfNoneMatch.Count == 0)
            return false;

        var ifNoneMatch = ctx.Request.Headers.IfNoneMatch[0];
        if (ifNoneMatch is null)
            return false;

        suffix = options.Suffix(ctx);
        if (!etagProvider.Compare(ifNoneMatch, lastTimestamp, suffix))
            return false;

        ctx.Response.StatusCode = StatusCodes.Status304NotModified;
        logger.LogNotModified(reqId, ifNoneMatch);
        return true;
    }

    private static async ValueTask<ulong> GetLastVersionAsync(
        TrackOptionsSnapshot options, ISourceProvider sourceOperations, CancellationToken token)
    {
        return options.Tables.Length switch
        {
            0 => (ulong)await sourceOperations.GetVersion(token),
            1 => (ulong)await sourceOperations.GetVersion(options.Tables[0], token),
            _ => (ulong)await sourceOperations.GetLatestVersion([.. options.Tables], token),
        };
    }
}
