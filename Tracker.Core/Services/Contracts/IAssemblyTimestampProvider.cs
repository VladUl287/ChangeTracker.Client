namespace Tracker.Core.Services.Contracts;

/// <summary>
/// Provides a mechanism to retrieve the timestamp of an assembly.
/// </summary>
public interface IAssemblyTimestampProvider
{
    /// <summary>
    /// Gets the last write time of the assembly.
    /// </summary>
    /// <returns>
    /// A <see cref="DateTimeOffset"/> representing the last write time of the assembly.
    /// </returns>
    DateTimeOffset GetWriteTime();
}