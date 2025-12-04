namespace Tracker.AspNet.Services.Contracts;

public interface IDbOperationsFactory
{
    IDbOperations Create(string provider);
    IDbOperations<TContext> Create<TContext>(string provider);
}
