using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class TrackAttribute(string[]? tables = null, Type[]? entities = null) : Attribute, IAsyncActionFilter
{
    public string[]? Tables { get; } = tables;
    public Type[]? Entities { get; } = entities;

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (IsGetMethod(context.HttpContext))
        {
            var etagService = context.HttpContext.RequestServices.GetRequiredService<IETagService>();
            var token = context.HttpContext.RequestAborted;

            if (await etagService.TrySetETagAsync(context.HttpContext, tables, token))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
                return;
            }
        }

        await next();
    }

    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        return Task.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsGetMethod(HttpContext context) => context.Request.Method == HttpMethod.Get.Method;
}