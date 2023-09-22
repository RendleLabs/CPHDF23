using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using UfoData;
using UfoDb;
using UfoWeb.Endpoints;
using UfoWeb.Startup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<DataHerbClient>();
builder.Services.AddTransient<IParser, StreamParser>();

OpenTelemetryStartup.Add(builder);

builder.Services.AddDbContextPool<UfoContext>(optionsBuilder =>
{
    optionsBuilder.UseSqlite("Data Source=Data/ufo.db");
    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

if (builder.Configuration.GetConnectionString("Redis") is { Length: > 0 } redisConnectionString)
{
    IConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConnectionString);
    builder.Services.AddSingleton(redis);
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.ConnectionMultiplexerFactory = () => Task.FromResult(redis);
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddScoped<SightingsEndpoint>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapGet("/api/v2/sightings", 
    async ([FromQuery] int page,
        [FromServices] SightingsEndpoint endpoint,
    CancellationToken cancellationToken) =>
        Results.Ok(await endpoint.Get(page, cancellationToken)));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();