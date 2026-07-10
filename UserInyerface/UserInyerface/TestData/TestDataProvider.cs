using System.Text.Json;

namespace UserInyerface.TestData
{
    public static class TestDataProvider
    {
        private static readonly string ResourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources");
        private static readonly string ImagesPath = Path.Combine(ResourcesPath, "Images");
        private static readonly string TestDataFilePath = Path.Combine(ResourcesPath, "testdata.json");

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static TestDataModel? cachedData;

        public static TestDataModel Data => cachedData ??= Load();

        public static string GetAvatarFilePath()
        {
            return Path.Combine(ImagesPath, Data.Registration.AvatarFileName);
        }

        private static TestDataModel Load()
        {
            var json = File.ReadAllText(TestDataFilePath);
            return JsonSerializer.Deserialize<TestDataModel>(json, JsonOptions)
                ?? throw new InvalidOperationException("Не удалось загрузить тестовые данные из testdata.json.");
        }
    }
}