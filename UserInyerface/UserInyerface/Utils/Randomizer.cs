using System.Text;

namespace UserInyerface.Utils
{
    public static class Randomizer
    {
        private const string LowerCaseLatin = "abcdefghijklmnopqrstuvwxyz";
        private const string UpperCaseLatin = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const int MinPasswordLength = 10;

        private static readonly Random Random = new();

        public static string GenerateRandomString(int length)
        {
            return new string(Enumerable.Repeat(LowerCaseLatin, length)
                .Select(characters => characters[Random.Next(characters.Length)])
                .ToArray());
        }

        public static string GenerateValidPassword(char requiredEmailChar)
        {
            var upperChar = UpperCaseLatin[Random.Next(UpperCaseLatin.Length)];
            var digitChar = Digits[Random.Next(Digits.Length)];
            var passwordBuilder = new StringBuilder();
            passwordBuilder.Append(upperChar);
            passwordBuilder.Append(digitChar);
            passwordBuilder.Append(requiredEmailChar);

            while (passwordBuilder.Length < MinPasswordLength)
            {
                passwordBuilder.Append(LowerCaseLatin[Random.Next(LowerCaseLatin.Length)]);
            }

            return new string(passwordBuilder.ToString().ToCharArray().OrderBy(_ => Random.Next()).ToArray());
        }
    }
}