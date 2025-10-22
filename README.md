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
