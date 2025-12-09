Here's the improved `README.md` file, incorporating the new content while maintaining the existing structure and information:

# URL Shortening Minimal API

A minimal C# Web API (Minimal API style) for creating short URLs backed by EF Core and SQLite/your configured database. The project targets .NET 8 and exposes simple CRUD endpoints to create, read, update, delete, and inspect short URLs.

## Features

- Create shortened URLs from long/original URLs
- Retrieve all shortened URLs or a single URL by id
- Lookup by short code (increments access count)
- Inspect URL statistics (access count, timestamps)
- Persist data using EF Core (migrations included)

## Requirements

- .NET 8 SDK or later
- A database provider supported by EF Core (project includes EF Core migrations). By default, the project uses the configured provider in `appsettings.json`.

## Installation

1. Clone the repository:

    ```sh
    git clone https://github.com/erencaglar/URL-Shortening-Minimal-API.git
    ```

2. Restore and build:

    ```sh
    dotnet restore
    dotnet build
    ```

3. Apply EF Core migrations to create the database:

    ```sh
    dotnet ef database update --project "URL Shortening Minimal API"
    ```

    (If `dotnet ef` is not available, install the EF Core tools: `dotnet tool install --global dotnet-ef`.)

## Running

Run the application from the solution folder:

dotnet run --project "URL Shortening Minimal API"

By default, the app will listen on the ports configured in `Properties/launchSettings.json` or the ASPNETCORE URLs environment variables. Use `http://localhost:<port>` shown in the console to call the endpoints.

## API Endpoints

- `GET /urls` — Returns all URL entries.
- `GET /urls/{id:int}` — Returns a URL by numeric database id.
- `POST /urls` — Create a new short URL. Body: `{ "originalUrl": "https://example.com" }`.
- `PUT /urls/{shortUrl}` — Update the original URL of an existing short code. Pass the updated DTO in the JSON body: `{ "originalUrl": "https://new.example.com" }`. (The implementation reads `shortUrl` as a query parameter; include `?shortUrl=<code>` if a route param fails.)
- `DELETE /urls/{shortUrl}` — Delete the mapping for the given short code. (Implementation expects a query parameter `shortUrl`.)
- `GET /urls/short/{shortUrl}` — Lookup the original URL by short code. This increments the access counter and returns the mapping (without statistics).
- `GET /urls/short/{shortUrl}/stats` — Returns the URL entry including statistics (`AccessCount`, `CreatedAt`, `updatedAt`).

## Project Structure

URL Shortening Minimal API/
├── Program.cs                    # App entry and DI registration
├── Endpoints/UrlEndpoints.cs     # Minimal API endpoint handlers
├── DataAccess/Entities/URL.cs    # Entity model
├── DataAccess/Entities/UrlDto.cs # DTOs
├── DataAccess/Data/URLDbContext.cs# EF Core DbContext and configuration
├── DataAccess/Migrations/        # EF Core migrations
├── appsettings.json              # Configuration (connection strings, etc.)
└── Properties/launchSettings.json

## Reference

Repository: [https://github.com/erencaglar/URL-Shortening-Minimal-API](https://github.com/erencaglar/URL-Shortening-Minimal-API)