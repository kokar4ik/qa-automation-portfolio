using System.Net;
using FluentAssertions;
using JsonPlaceholderApiTests.Api;
using JsonPlaceholderApiTests.Configuration;
using JsonPlaceholderApiTests.Models;
using JsonPlaceholderApiTests.Utils;

namespace JsonPlaceholderApiTests.Tests;

public static class JsonPlaceholderScenarioRunner
{
    public static async Task RunSixStepsAsync(
        PostsApi postsApi,
        UsersApi usersApi,
        TestDataSettings testData)
    {
        TestLogger.LogInfo("Запуск сценария JsonPlaceholder API (6 шагов).");

        TestLogger.LogStep("Шаг 1: GET /posts — проверка 200, непустой список, сортировка по id.");
        var allPostsResponse = await postsApi.GetAllPostsAsync();

        allPostsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        allPostsResponse.Body.Should().NotBeNull();
        allPostsResponse.Body.Should().NotBeEmpty();
        allPostsResponse.Body!.Should().BeInAscendingOrder(post => post.Id);
        TestLogger.LogInfo($"Шаг 1 пройден. Количество постов: {allPostsResponse.Body!.Count}.");

        TestLogger.LogStep($"Шаг 2: GET /posts/{testData.ExistingPostId} — проверка 200 и полей поста.");
        var existingPostResponse = await postsApi.GetPostByIdAsync(testData.ExistingPostId);

        existingPostResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        existingPostResponse.Body.Should().NotBeNull();
        existingPostResponse.Body!.UserId.Should().Be(testData.ExistingPostUserId);
        existingPostResponse.Body.Id.Should().Be(testData.ExistingPostId);
        existingPostResponse.Body.Title.Should().NotBeNullOrWhiteSpace();
        existingPostResponse.Body.Body.Should().NotBeNullOrWhiteSpace();
        TestLogger.LogInfo("Шаг 2 пройден.");

        TestLogger.LogStep($"Шаг 3: GET /posts/{testData.MissingPostId} — проверка 404 и пустого тела.");
        var missingPostResponse = await postsApi.GetPostByIdAsync(testData.MissingPostId);

        missingPostResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        missingPostResponse.RawContent.Trim().Should().Be(testData.MissingPostEmptyBody);
        TestLogger.LogInfo("Шаг 3 пройден.");

        TestLogger.LogStep("Шаг 4: POST /posts — проверка 201 и совпадения данных.");
        var createRequest = new PostCreateRequest
        {
            UserId = testData.CreatePostUserId,
            Title = RandomDataGenerator.CreateRandomTitle(),
            Body = RandomDataGenerator.CreateRandomBody()
        };

        var createPostResponse = await postsApi.CreatePostAsync(createRequest);

        createPostResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createPostResponse.Body.Should().NotBeNull();
        createPostResponse.Body!.Title.Should().Be(createRequest.Title);
        createPostResponse.Body.Body.Should().Be(createRequest.Body);
        createPostResponse.Body.UserId.Should().Be(createRequest.UserId);
        createPostResponse.Body.Id.Should().BeGreaterThan(0);
        TestLogger.LogInfo($"Шаг 4 пройден. Id созданного поста: {createPostResponse.Body.Id}.");

        TestLogger.LogStep("Шаг 5: GET /users — проверка 200 и эталонного пользователя в списке.");
        var expectedUser = TestDataProvider.LoadExpectedUser(testData.ExpectedUserFile);
        var allUsersResponse = await usersApi.GetAllUsersAsync();

        allUsersResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        allUsersResponse.Body.Should().NotBeNull();

        var userFromStep5 = allUsersResponse.Body!
            .SingleOrDefault(user => user.Id == testData.ExistingUserId);

        userFromStep5.Should().NotBeNull();
        userFromStep5.Should().BeEquivalentTo(expectedUser);
        TestLogger.LogInfo($"Шаг 5 пройден. Пользователь id={testData.ExistingUserId} совпадает с эталоном.");

        TestLogger.LogStep($"Шаг 6: GET /users/{testData.ExistingUserId} — проверка 200 и данных из шага 5.");
        var userByIdResponse = await usersApi.GetUserByIdAsync(testData.ExistingUserId);

        userByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        userByIdResponse.Body.Should().NotBeNull();
        userByIdResponse.Body.Should().BeEquivalentTo(userFromStep5);
        TestLogger.LogInfo("Шаг 6 пройден.");

        TestLogger.LogInfo("Сценарий JsonPlaceholder API успешно завершён.");
    }
}