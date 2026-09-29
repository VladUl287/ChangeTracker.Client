using Microsoft.AspNetCore.Http;

namespace Tracker.AspNet;

public readonly struct RequestId(HttpContext httpContext)
{
    public override string ToString() => httpContext.TraceIdentifier;

    public static implicit operator RequestId(HttpContext ctx) => new(ctx);
}
