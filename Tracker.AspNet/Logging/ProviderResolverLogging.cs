using Microsoft.Extensions.Logging;

namespace Tracker.AspNet.Logging;

public static partial class ProviderResolverLogging
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "Resolving keyed provider: {ProviderId}. TraceId: {TraceId}")]
    public static partial void LogResolvingKeyedProvider(this ILogger logger, string? providerId, TraceId traceId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Using direct provider instance. TraceId: {TraceId}")]
    public static partial void LogUsingDirectProviderInstance(this ILogger logger, TraceId traceId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Creating provider via factory. TraceId: {TraceId}")]
    public static partial void LogCreatingProviderViaFactory(this ILogger logger, TraceId traceId);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Provider not specified. Resolving last registered provider. TraceId: {TraceId}")]
    public static partial void LogResolvingLastRegisteredProvider(this ILogger logger, TraceId traceId);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to resolve source provider. TraceId: {TraceId}")]
    public static partial void LogFailedToResolveSourceProvider(this ILogger logger, Exception ex, TraceId traceId);
}