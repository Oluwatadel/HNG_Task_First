using System.Text.RegularExpressions;

namespace String_Analyzer
{
    //public class NaturalLanguageParseResult
    //{
    //    public bool Success { get; set; }
    //    public string Original { get; set; } = string.Empty;
    //    public Dictionary<string, object> ParsedFilters { get; set; } = new();
    //    public string? Error { get; set; }
    //}

    public static class NaturalLanguageParser
    {
        // Heuristic parser: covers the example queries listed in the task.
        public static (bool Success, Dictionary<string, object> ParsedFilters, string? Error) Parse(string query)
        {
            var filters = new Dictionary<string, object>();
            var lower = query.ToLowerInvariant();

                    if (lower.Contains("palindrome") || lower.Contains("palindromic"))
                        filters["is_palindrome"] = true;            if (lower.Contains("not palindrome"))
                filters["is_palindrome"] = false;

            var lenMatch = Regex.Match(lower, @"min(imum)? length (\d+)");
            if (lenMatch.Success)
                filters["min_length"] = int.Parse(lenMatch.Groups[2].Value);

            var maxMatch = Regex.Match(lower, @"max(imum)? length (\d+)");
            if (maxMatch.Success)
                filters["max_length"] = int.Parse(maxMatch.Groups[2].Value);

            var wordCountMatch = Regex.Match(lower, @"(\d+) words?");
            if (wordCountMatch.Success)
                filters["word_count"] = int.Parse(wordCountMatch.Groups[1].Value);

            var charMatch = Regex.Match(lower, @"containing '?([a-z])'?");
            if (charMatch.Success)
                filters["contains_character"] = charMatch.Groups[1].Value;

            return (filters.Count > 0, filters, filters.Count > 0 ? null : "No filters found");
        }
    }
}