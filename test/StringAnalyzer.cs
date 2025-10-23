using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using String_Analyzer;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace test
{
    public class StringsApiTests : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
    
        public StringsApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
            using (var scope = factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }
    
        public void Dispose()
        {
            _factory.Dispose();
        }
        [Fact]
        public async Task Post_Then_Get_String()
        {
            // 1. POST the string
            var payload = new { value = "racecar" };
            var postResponse = await _client.PostAsJsonAsync("/api/strings", payload);
            postResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            // 2. Get the ID from the response
            var createdString = await postResponse.Content.ReadFromJsonAsync<ResponseModel>();
            var stringId = createdString.Id;

            // 3. GET the string using the ID
            var getResponse = await _client.GetAsync($"/api/strings/{stringId}");
            getResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            // 4. Assert the response
            var fetchedString = await getResponse.Content.ReadFromJsonAsync<ResponseModel>();
            Assert.Equal("racecar", fetchedString.Value);
            Assert.True(fetchedString.Properties.IsPalindrome);
        }

        [Fact]
        public async Task Get_Strings_With_Filters()
        {
            // 1. Add some strings
            await _client.PostAsJsonAsync("/api/strings", new { value = "level" });
            await _client.PostAsJsonAsync("/api/strings", new { value = "hello" });
            await _client.PostAsJsonAsync("/api/strings", new { value = "world" });
            await _client.PostAsJsonAsync("/api/strings", new { value = "stats" });

            // 2. Filter by is_palindrome=true
            var response = await _client.GetAsync("/api/strings?is_palindrome=true");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<FilterResponseModel>();
            Assert.Equal(2, json.Count);
            Assert.Contains(json.Data, s => s.Value == "level");
            Assert.Contains(json.Data, s => s.Value == "stats");
        }

        [Fact]
        public async Task Get_Strings_With_Natural_Language_Filter()
        {
            // 1. Add some strings
            await _client.PostAsJsonAsync("/api/strings", new { value = "madam" });
            await _client.PostAsJsonAsync("/api/strings", new { value = "test" });
            await _client.PostAsJsonAsync("/api/strings", new { value = "A man a plan a canal Panama" });

            // 2. Filter by natural language query
            var response = await _client.GetAsync("/api/strings/filter-by-natural-language?query=palindromic%20strings");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadFromJsonAsync<FilterResponseModel>();
            Assert.Equal(2, json.Count);
            Assert.Contains(json.Data, s => s.Value == "madam");
            Assert.Contains(json.Data, s => s.Value == "A man a plan a canal Panama");
        }
    }

    public class ResponseModel
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public Properties Properties { get; set; }
        public string Created_At { get; set; }
    }

    public class Properties
    {
        public int Length { get; set; }
        public bool IsPalindrome { get; set; }
        public int UniqueCharacters { get; set; }
        public int WordCount { get; set; }
        public string Sha256Hash { get; set; } = string.Empty;
        public Dictionary<string, int> CharacterFrequencyMap { get; set; } = new();
    }

    public class FilterResponseModel
    {
        public List<ResponseModel> Data { get; set; }
        public int Count { get; set; }
        public object Filters_Applied { get; set; }
    }

}
