using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class DefaultProvidersValidator(IEnumerable<ISourceProvider> operations) : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        ValidateSourceOperationsProviders(operations);
        return next;
    }

    private void ValidateSourceOperationsProviders(IEnumerable<ISourceProvider> operations)
    {
        if (!operations.Any())
            throw new InvalidOperationException(
                $"At least one {nameof(ISourceProvider)} implementation is required");
    }
}