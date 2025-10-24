using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace String_Analyzer.Controllers
{
    [ApiController]
    [Route("strings")]
    public class StringsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AnalyzeService _analyzeService;

        public StringsController(AppDbContext db, AnalyzeService analyzeService)
        {
            _db = db;
            _analyzeService = analyzeService;
        }

        // 1️⃣ POST /strings
        [HttpPost]
        public async Task<IActionResult> AddString([FromBody] JsonElement body)
        {
            // Validate 'value' field
            if (!body.TryGetProperty("value", out var valProp))
                return BadRequest(new { error = "Invalid request body or missing \"value\" field" });

            if (valProp.ValueKind != JsonValueKind.String)
                return UnprocessableEntity(new { error = "Invalid data type for \"value\" (must be string)" });

            var value = valProp.GetString()?.Trim() ?? string.Empty;
            var props = _analyzeService.AnalyzeString(value);

            // Use sha256 as ID
            var id = props.sha256_hash;

            // Check if already exists
            var existing = await _db.Strings.FindAsync(id);
            if (existing != null)
                return Conflict(new { error = "String already exists in the system" });

            var entry = new StringEntry
            {
                Id = id,
                Value = value,
                Properties = props,
                CreatedAt = DateTime.UtcNow
            };

            _db.Strings.Add(entry);
            await _db.SaveChangesAsync();

            var response = new
            {
                id = $"{entry.Id}_{entry.Value}",
                value = entry.Value,
                properties = entry.Properties,
                created_at = entry.CreatedAt.ToString("o")
            };

            return Created($"/strings/{entry.Id}", response);
        }


        // 2️⃣ GET /strings/{string_value}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetString(string id)
        {
            var entry = await _db.Strings.FindAsync(id);

            if (entry == null)
                return NotFound(new { error = " String does not exist in the system" });

            var response = new
            {
                id = $"{entry.Id}_{entry.Value}",
                value = entry.Value,
                properties = entry.Properties,
                created_at = entry.CreatedAt.ToString("o")
            };

            return Ok(response);
        }


        // 3️⃣ GET /strings with filters
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] bool? is_palindrome,
            [FromQuery] int? min_length,
            [FromQuery] int? max_length,
            [FromQuery] int? word_count,
            [FromQuery(Name = "contains_character")] string? containsChar)
        {
            var query = _db.Strings.AsEnumerable();
            var filters_applied = new Dictionary<string, object>();

            if (is_palindrome.HasValue)
            {
                query = query.Where(x => x.Properties.is_palindrome == is_palindrome.Value);
                filters_applied["is_palindrome"] = is_palindrome.Value;
            }

            if (min_length.HasValue)
            {
                query = query.Where(x => x.Properties.length >= min_length.Value);
                filters_applied["min_length"] = min_length.Value;
            }

            if (max_length.HasValue)
            {
                query = query.Where(x => x.Properties.length <= max_length.Value);
                filters_applied["max_length"] = max_length.Value;
            }

            if (word_count.HasValue)
            {
                query = query.Where(x => x.Properties.word_count == word_count.Value);
                filters_applied["word_count"] = word_count.Value;
            }

            if (!string.IsNullOrWhiteSpace(containsChar))
            {
                query = query.Where(x => x.Value.Contains(containsChar));
                filters_applied["contains_character"] = containsChar;
            }

            var results = query.ToList();

            var response = new
            {
                data = results.Select(x => new
                {
                    id = x.Id,
                    value = x.Value,
                    properties = x.Properties,
                    created_at = x.CreatedAt.ToString("o")
                }),
                count = results.Count,
                filters_applied
            };

            return Ok(response);
        }


        // 4️⃣ GET /strings/filter-by-natural-language
        [HttpGet("filter-by-natural-language")]
        public IActionResult FilterByNaturalLanguage([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { error = "Query is required" });

            var (success, filters, error) = NaturalLanguageParser.Parse(query);

            if (!success)
                return UnprocessableEntity(new { error });
            var q = _db.Strings.AsEnumerable();

            // Apply filters
            if (filters.TryGetValue("is_palindrome", out var palVal) && palVal is bool palBool)
                q = q.Where(x => x.Properties.is_palindrome == palBool);

            if (filters.TryGetValue("min_length", out var minVal) && int.TryParse(minVal.ToString(), out var min))
                q = q.Where(x => x.Properties.length >= min);

            if (filters.TryGetValue("max_length", out var maxVal) && int.TryParse(maxVal.ToString(), out var max))
                q = q.Where(x => x.Properties.length <= max);

            if (filters.TryGetValue("word_count", out var wordVal) && int.TryParse(wordVal.ToString(), out var wc))
                q = q.Where(x => x.Properties.word_count == wc);

            if (filters.TryGetValue("contains_character", out var charVal) &&
                charVal is string ch && !string.IsNullOrEmpty(ch))
                q = q.Where(x => x.Value.Contains(ch, StringComparison.OrdinalIgnoreCase));

            var results = q.ToList();

            // Return structured response
            return Ok(new
            {
                count = results.Count,
                interpreted_query = new
                {
                    original = query,
                    parsed_filters = filters
                },
                data = results.Select(x => new
                {
                    id = x.Id,
                    value = x.Value,
                    properties = x.Properties,
                    created_at = x.CreatedAt.ToString("o")
                })
            });
        }



        // 5️⃣ DELETE /strings/{string_value}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteString(string id)
        {
            var entry = await _db.Strings.FindAsync(id);

            if (entry == null)
                return NotFound(new { error = "String not found" });

            _db.Strings.Remove(entry);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}