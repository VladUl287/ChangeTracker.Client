using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tracker.AspNet.Extensions;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class TrackAttribute() : Attribute, IAsyncActionFilter
{
    public TrackAttribute(string[] tables) : this()
    {
        ArgumentNullException.ThrowIfNull(tables, nameof(tables));
        Tables = tables;
    }

    public string[] Tables { get; } = [];

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<TrackAttribute>>();

        if (!context.HttpContext.IsGetRequest())
        {
            logger.LogInformation("Not 'GET' request but '{request}'. Ignore tracker logic.", context.HttpContext.Request.Method);
            await next();
            return;
        }

        var options = context.HttpContext.RequestServices.GetRequiredService<GlobalOptions>();
        if (!options.Filter(context.HttpContext))
        {
            logger.LogInformation("Request '{request}' filtered by options. Ignore tracker logic.", context.HttpContext.Request.Method);
            await next();
            return;
        }

        options = options.Copy();
        options.Tables = Tables;

        var etagService = context.HttpContext.RequestServices.GetRequiredService<IETagService>();
        var token = context.HttpContext.RequestAborted;

        var shouldReturnNotModified = await etagService.TrySetETagAsync(context.HttpContext, options, token);
        if (shouldReturnNotModified)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            return;
        }

        await next();
    }
}