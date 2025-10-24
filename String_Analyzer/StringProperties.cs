namespace String_Analyzer
{
    public class StringProperties
    {
        public int length { get; set; }
        public bool is_palindrome { get; set; }
        public int unique_characters { get; set; }
        public int word_count { get; set; }
        public string sha256_hash { get; set; } = string.Empty;
        public Dictionary<string, int> character_frequency_map { get; set; } = new();
    }
}