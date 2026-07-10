using System.Text;

namespace YandexDiskApi.Utils;

public static class TextUtils
{
    public static string RemoveControlCharacters(string text) =>
        new string(text.Where(character => !char.IsControl(character)).ToArray());

    public static string NormalizeViewerText(string rawText)
    {
        var text = RemoveControlCharacters(rawText.Trim());
        text = StripWrappingQuotes(text);
        text = text.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        if (LooksLikeBase64(text))
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(PadBase64(text)));
            }
            catch (FormatException)
            {
                return StripWrappingQuotes(rawText.Trim());
            }
        }

        return StripWrappingQuotes(rawText.Trim());
    }

    public static string StripWrappingQuotes(string value)
    {
        if (value.Length >= 2
            && ((value.StartsWith('"') && value.EndsWith('"'))
                || (value.StartsWith('\'') && value.EndsWith('\''))))
        {
            return value[1..^1].Trim();
        }

        return value;
    }

    private static bool LooksLikeBase64(string value) =>
        value.Length >= 4
        && value.All(character => char.IsLetterOrDigit(character) || character is '+' or '/' or '=');

    private static string PadBase64(string value)
    {
        var padding = (4 - value.Length % 4) % 4;
        return value + new string('=', padding);
    }
}
