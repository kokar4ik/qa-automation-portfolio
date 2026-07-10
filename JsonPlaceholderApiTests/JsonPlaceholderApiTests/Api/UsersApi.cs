using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Models;
using JsonPlaceholderApiTests.Utils;

namespace JsonPlaceholderApiTests.Api;

public class UsersApi : ApiClient
{
    private readonly string _usersEndpoint;

    public UsersApi(ApiSettings settings) : base(settings)
    {
        _usersEndpoint = settings.UsersEndpoint;
    }

    public Task<ApiResponse<List<User>>> GetAllUsersAsync()
    {
        TestLogger.LogInfo("UsersApi.GetAllUsersAsync — получение всех пользователей.");
        return GetAsync<List<User>>(_usersEndpoint);
    }

    public Task<ApiResponse<User>> GetUserByIdAsync(int userId)
    {
        TestLogger.LogInfo($"UsersApi.GetUserByIdAsync — получение пользователя. Id: {userId}");
        return GetAsync<User>(BuildPath(_usersEndpoint, userId.ToString()));
    }
}
