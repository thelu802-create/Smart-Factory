using Microsoft.Data.Sqlite;

namespace SmartFactory.Api.Data;

/// <summary>
/// Resolves the SQLite connection string the same way SampleDataService does
/// (relative paths are rooted against the content root) and opens live connections
/// for repositories that need to read and write, not just the startup snapshot.
/// </summary>
public sealed class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IWebHostEnvironment environment, IConfiguration configuration)
    {
        var configured = configuration.GetConnectionString("SmartFactoryDatabase")
            ?? "Data Source=database/smart_factory_demo.db";
        _connectionString = Normalize(environment.ContentRootPath, configured);
        EnsureDataDirectoryExists();
    }

    private void EnsureDataDirectoryExists()
    {
        var dataSource = new SqliteConnectionStringBuilder(_connectionString).DataSource;
        if (string.IsNullOrWhiteSpace(dataSource) || dataSource == ":memory:")
        {
            return;
        }

        var directory = Path.GetDirectoryName(dataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    /// <summary>True when the SQLite database file exists and can be opened.</summary>
    public bool IsAvailable()
    {
        try
        {
            using var connection = CreateOpenConnection();
            return true;
        }
        catch (SqliteException)
        {
            return false;
        }
    }

    public SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }

    private static string Normalize(string contentRootPath, string configuredConnectionString)
    {
        var builder = new SqliteConnectionStringBuilder(configuredConnectionString);
        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            builder.DataSource = Path.Combine(contentRootPath, "database", "smart_factory_demo.db");
        }
        else if (!Path.IsPathRooted(builder.DataSource) && builder.DataSource != ":memory:")
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, builder.DataSource));
        }

        return builder.ToString();
    }
}
