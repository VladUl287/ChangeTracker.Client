using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services.Contracts;

public interface ISourceOperationsResolver
{
    bool Registered(string sourceId);
    ISourceOperations Resolve(string? sourceId);
}
