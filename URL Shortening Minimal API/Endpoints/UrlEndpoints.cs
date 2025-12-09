using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using URL_Shortening_Minimal_API.DataAccess.Data;
using URL_Shortening_Minimal_API.DataAccess.Entities;


namespace URL_Shortening_Minimal_API.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapGet("/urls", GetAllUrls);
        app.MapGet("/urls/{id:int}", GetUrlById);
        app.MapPost("/urls", CreateUrl);
        app.MapPut("/urls/{shortUrl}", UpdateUrl);
        app.MapDelete("/urls/{shortUrl}", DeleteUrl);
        app.MapGet("/urls/short/{shortUrl}", GetUrlByShortUrl); // Changed route to avoid conflict
        app.MapGet("/urls/short/{shortUrl}/stats", GetUrlStats);
    }

    private static async Task<IResult> GetUrlStats([FromRoute] string shortUrl, URLDbContext db)
    {
        var url = await db.URLs.FirstOrDefaultAsync(x => x.ShortenedUrl == shortUrl);
        if (url is null) return Results.NotFound();
        await db.SaveChangesAsync();
        var urlWOStats = new URL()
        {
            Id = url.Id,
            CreatedAt = url.CreatedAt,
            ShortenedUrl = url.ShortenedUrl,
            updatedAt = url.updatedAt,
            OriginalUrl = url.OriginalUrl,
            AccessCount = url.AccessCount,
        };
        return Results.Ok(urlWOStats);
    }

    private static async Task<IResult> GetUrlByShortUrl([FromRoute] string shortUrl, URLDbContext db)
    {
        var url = await db.URLs.FirstOrDefaultAsync(x => x.ShortenedUrl == shortUrl);
        if (url is null) return Results.NotFound();
        url.AccessCount = url.AccessCount + 1;
        await db.SaveChangesAsync();
        var urlWOStats = new UrlWOStatistics() { 
            Id=url.Id,
            CreatedAt = url.CreatedAt,
            ShortenedUrl = url.ShortenedUrl,
            updatedAt = url.updatedAt,
            OriginalUrl = url.OriginalUrl,
        };
        return Results.Ok(urlWOStats);
    }

    private static async Task<IResult> GetAllUrls(URLDbContext db)
    {
        var urls = await db.URLs.ToListAsync();
        return Results.Ok(urls);
    }

    private static async Task<IResult> GetUrlById(int id, URLDbContext db)
    {
        var url = await db.URLs.FindAsync(id);
        return url is not null ? Results.Ok(url) : Results.NotFound();
    }

    private static async Task<IResult> CreateUrl(UrlDto url, URLDbContext db)
    {
        if (string.IsNullOrWhiteSpace(url.OriginalUrl) ||
        !Uri.TryCreate(url.OriginalUrl, UriKind.Absolute, out _))
        {
            return Results.BadRequest("Lütfen geçerli bir URL formatı giriniz (örn: https://google.com).");
        }
        string shorturl;
        do
        {
            shorturl = GenerateShortCode(url.OriginalUrl);
        }
        while (await db.URLs.AnyAsync(x=>x.ShortenedUrl == shorturl));
        var urlDb = new URL() { 
            OriginalUrl = url.OriginalUrl,
            ShortenedUrl = shorturl,
            CreatedAt = DateTime.UtcNow,
            AccessCount = 0,
        };
        db.URLs.Add(urlDb);
        await db.SaveChangesAsync();
        return Results.Created($"/urls/{urlDb.Id}", urlDb);
    }
    private static string GenerateShortCode(string url) 
    {
        Random rnd = new Random();
        var shortcode =new StringBuilder();
        for (int i = 0; i < 6; i++)
        {
            int asciiNumber = rnd.Next(32, 127);
            char asciiChar = (char)asciiNumber;
            shortcode.Append(asciiChar.ToString());
        }
        return shortcode.ToString();

    }
    private static async Task<IResult> UpdateUrl([FromQuery]string shortUrl, UrlDto updatedUrl, URLDbContext db)
    {
        var url = await db.URLs.FirstOrDefaultAsync(x=>x.ShortenedUrl == shortUrl);
        if (url is null) return Results.NotFound();
        url.OriginalUrl = updatedUrl.OriginalUrl;
        url.updatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUrl([FromQuery] string shortUrl, URLDbContext db)
    {
        var url = await db.URLs.FirstOrDefaultAsync(x => x.ShortenedUrl == shortUrl);
        if (url is null) return Results.NotFound();
        db.URLs.Remove(url);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}

