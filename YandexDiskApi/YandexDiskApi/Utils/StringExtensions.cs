namespace YandexDiskApi.Utils;

public static class StringExtensions
{
    public static string ToQuote(this string value) => $"«{value}»";
}
