using Microsoft.Data.SqlClient;

namespace SmartFactory.Api.Data;

/// <summary>
/// Opens live connections to the SmartFactory SQL Server database (Windows
/// Authentication). This is the connection the app will move onto for the
/// code-first (EF Core) work; for now it exists so backend connectivity to the
/// SmartFactory instance can be established and verified independently of the
/// current SQLite data layer.
/// </summary>
public sealed class SqlServerConnectionFactory
{
    private readonly string? _connectionString;

    public SqlServerConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SmartFactorySqlServer");
    }

    /// <summary>The configured connection string, or null when none is set.</summary>
    public string? ConnectionString => _connectionString;

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_connectionString);

    /// <summary>Opens a new SQL Server connection. Caller owns disposal.</summary>
    public SqlConnection CreateOpenConnection()
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException("Connection string 'SmartFactorySqlServer' is not configured.");
        }

        var connection = new SqlConnection(_connectionString);
        connection.Open();
        return connection;
    }

    /// <summary>Connectivity probe result surfaced by the /health/database endpoint.</summary>
    public sealed record ProbeResult(bool Connected, string? Server, string? Database, string? ServerVersion, int TableCount, string? Error);

    /// <summary>
    /// Tries to connect and read basic server/database facts. Never throws — returns
    /// Connected = false with the error message so a failed connection is diagnosable.
    /// </summary>
    public ProbeResult Probe()
    {
        if (!IsConfigured)
        {
            return new ProbeResult(false, null, null, null, 0, "Connection string 'SmartFactorySqlServer' is not configured.");
        }

        try
        {
            using var connection = CreateOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DB_NAME(), @@SERVERNAME, (SELECT COUNT(*) FROM sys.tables)";
            using var reader = command.ExecuteReader();
            reader.Read();
            var database = reader.IsDBNull(0) ? null : reader.GetString(0);
            var server = reader.IsDBNull(1) ? null : reader.GetString(1);
            var tableCount = reader.GetInt32(2);
            return new ProbeResult(true, server, database, connection.ServerVersion, tableCount, null);
        }
        catch (Exception exception)
        {
            return new ProbeResult(false, null, null, null, 0, exception.Message);
        }
    }
}
