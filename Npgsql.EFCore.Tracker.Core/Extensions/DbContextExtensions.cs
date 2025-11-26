using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Npgsql.EFCore.Tracker.Core.Extensions;

public static class DbContextExtensions
{
    public static async Task<string?> GetLastTimestamp(this DbContext dbContext, string tableName, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            throw new ArgumentException("Table name cannot be null or empty", nameof(tableName));

        var database = dbContext.Database;
        var connection = database.GetDbConnection();
        var transaction = database.CurrentTransaction?.GetDbTransaction();

        await using var command = connection.CreateCommand();
        command.Transaction = transaction;

        await connection.OpenAsync(token);

        try
        {
            command.CommandText = "SELECT get_last_timestamp(@table_name);";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "@table_name";
            parameter.Value = tableName;
            parameter.DbType = DbType.String;
            command.Parameters.Add(parameter);

            var result = await command.ExecuteScalarAsync(token);
            return result?.ToString();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public static async Task<uint?> GetLastCommittedXact(this DbContext dbContext, CancellationToken token)
    {
        var database = dbContext.Database;
        var connection = database.GetDbConnection();
        var transaction = database.CurrentTransaction?.GetDbTransaction();

        await using var command = connection.CreateCommand();
        command.Transaction = transaction;

        await connection.OpenAsync(token);

        try
        {
            command.CommandText = "SELECT pg_last_committed_xact();";

            var results = (object?[]?)await command.ExecuteScalarAsync(token);
            var xid = results?[0];
            return xid is null ? null : (uint)xid;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}
