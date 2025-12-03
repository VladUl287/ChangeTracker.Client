using Microsoft.AspNetCore.Http;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class TrackerEndpointFilter(
    IETagService eTagService, IRequestFilter requestFilter, ImmutableGlobalOptions options) : IEndpointFilter
{
    private readonly IETagService _eTagService = eTagService ?? throw new NullReferenceException();
    private readonly IRequestFilter _requestFilter = requestFilter ?? throw new NullReferenceException();
    private readonly ImmutableGlobalOptions _options = options ?? throw new NullReferenceException();

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var httpCtx = context.HttpContext;
        var token = httpCtx.RequestAborted;

        var shouldProcessRequest = _requestFilter.ShouldProcessRequest(httpCtx, _options);
        if (!shouldProcessRequest)
            return await next(context);

        if (token.IsCancellationRequested)
            return Results.BadRequest();

        var shouldReturnNotModified = await _eTagService.TrySetETagAsync(httpCtx, _options, token);
        if (!shouldReturnNotModified)
            return await next(context);

        return Results.StatusCode(StatusCodes.Status304NotModified);
    }
}
