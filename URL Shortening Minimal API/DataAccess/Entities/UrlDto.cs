namespace URL_Shortening_Minimal_API.DataAccess.Entities
{
    public class UrlDto
    {
        public required string OriginalUrl { get; set; }
    }

    public class UrlWOStatistics:UrlDto
    {
        public int Id { get; set; }
        public required string ShortenedUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? updatedAt { get; set; }
    }
}
