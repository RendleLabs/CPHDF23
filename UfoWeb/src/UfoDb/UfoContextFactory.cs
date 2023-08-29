using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UfoDb;

public class UfoContextFactory : IDesignTimeDbContextFactory<UfoContext>
{
    public UfoContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UfoContext>()
            .UseSqlite("Data Source=ufo.db")
            .Options;

        return new UfoContext(options);
    }
}