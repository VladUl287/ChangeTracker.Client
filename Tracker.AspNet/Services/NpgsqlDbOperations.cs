using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Tracker.AspNet.Services.Contracts;

namespace Tracker.AspNet.Services;

public sealed class NpgsqlDbOperations : IDbOperations
{
    public string Provider { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

    public Task<uint?> GetLastCommittedXact(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task<DateTimeOffset?> GetLastTimestamp(string table, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

public sealed class NpgsqlDbOperations<TContext>(IServiceScopeFactory scopeFactory) : IDbOperations<TContext>
    where TContext : DbContext
{
    public string Provider { get; init; } = nameof(Npgsql.EntityFrameworkCore.PostgreSQL);

    public async Task<DateTimeOffset?> GetLastTimestamp(string table, CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        using var dbContext = scope.ServiceProvider.GetService<TContext>();

        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentException.ThrowIfNullOrEmpty(table, nameof(table));

        var database = dbContext.Database;
        var connection = database.GetDbConnection();

        await using var command = connection.CreateCommand();

        await connection.OpenAsync(token);

        try
        {
            command.CommandText = "SELECT get_last_timestamp(@table_name);";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@table_name";
            parameter.Value = table;
            parameter.DbType = DbType.String;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync(token);
            var res = result?.ToString();
            return string.IsNullOrEmpty(res) ? null : DateTimeOffset.Parse(res);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public Task<uint?> GetLastCommittedXact(CancellationToken token)
    {
        throw new NotImplementedException();
    }

}
