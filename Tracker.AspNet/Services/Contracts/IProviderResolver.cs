using Tracker.AspNet.Models;
using Microsoft.EntityFrameworkCore;
using Tracker.Core.Services.Contracts;

namespace Tracker.AspNet.Services.Contracts;

public interface IProviderResolver
{
    ISourceOperations? SelectProvider(string? sourceId, ImmutableGlobalOptions options);
    ISourceOperations? SelectProvider(GlobalOptions options);

    ISourceOperations? SelectProvider<TContext>(string? sourceId, ImmutableGlobalOptions options) where TContext : DbContext;
    ISourceOperations? SelectProvider<TContext>(GlobalOptions options) where TContext : DbContext;
}
