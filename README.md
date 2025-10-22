# 🧠 String Analyzer API
### Backend Wizards — Stage 1 Task

A RESTful API service that analyzes strings and stores their computed properties using **.NET 9 (C#)** and **SQLite + Entity Framework Core**.

---

## 🚀 Features

For each analyzed string, the API computes and stores:

| Property | Description |
|-----------|--------------|
| `length` | Number of characters in the string |
| `is_palindrome` | Whether the string reads the same backward and forward (case-insensitive) |
| `unique_characters` | Number of distinct characters |
| `word_count` | Number of words (split by whitespace) |
| `sha256_hash` | Unique SHA-256 hash of the string |
| `character_frequency_map` | Dictionary showing the frequency of each character |

---

## 🧩 Tech Stack

- **Language:** C# (.NET 9)
- **Database:** SQLite (via Entity Framework Core)
- **ORM:** EF Core
- **Hosting:** Any of Railway, Render, AWS, Heroku, PXXL, etc.
- **Testing:** xUnit + WebApplicationFactory
- **Serialization:** System.Text.Json

---

## ⚙️ Setup Instructions

### 1️⃣ Clone the Repository

```bash
git clone https://github.com/<your-username>/String_Analyzer.git
cd String_Analyzer


2️⃣ Configure Database Connection

In the root of the project, there’s an appsettings.json file:

{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=stringanalyzer.db"
  }
}


No external DB setup required — SQLite runs locally.

3️⃣ Install Dependencies

Make sure you have the .NET 9 SDK installed, then run:

dotnet restore

4️⃣ Apply Database Migrations
dotnet ef database update


(If migrations folder doesn’t exist yet, create one using
dotnet ef migrations add InitialCreate)

5️⃣ Run the API
dotnet run --project String_Analyzer


By default it runs on:

https://localhost:5001

🧪 Testing

Go to the test project folder and run:

dotnet test


This uses xUnit + Microsoft.AspNetCore.Mvc.Testing for integration tests.

🌍 API Endpoints
1️⃣ Create/Analyze String

POST /strings

Request:

{
  "value": "string to analyze"
}


Response (201 Created):

{
  "id": "sha256_hash_value",
  "value": "string to analyze",
  "properties": {
    "length": 16,
    "is_palindrome": false,
    "unique_characters": 12,
    "word_count": 3,
    "sha256_hash": "abc123...",
    "character_frequency_map": {
      "s": 2,
      "t": 3
    }
  },
  "created_at": "2025-08-27T10:00:00Z"
}


Errors:

400 Bad Request: Missing or invalid body

409 Conflict: String already exists

422 Unprocessable Entity: Invalid data type for "value"

2️⃣ Get Specific String

GET /strings/{string_value}

Response:

{
  "id": "sha256_hash_value",
  "value": "requested string",
  "properties": { /* computed values */ },
  "created_at": "2025-08-27T10:00:00Z"
}


Errors:

404 Not Found: String does not exist

3️⃣ Get All Strings (Filterable)

GET /strings?is_palindrome=true&min_length=5&max_length=20&word_count=2&contains_character=a

Response:

{
  "data": [
    {
      "id": "hash1",
      "value": "madam level",
      "properties": { /* ... */ },
      "created_at": "2025-08-27T10:00:00Z"
    }
  ],
  "count": 15,
  "filters_applied": {
    "is_palindrome": true,
    "min_length": 5,
    "max_length": 20,
    "word_count": 2,
    "contains_character": "a"
  }
}

4️⃣ Natural Language Filtering

GET /strings/filter-by-natural-language?query=all%20single%20word%20palindromic%20strings

Response:

{
  "data": [ /* matching strings */ ],
  "count": 3,
  "interpreted_query": {
    "original": "all single word palindromic strings",
    "parsed_filters": {
      "word_count": 1,
      "is_palindrome": true
    }
  }
}


Example Queries Supported:

all single word palindromic strings

strings longer than 10 characters

palindromic strings that contain the first vowel

strings containing the letter z

Errors:

400 Bad Request: Unable to parse query

422 Unprocessable Entity: Conflicting filters

5️⃣ Delete String

DELETE /strings/{string_value}

Response: 204 No Content

Errors:

404 Not Found: String not in database

🧠 Example JSON Output
{
  "id": "b9c9dcd29db8c51a...",
  "value": "madam",
  "properties": {
    "length": 5,
    "is_palindrome": true,
    "unique_characters": 3,
    "word_count": 1,
    "sha256_hash": "b9c9dcd29db8c51a...",
    "character_frequency_map": {
      "m": 2,
      "a": 2,
      "d": 1
    }
  },
  "created_at": "2025-10-22T18:45:00Z"
}

🧰 Environment Variables
Variable	Description	Default
ASPNETCORE_ENVIRONMENT	Hosting environment	Development
ConnectionStrings__DefaultConnection	Database path	Data Source=stringanalyzer.db
🧪 Example Test
[Fact]
public async Task Post_Then_Get_String()
{
    var payload = new { value = "racecar" };
    await _client.PostAsJsonAsync("/strings", payload);

    var get = await _client.GetAsync("/strings/racecar");
    var json = await get.Content.ReadFromJsonAsync<ResponseModel>();

    Assert.Equal("racecar", json.Value);
    Assert.True(json.Properties.Is_Palindrome);
}
