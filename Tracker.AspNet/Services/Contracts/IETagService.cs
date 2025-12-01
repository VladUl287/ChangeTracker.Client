using Microsoft.AspNetCore.Http;
using Tracker.AspNet.Models;

namespace Tracker.AspNet.Services.Contracts;

public interface IETagService
{
    Task<bool> TrySetETagAsync(HttpContext context, string[] tables, CancellationToken token = default);
}
