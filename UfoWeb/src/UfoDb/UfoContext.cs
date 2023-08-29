using Microsoft.EntityFrameworkCore;

namespace UfoDb;

public class UfoContext : DbContext
{
    public UfoContext(DbContextOptions<UfoContext> options) : base(options)
    {
        
    }
    
    public DbSet<DbSighting> Sightings { get; internal set; }
}