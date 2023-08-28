using System.Diagnostics;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace UfoData;

public partial class DataHerbClient
{
    private static readonly Action<ILogger, double, Exception?> LogDownloadTime =
        LoggerMessage.Define<double>(LogLevel.Information,
            1, "Downloaded CSV in {Seconds} seconds");

    private const string CacheKey = "DataHerbClient.Sightings";
    
    private readonly IParser _parser;
    private readonly IDistributedCache _cache;
    private readonly ILogger<DataHerbClient> _logger;
    private const string CsvUri = "https://raw.githubusercontent.com/RendleLabs/nuforc-ufo-records/master/dataset/nuforc_ufo_records.csv";
    private readonly HttpClient _httpClient = new();

    public DataHerbClient(IParser parser, IDistributedCache cache, ILogger<DataHerbClient> logger)
    {
        _parser = parser;
        _cache = cache;
        _logger = logger;

    }

    public async Task<IList<Sighting>> GetSightings(CancellationToken cancellationToken = default)
    {
        var stopwatch = ValueStopwatch.StartNew();
        
        var bytes = await _cache.GetAsync(CacheKey, cancellationToken);
        if (bytes is { Length: > 0 })
        {
            var cached = MessagePackSerializer.Deserialize<SightingCacheItem>(bytes, cancellationToken: cancellationToken);
            LogServedFromCache(stopwatch.Stop().TotalSeconds);
            return cached.Sightings;
        }
        
        var response = await _httpClient.GetAsync(CsvUri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var elapsedTime = stopwatch.Stop();

        LogDownloadTime(_logger, elapsedTime.TotalSeconds, null);

        var sightings = (await _parser.Parse(stream)).ToArray();

        var cacheItem = new SightingCacheItem
        {
            Sightings = sightings,
            Timestamp = DateTimeOffset.UtcNow
        };
        bytes = MessagePackSerializer.Serialize(cacheItem, cancellationToken: cancellationToken);
        await _cache.SetAsync(CacheKey, bytes, cancellationToken);

        return sightings;
    }

    [LoggerMessage(2, LogLevel.Information, "Served Sightings from Cache in {Seconds} seconds")]
    partial void LogServedFromCache(double seconds);
}

public readonly struct ValueStopwatch
{
    private readonly long _start;

    public ValueStopwatch(long start)
    {
        _start = start;
    }

    public static ValueStopwatch StartNew() => new ValueStopwatch(Stopwatch.GetTimestamp());

    public TimeSpan Stop() => Stopwatch.GetElapsedTime(_start);
}
