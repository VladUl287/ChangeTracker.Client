using Microsoft.Extensions.Logging;

namespace Tracker.AspNet.Logging;

public static partial class ProviderResolverLogging
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Provider '{providerId}' successfully resolved from explicit provider ID.")]
    public static partial void LogProviderResolvedFromExplicitId(this ILogger logger, string providerId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No explicit provider ID provided and no source operations configured. Returning default provider '{defaultProviderId}'.")]
    public static partial void LogNoProviderIdNoSourceOpsReturningDefault(this ILogger logger, string defaultProviderId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No explicit source ID provided. Returning source operations from options.")]
    public static partial void LogNoExplicitProviderIdReturningFromOptions(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Provider '{providerId}' successfully resolved from GlobalOptions source.")]
    public static partial void LogProviderResolvedFromGlobalOptions(this ILogger logger, string providerId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No source ID in GlobalOptions and no source operations configured. Returning default provider '{defaultProviderId}'.")]
    public static partial void LogNoSourceIdNoOpsReturningDefault(this ILogger logger, string defaultProviderId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No provider ID in GlobalOptions. Returning source operations from GlobalOptions.")]
    public static partial void LogNoProviderIdReturningFromGlobalOptions(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Provider '{providerId}' successfully resolved from explicit source ID for context '{contextType}'.")]
    public static partial void LogProviderResolvedFromExplicitIdForContext(this ILogger logger, string providerId, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No explicit source ID provided for context '{contextType}'. Attempting to generate provider ID based on context type.")]
    public static partial void LogNoExplicitIdAttemptingGeneration(this ILogger logger, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Provider '{providerId}' successfully resolved from generated ID for context '{contextType}'.")]
    public static partial void LogProviderResolvedFromGeneratedId(this ILogger logger, string providerId, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Generated provider ID '{generatedId}' not found for context '{contextType}' and no source operations configured. Returning default provider '{defaultProviderId}'.")]
    public static partial void LogGeneratedIdNotFoundNoOpsReturningDefault(this ILogger logger, string generatedId, string contextType, string defaultProviderId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Generated provider ID '{generatedId}' not found for context '{contextType}'. Returning source operations from options.")]
    public static partial void LogGeneratedIdNotFoundReturningFromOptions(this ILogger logger, string generatedId, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Provider '{providerId}' successfully resolved from GlobalOptions source for context '{contextType}'.")]
    public static partial void LogProviderResolvedFromGlobalOptionsForContext(this ILogger logger, string providerId, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "No source ID in GlobalOptions for context '{contextType}'. Attempting to generate provider ID based on context type.")]
    public static partial void LogNoSourceIdInGlobalOptionsAttemptingGeneration(this ILogger logger, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Generated provider ID '{generatedId}' for context '{contextType}'.")]
    public static partial void LogGeneratedIdForContext(this ILogger logger, string generatedId, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Provider '{providerId}' successfully resolved from generated ID for context '{contextType}'.")]
    public static partial void LogProviderResolvedFromGeneratedIdForContext(this ILogger logger, string providerId, string contextType);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Generated provider ID '{generatedId}' not found for context '{contextType}' and no source operations configured in GlobalOptions. Returning default provider '{defaultProviderId}'.")]
    public static partial void LogGeneratedIdNotFoundNoOpsInGlobalOptionsReturningDefault(this ILogger logger, string generatedId, string contextType, string defaultProviderId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Generated provider ID '{generatedId}' not found for context '{contextType}'. Returning source operations from GlobalOptions.")]
    public static partial void LogGeneratedIdNotFoundReturningFromGlobalOptions(this ILogger logger, string generatedId, string contextType);
}