using Microsoft.Extensions.DependencyInjection;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class DbOperationsFactory : IDbOperationsFactory
{
    private readonly IServiceProvider serviceProvider;

    public DbOperationsFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IDbOperations Create(string provider)
    {
        throw new NotImplementedException();
    }

    public IDbOperations<TContext> Create<TContext>(string provider)
    {
        return serviceProvider.GetRequiredService<IDbOperations<TContext>>();
    }
}
