using NLog;

namespace JsonPlaceholderApiTests.Utils;

public static class TestLogger
{
    private static readonly Logger Logger = LogManager.GetLogger("JsonPlaceholderApiTests");

    public static void Initialize()
    {
        if (LogManager.Configuration is not null)
        {
            return;
        }

        var configPath = Path.Combine(AppContext.BaseDirectory, "NLog.config");

        if (File.Exists(configPath))
        {
            LogManager.Setup().LoadConfigurationFromFile(configPath);
        }

        Logger.Info($"Логирование инициализировано. Файл логов: {Path.Combine(AppContext.BaseDirectory, "logs", "test.log")}");
    }

    public static void Shutdown()
    {
        Logger.Info("Логирование завершено.");
        LogManager.Shutdown();
    }

    public static void LogInfo(string message) => Logger.Info(message);

    public static void LogStep(string stepDescription) => Logger.Info($"[ШАГ] {stepDescription}");
}
