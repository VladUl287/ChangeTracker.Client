namespace Tracker.SqlServer.Tests.Utils;

internal static class TestConfiguration
{
    internal static string GetSqlConnectionString() =>
        "Data Source=localhost,1433;User ID=sa;Password=Password1;Database=TrackerTestDb;TrustServerCertificate=True;";

    internal static string GetSqlLowPrivilageConnectionString() =>
        "Data Source=localhost,1433;User ID=lowprivilege;Password=Password1;Database=TrackerTestDb;TrustServerCertificate=True;";
}

