using System.Data;
using JsonPlaceholderApiTests.Configuration;
using MySqlConnector;

namespace JsonPlaceholderApiTests.Database;

public class DatabaseConnection : IAsyncDisposable, IDisposable
{
    private readonly DatabaseSettings _settings;
    private MySqlConnection? _connection;

    public DatabaseConnection(DatabaseSettings settings)
    {
        _settings = settings;
    }

    public MySqlConnection Connection =>
        _connection ?? throw new InvalidOperationException("Database connection is not open.");

    public bool IsOpen => _connection?.State == ConnectionState.Open;

    public void Open()
    {
        if (_connection is not null)
        {
            return;
        }

        _connection = new MySqlConnection(_settings.BuildConnectionString());
        _connection.Open();
    }

    public async Task OpenAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is not null)
        {
            return;
        }

        _connection = new MySqlConnection(_settings.BuildConnectionString());
        await _connection.OpenAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (_connection is null)
        {
            return;
        }

        _connection.Dispose();
        _connection = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is null)
        {
            return;
        }

        await _connection.DisposeAsync();
        _connection = null;
    }
}
