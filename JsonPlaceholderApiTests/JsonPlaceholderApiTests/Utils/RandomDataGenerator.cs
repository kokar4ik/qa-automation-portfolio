namespace JsonPlaceholderApiTests.Utils;

public static class RandomDataGenerator
{
    private const string Characters = "abcdefghijklmnopqrstuvwxyz";
    private static readonly Random Random = new();

    public static string CreateRandomTitle() => CreateRandomString();

    public static string CreateRandomBody() => CreateRandomString();

    private static string CreateRandomString(int length = 10)
    {
        return new string(Enumerable.Range(0, length)
            .Select(_ => Characters[Random.Next(Characters.Length)])
            .ToArray());
    }
}
