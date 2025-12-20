namespace Tracker.Npgsql.Tests.Utils;

internal static class TestConfiguration
{
    internal static string GetSqlConnectionString() =>
        "Host=localhost;Port=5432;Database=main;Username=postgres;Password=postgres";

    internal static string GetSqlLowPrivilageConnectionString() =>
        "Host=localhost;Port=5432;Database=main;Username=lowprivilagepostgres;Password=lowprivilagepostgres";
}
