using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Frozen;
using Tracker.AspNet.Logging;
using Tracker.AspNet.Models;
using Tracker.AspNet.Services.Contracts;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class DefaultProviderResolver(
    IEnumerable<ISourceProvider> providers,
    IProviderIdGenerator idGenerator,
    ILogger<DefaultProviderResolver> logger) : IProviderResolver
{
    private readonly FrozenDictionary<string, ISourceProvider> _store = providers.ToFrozenDictionary(c => c.Id);
    private readonly ISourceProvider _default = providers.First();

    public ISourceProvider? SelectProvider(string? providerId, ImmutableGlobalOptions options)
    {
        if (providerId is not null)
        {
            if (_store.TryGetValue(providerId, out var provider))
            {
                logger.LogProviderResolvedFromExplicitId(providerId);
                return provider;
            }

            throw new InvalidOperationException($"Fail to resolve provider for explicit provider ID - '{providerId}'");
        }

        if (options is { SourceProvider: null, SourceProviderFactory: null })
        {
            logger.LogNoProviderIdNoSourceOpsReturningDefault(_default.Id);
            return _default;
        }

        logger.LogNoExplicitProviderIdReturningFromOptions();
        return options.SourceProvider;
    }

    public ISourceProvider? SelectProvider(GlobalOptions options)
    {
        var sourceId = options.Source;

        if (sourceId is not null)
        {
            if (_store.TryGetValue(sourceId, out var provider))
            {
                logger.LogProviderResolvedFromGlobalOptions(sourceId);
                return provider;
            }

            throw new InvalidOperationException(
                $"Failed to resolve source provider from GlobalOptions. Provider with ID '{sourceId}' was not found. " +
                $"Available provider IDs: {string.Join(", ", _store.Keys)}");
        }

        if (options is { SourceProvider: null, SourceProviderFactory: null })
        {
            logger.LogNoSourceIdNoOpsReturningDefault(_default.Id);
            return _default;
        }

        logger.LogNoProviderIdReturningFromGlobalOptions();
        return options.SourceProvider;
    }

    public ISourceProvider? SelectProvider<TContext>(string? sourceId, ImmutableGlobalOptions options) where TContext : DbContext
    {
        var contextTypeName = typeof(TContext).Name;

        if (sourceId is not null)
        {
            if (_store.TryGetValue(sourceId, out var provider))
            {
                logger.LogProviderResolvedFromExplicitIdForContext(sourceId, contextTypeName);
                return provider;
            }

            throw new InvalidOperationException(
                $"Failed to resolve source provider for context '{contextTypeName}'. " +
                $"Provider with ID '{sourceId}' was not found. " +
                $"Available provider IDs: {string.Join(", ", _store.Keys)}");
        }

        logger.LogNoExplicitIdAttemptingGeneration(contextTypeName);

        sourceId = idGenerator.GenerateId<TContext>();

        if (_store.TryGetValue(sourceId, out var contextProvider))
        {
            logger.LogProviderResolvedFromGeneratedId(sourceId, contextTypeName);
            return contextProvider;
        }

        if (options is { SourceProvider: null, SourceProviderFactory: null })
        {
            logger.LogGeneratedIdNotFoundNoOpsReturningDefault(sourceId, contextTypeName, _default.Id);
            return _default;
        }

        logger.LogGeneratedIdNotFoundReturningFromOptions(sourceId, contextTypeName);
        return options.SourceProvider;
    }

    public ISourceProvider? SelectProvider<TContext>(GlobalOptions options) where TContext : DbContext
    {
        var contextTypeName = typeof(TContext).Name;
        var sourceId = options.Source;

        if (sourceId is not null)
        {
            if (_store.TryGetValue(sourceId, out var provider))
            {
                logger.LogProviderResolvedFromGlobalOptionsForContext(sourceId, contextTypeName);
                return provider;
            }

            throw new InvalidOperationException(
                $"Failed to resolve source provider for context '{contextTypeName}' from GlobalOptions. " +
                $"Provider with ID '{sourceId}' was not found. " +
                $"Available provider IDs: {string.Join(", ", _store.Keys)}");
        }

        logger.LogNoSourceIdInGlobalOptionsAttemptingGeneration(contextTypeName);

        sourceId = idGenerator.GenerateId<TContext>();

        logger.LogGeneratedIdForContext(sourceId, contextTypeName);

        if (_store.TryGetValue(sourceId, out var contextProvider))
        {
            logger.LogProviderResolvedFromGeneratedIdForContext(sourceId, contextTypeName);
            return contextProvider;
        }

        if (options is { SourceProvider: null, SourceProviderFactory: null })
        {
            logger.LogGeneratedIdNotFoundNoOpsInGlobalOptionsReturningDefault(sourceId, contextTypeName, _default.Id);
            return _default;
        }

        logger.LogGeneratedIdNotFoundReturningFromGlobalOptions(sourceId, contextTypeName);
        return options.SourceProvider;
    }
}
