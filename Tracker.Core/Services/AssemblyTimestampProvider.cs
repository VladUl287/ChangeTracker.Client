using System.Reflection;
using Tracker.Core.Extensions;
using Tracker.Core.Services.Contracts;

namespace Tracker.Core.Services;

public class AssemblyTimestampProvider(Assembly assembly) : IAssemblyTimestampProvider
{
    public DateTimeOffset GetWriteTime() => assembly.GetAssemblyWriteTime();
}
