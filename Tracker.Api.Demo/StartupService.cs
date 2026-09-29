using Tracker.Core.Services.Contracts;

namespace Tracker.Api.Demo;

public class StartupService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var sourceProvider = scope.ServiceProvider.GetKeyedServices<ISourceProvider>(KeyedService.AnyKey);
        await sourceProvider
            .First()
            .EnableTracking("roles", stoppingToken);
    }
}
