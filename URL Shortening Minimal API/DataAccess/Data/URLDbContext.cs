using Microsoft.EntityFrameworkCore;
using URL_Shortening_Minimal_API.DataAccess.Entities;

namespace URL_Shortening_Minimal_API.DataAccess.Data;
public class URLDbContext : DbContext
{
    public URLDbContext(DbContextOptions<URLDbContext> options) : base(options)
    {
    }
    public DbSet<URL> URLs { get; set; }
}
