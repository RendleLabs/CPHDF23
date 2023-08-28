using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace UfoData;

public partial class StringParser : IParser
{
    private const int Time = 0;
    private const int City = 1;
    private const int State = 2;
    private const int Country = 3;
    private const int Shape = 4;
    private const int Duration = 5;
    private const int Summary = 6;
    private const int Posted = 7;
    private const int Images = 8;

    private readonly ILogger<StringParser> _logger;
    private readonly DateParser _dateParser = new();

    public StringParser(ILogger<StringParser> logger)
    {
        _logger = logger;
    }

    public async Task<IList<Sighting>> Parse(Stream stream)
    {
        var stopwatch = ValueStopwatch.StartNew();
        
        var list = new List<Sighting>();
        using var reader = new StreamReader(stream);
        await reader.ReadLineAsync(); // Skip header
        for (var line = await reader.ReadLineAsync(); line is { Length: > 0 }; line = await reader.ReadLineAsync())
        {
            var sighting = ParseLine(line);
            list.Add(sighting);
        }
        
        var elapsed = stopwatch.Stop();
        
        LogParseTime(elapsed.TotalSeconds);

        return list;
    }

    private Sighting ParseLine(string line)
    {
        var builder = new StringBuilder();
        var fields = new string[9];
        int f = 0;
        bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            switch (line[i])
            {
                case '"':
                    inQuotes = !inQuotes;
                    break;
                case ',' when !inQuotes:
                    fields[f] = builder.ToString();
                    builder.Clear();
                    ++f;
                    break;
                default:
                    builder.Append(line[i]);
                    break;
            }
        }

        var sighting = new Sighting
        {
            Time = _dateParser.Parse(fields[Time]),
            City = fields[City],
            State = fields[State],
            Country = fields[Country],
            Shape = fields[Shape],
            Duration = fields[Duration],
            Summary = fields[Summary],
            Posted = _dateParser.Parse(fields[Posted]),
            Images = fields[Images]
        };

        return sighting;
    }

    [LoggerMessage(1, LogLevel.Information, "Parsed CSV in {Seconds} seconds")]
    partial void LogParseTime(double seconds);

}