using Microsoft.AspNetCore.Http;

namespace Npgsql.EFCore.Tracker.AspNet.Services.Contracts;

public interface IPathResolver
{
    string Resolve(HttpContext context);
}
