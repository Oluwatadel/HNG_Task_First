using System.ComponentModel.DataAnnotations;

namespace String_Analyzer
{
    public class StringEntry
    {
        [Key]
        public string Id { get; set; } = string.Empty; // sha256 hash

        [Required]
        public string Value { get; set; } = string.Empty;

        public StringProperties Properties { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
