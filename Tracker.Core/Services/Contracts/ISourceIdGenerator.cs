using Microsoft.EntityFrameworkCore;

namespace Tracker.Core.Services.Contracts;

public interface ISourceIdGenerator
{
    string GenerateId<TContext>() where TContext : DbContext;
}
