using FluentAssertions;
using MySqlConnector;

namespace JsonPlaceholderApiTests.Tests;

public class DatabaseConnectionTests : DatabaseTestBase
{
    [Test]
    public async Task CanConnectToDatabase()
    {
        DatabaseConnection.IsOpen.Should().BeTrue();

        await using var command = new MySqlCommand("SELECT 1", DatabaseConnection.Connection);
        var result = await command.ExecuteScalarAsync();

        Convert.ToInt32(result).Should().Be(1);
    }
}