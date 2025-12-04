namespace Tracker.AspNet.Services.Contracts;

public interface IDbOperations
{
    //string ConnectionId { get; init; }
    string Provider { get; init; }
    Task<DateTimeOffset?> GetLastTimestamp(string table, CancellationToken token);
    Task<uint?> GetLastCommittedXact(CancellationToken token);
}

public interface IDbOperations<TContext> : IDbOperations
{
    //Type Context { get; init; }
}
