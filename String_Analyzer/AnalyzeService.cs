using System.Text;
using System.Text.RegularExpressions;

namespace String_Analyzer
{
    public class AnalyzeService
    {
        public StringProperties AnalyzeString(string input)
        {
            var properties = new StringProperties
            {
                length = input.Length,
                is_palindrome = IsPalindrome(input),
                unique_characters = input.Distinct().Count(),
                word_count = CountWords(input),
                sha256_hash = ComputeSha256Hash(input),
                character_frequency_map = ComputeCharacterFrequency(input)
            };
            return properties;
        }
        public bool IsPalindrome(string input)
        {
            if(string.IsNullOrWhiteSpace(input))
                return false;

            // Remove all non-alphanumeric characters
            var cleaned = Regex.Replace(input, "[^a-zA-Z0-9]", "");

            // Reverse the cleaned string
            var reversed = new string(cleaned.Reverse().ToArray());

            // Compare ignoring case
            return string.Equals(cleaned, reversed, StringComparison.OrdinalIgnoreCase);
        }
        public int CountWords(string input)
        {
            if(string.IsNullOrEmpty(input)) return 0;
            return input.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        public string ComputeSha256Hash(string raw)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create(); ;
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
        public Dictionary<string, int> ComputeCharacterFrequency(string input)
        {
            var frequencyMap = new Dictionary<string, int>();
            foreach (var ch in input)
            {
                var key = ch.ToString();
                if (frequencyMap.ContainsKey(key))
                {
                    frequencyMap[key]++;
                }
                else
                {
                    frequencyMap[key] = 1;
                }
            }
            return frequencyMap;
        }
    }
}
