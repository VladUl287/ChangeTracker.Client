using Microsoft.Extensions.Logging;

namespace Tracker.AspNet.Logging;

public static partial class RequestFilterLogging
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Request validation failed: Method '{method}' is not GET for path {path}")]
    public static partial void LogNotGetRequest(this ILogger logger, string method, string path);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request validation failed: ETag header present for path {path}")]
    public static partial void LogEtagHeaderPresent(this ILogger logger, string path);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request validation failed: Not valid cache control directive detected. TraceId - {TraceId}, Path = {path}")]
    public static partial void LogNotValidCacheControlDirectiveDetected(this ILogger logger, string traceId, string path);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request validation failed: Custom filter rejected path {path}")]
    public static partial void LogFilterRejected(this ILogger logger, string path);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request validation passed for path {path}")]
    public static partial void LogRequestValidated(this ILogger logger, string path);
}
