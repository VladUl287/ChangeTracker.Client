using FastEndpoints;
using Tracker.FastEndpoints.Services.Contracts;

namespace Tracker.FastEndpoints;

public sealed class TrackerPreProcessor(ITrackerProcessor trackerProcessor) : IPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
        => trackerProcessor.ProcessAsync(context.HttpContext, ct);
}

public sealed class TrackerPreProcessor<TRequest>(ITrackerProcessor trackerProcessor) : IPreProcessor<TRequest>
{
    public Task PreProcessAsync(IPreProcessorContext<TRequest> context, CancellationToken ct)
        => trackerProcessor.ProcessAsync(context.HttpContext, ct);
}

public sealed class GlobalTrackerPreProcessor(ITrackerProcessor trackerProcessor) : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
        => trackerProcessor.ProcessAsync(context.HttpContext, ct);
}
