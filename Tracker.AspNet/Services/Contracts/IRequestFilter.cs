using Microsoft.AspNetCore.Http;

namespace Tracker.AspNet.Services.Contracts;

public interface IRequestFilter
{
    bool ShouldProcessRequest<TState>(HttpContext context, Func<TState, bool> filter, TState state);
}
