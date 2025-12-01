using Microsoft.AspNetCore.Http;

namespace Tracker.AspNet.Filters;

public sealed class ETagEndpointFilter : IEndpointFilter
{
    public ETagEndpointFilter()
    {
        
    }

    public ETagEndpointFilter(string[] tables)
    {
        
    }
    
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        return await next(context);
    }
}
