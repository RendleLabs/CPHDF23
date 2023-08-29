using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using UfoData;
using UfoDb;
using UfoDbCreator;

if (!CommandLine.TryParse(args, out var dbPath, out var migrate))
{
    dbPath = Path.GetFullPath("./ufo.db");
}

var options = new DbContextOptionsBuilder<UfoContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

var context = new UfoContext(options);

if (migrate)
{
    Console.WriteLine("Creating database...");
    await context.Database.MigrateAsync();
}

var client = new DataHerbClient(new StreamParser(NullLogger<StreamParser>.Instance), new MemoryDistributedCache(
    new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions
    {
        ExpirationScanFrequency = TimeSpan.FromSeconds(5)
    })), NullLogger<DataHerbClient>.Instance);

Console.WriteLine("Getting sightings...");
var sightings = await client.GetSightings();

Console.WriteLine("Inserting sightings...");
var entities = sightings.Select(sighting => new DbSighting
{
    City = sighting.City,
    Country = sighting.Country,
    Duration = sighting.Duration,
    Images = sighting.Images,
    Posted = sighting.Posted.UtcDateTime,
    Shape = sighting.Shape,
    State = sighting.State,
    Summary = sighting.Summary,
    Time = sighting.Time.UtcDateTime
});

context.Sightings.AddRange(entities);
await context.SaveChangesAsync();

Console.WriteLine("Validating data...");

var count = await context.Sightings.CountAsync();

if (count == sightings.Count)
{
    Console.WriteLine("Data inserted successfully.");
}
else
{
    Console.WriteLine($"Data mismatch: downloaded {sightings.Count}, inserted {count}.");
}