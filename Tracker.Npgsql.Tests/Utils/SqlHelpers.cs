using Npgsql;

namespace Tracker.Npgsql.Tests.Utils;

internal static class SqlHelpers
{
    internal static async Task CreateTestTable(string connectionString, string tableName)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        using var createTableCmd = new NpgsqlCommand(
            $@"CREATE TABLE IF NOT EXISTS {tableName} (
                id SERIAL PRIMARY KEY,
                value INT
            )", connection);

        await createTableCmd.ExecuteNonQueryAsync();
    }

    internal static async Task DropTable(string connectionString, string tableName)
    {
        using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        using var createTableCmd = new NpgsqlCommand($"DROP TABLE IF EXISTS {tableName}", connection);

        await createTableCmd.ExecuteNonQueryAsync();
    }

    internal static async Task CreateDatabaseIfNotExists(string connectionString, string databaseName)
    {
        using var masterDataSource = new NpgsqlDataSourceBuilder(connectionString).Build();

        using var checkCmd = masterDataSource.CreateCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'");

        var exists = await checkCmd.ExecuteScalarAsync();
        if (exists is null)
        {
            using var createCmd = masterDataSource.CreateCommand($"CREATE DATABASE {databaseName}");
            await createCmd.ExecuteNonQueryAsync();
        }
    }
}
