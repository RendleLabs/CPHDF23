using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace UfoData;

public class DataHerbClient
{
    private readonly IParser _parser;
    private readonly ILogger<DataHerbClient> _logger;
    private const string CsvUri = "https://raw.githubusercontent.com/RendleLabs/nuforc-ufo-records/master/dataset/nuforc_ufo_records.csv";
    private readonly HttpClient _httpClient = new();

    public DataHerbClient(IParser parser, ILogger<DataHerbClient> logger)
    {
        _parser = parser;
        _logger = logger;
    }

    public async Task<IList<Sighting>> GetSightings(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await _httpClient.GetAsync(CsvUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        stopwatch.Stop();
        _logger.LogInformation("Downloaded CSV in {Seconds} seconds", stopwatch.Elapsed.TotalSeconds);
        
        var sightings = await _parser.Parse(stream);
        return sightings;
    }
}
