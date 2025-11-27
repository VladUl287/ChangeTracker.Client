using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EFCore.Tracker.AspNet.Services.Contracts;
using Npgsql.EFCore.Tracker.AspNet.Utils;
using Npgsql.EFCore.Tracker.Core.Extensions;
using System.Net;

namespace Npgsql.EFCore.Tracker.AspNet.Middlewares;

public sealed class TrackMiddleware<TContext>(RequestDelegate next, IActionsRegistry registry)
    where TContext : DbContext
{
    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;

        if (method == "GET")
        {
            var path = context.Request.GetEncodedPathAndQuery();

            var descriptor = registry.Get(path);
            if (descriptor is not null)
            {
                var dbContext = context.RequestServices.GetService<TContext>();

                if (dbContext is not null)
                {
                    var lastTimestamp = await dbContext.GetLastTimestamp(descriptor.Tables, default);
                    if (!string.IsNullOrEmpty(lastTimestamp))
                    {
                        var dateTime = DateTimeOffset.Parse(lastTimestamp);
                        var etag = ETagUtils.GenETagTicks(dateTime);

                        if (context.Request.Headers["If-None-Match"] == etag)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                            return;
                        }

                        context.Response.Headers["ETag"] = etag;
                    }
                }
            }
        }

        await next(context);
    }
}
