using Microsoft.EntityFrameworkCore;
using URL_Shortening_Minimal_API.DataAccess.Data;
using URL_Shortening_Minimal_API.DataAccess.Entities;


namespace URL_Shortening_Minimal_API.Endpoints;

public static class UrlEndpoints
{
    public static void MapUrlEndpoints(this WebApplication app)
    {
        app.MapGet("/urls", GetAllUrls);
        app.MapGet("/urls/{id}", GetUrlById);
        app.MapPost("/urls", CreateUrl);
        app.MapPut("/urls/{id}", UpdateUrl);
        app.MapDelete("/urls/{id}", DeleteUrl);
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

    private static async Task<IResult> CreateUrl(URL url, URLDbContext db)
    {
        db.URLs.Add(url);
        await db.SaveChangesAsync();
        return Results.Created($"/urls/{url.Id}", url);
    }

    private static async Task<IResult> UpdateUrl(int id, URL updatedUrl, URLDbContext db)
    {
        var url = await db.URLs.FindAsync(id);
        if (url is null) return Results.NotFound();
        url.OriginalUrl = updatedUrl.OriginalUrl;
        url.ShortenedUrl = updatedUrl.ShortenedUrl;
        url.updatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteUrl(int id, URLDbContext db)
    {
        var url = await db.URLs.FindAsync(id);
        if (url is null) return Results.NotFound();
        db.URLs.Remove(url);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}

