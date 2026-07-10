using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Models;
using JsonPlaceholderApiTests.Utils;

namespace JsonPlaceholderApiTests.Api;

public class PostsApi : ApiClient
{
    private readonly string _postsEndpoint;

    public PostsApi(ApiSettings settings) : base(settings)
    {
        _postsEndpoint = settings.PostsEndpoint;
    }

    public Task<ApiResponse<List<Post>>> GetAllPostsAsync()
    {
        TestLogger.LogInfo("PostsApi.GetAllPostsAsync — получение всех постов.");
        return GetAsync<List<Post>>(_postsEndpoint);
    }

    public Task<ApiResponse<Post>> GetPostByIdAsync(int postId)
    {
        TestLogger.LogInfo($"PostsApi.GetPostByIdAsync — получение поста. Id: {postId}");
        return GetAsync<Post>(BuildPath(_postsEndpoint, postId.ToString()));
    }

    public Task<ApiResponse<Post>> CreatePostAsync(PostCreateRequest request)
    {
        TestLogger.LogInfo($"PostsApi.CreatePostAsync — создание поста. UserId: {request.UserId}, Title: {request.Title}");
        return PostAsync<PostCreateRequest, Post>(_postsEndpoint, request);
    }
}
