using Microsoft.Extensions.Configuration;

namespace JsonPlaceholderApiTests.Utils;

public static class ConfigurationExtensions
{
    public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName)
        where T : class
    {
        return configuration.GetSection(sectionName).Get<T>()
            ?? throw new InvalidOperationException(
                $"Секция '{sectionName}' отсутствует или не удалось десериализовать в {typeof(T).Name}.");
    }
}
