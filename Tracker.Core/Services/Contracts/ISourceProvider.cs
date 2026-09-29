namespace Tracker.Core.Services.Contracts;

public interface ISourceProvider : IDisposable
{
    string Id { get; }

    ValueTask<long> GetVersion(CancellationToken token = default);

    ValueTask<long> GetVersion(string key, CancellationToken token = default);

    ValueTask<long> GetLastVersion(string[] keys, CancellationToken token = default);

    ValueTask<bool> SetVersion(string key, long value, CancellationToken token = default);

    ValueTask<bool> EnableTracking(string key, CancellationToken token = default);

    ValueTask<bool> DisableTracking(string key, CancellationToken token = default);

    ValueTask<bool> IsTrackingEnabled(string key, CancellationToken token = default);
}
