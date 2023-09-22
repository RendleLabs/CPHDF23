using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UfoData;
using UfoDb;
using UfoWeb.Endpoints;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddDbContextPool<UfoContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlite("Data Source=Data/ufo.db");
    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddScoped<SightingsEndpoint>();

var app = builder.Build();

app.MapGet("/api/v2/sightings", 
    async ([FromQuery] int page,
        [FromServices] SightingsEndpoint endpoint,
    CancellationToken cancellationToken) =>
        Results.Ok(await endpoint.Get(page, cancellationToken)));

app.Run();

[JsonSerializable(typeof(SightingsResult))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}

public class SightingsResult
{
    public Sighting[] Sightings { get; set; }
    public int Page { get; set; }
}
