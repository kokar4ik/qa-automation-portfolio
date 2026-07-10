namespace YandexDiskApi.Utils;

public static class DiskPathUtils
{
    public static string Normalize(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Требуется путь.", nameof(path));
        }

        return path.StartsWith('/') ? path : $"/{path}";
    }

    public static string GetFileName(string remotePath) =>
        Path.GetFileName(Normalize(remotePath));
}
