using JsonPlaceholderApiTests.Utils;

namespace JsonPlaceholderApiTests.Tests;

public class JsonPlaceholderApiTests : TestBase
{
    [Test]
    public async Task JsonPlaceholderScenarioSixSteps()
    {
        await JsonPlaceholderScenarioRunner.RunSixStepsAsync(PostsApi, UsersApi, TestData);
    }
}
