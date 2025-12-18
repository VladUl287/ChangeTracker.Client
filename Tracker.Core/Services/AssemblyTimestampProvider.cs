using System.Reflection;
using Tracker.Core.Services.Contracts;

namespace Tracker.Core.Services;

public sealed class AssemblyTimestampProvider(Assembly assembly) : IAssemblyTimestampProvider
{
    public DateTimeOffset GetWriteTime()
    {
        ArgumentNullException.ThrowIfNull(assembly, nameof(assembly));

        if (!File.Exists(assembly.Location))
            throw new FileNotFoundException($"Assembly file not found at '{assembly.Location}'");

        return File.GetLastWriteTimeUtc(assembly.Location);
    }
}
