using MySqlConnector;

namespace JsonPlaceholderApiTests.Configuration;

public class DatabaseSettings
{
    public required string Host { get; set; }
    public required int Port { get; set; }
    public required string Database { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }

    public string BuildConnectionString()
    {
        return new MySqlConnectionStringBuilder
        {
            Server = Host,
            Port = (uint)Port,
            Database = Database,
            UserID = User,
            Password = Password
        }.ConnectionString;
    }
}
