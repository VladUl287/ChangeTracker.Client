using Microsoft.Extensions.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Tracker.AspNet.Services.Contracts;

public interface IDirectiveChecker
{
    bool AnyInvalidDirective(StringValues headers, ReadOnlySpan<string> invalidDirectives, [NotNullWhen(true)] out string? directive);
}
