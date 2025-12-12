namespace Tracker.Core.Services.Contracts;

public interface IAssemblyTimestampProvider
{
    DateTimeOffset GetWriteTime();
}
