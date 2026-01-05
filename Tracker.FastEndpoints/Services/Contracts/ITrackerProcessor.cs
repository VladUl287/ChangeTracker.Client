using Microsoft.AspNetCore.Http;

namespace Tracker.FastEndpoints.Services.Contracts;

public interface ITrackerProcessor
{
    Task ProcessAsync(HttpContext context, CancellationToken token);
}
