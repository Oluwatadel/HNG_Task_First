using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace test
{
    public class StringsApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        public StringsApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Then_Get_String()
        {
            var payload = new { value = "racecar" };

            var get = await _client.GetAsync("/strings/racecar");
            Assert.Equal(HttpStatusCode.OK, get.StatusCode);

            var json = await get.Content.ReadFromJsonAsync<ResponseModel>();
            Assert.Equal("racecar", json.Value);
            Assert.True((bool)json.Properties.IsPalindrome);
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

}
