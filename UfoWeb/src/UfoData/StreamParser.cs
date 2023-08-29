using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Microsoft.Extensions.Logging;

namespace UfoData;

public partial class StreamParser : IParser
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

    private readonly ILogger<StreamParser> _logger;
    private readonly DateParser _dateParser = new();

    public StreamParser(ILogger<StreamParser> logger)
    {
        _logger = logger;
    }

    public async Task<IList<Sighting>> Parse(Stream stream)
    {
        var stopwatch = ValueStopwatch.StartNew();
        
        var list = new List<Sighting>();

        var pipe = PipeReader.Create(stream);
        bool isFirst = true;

        while (true)
        {
            var result = await pipe.ReadAsync();
            var position = Read(result.Buffer, ref isFirst, result.IsCompleted, list);
            
            if (result.IsCompleted) break;
            
            pipe.AdvanceTo(position, result.Buffer.End);
        }

        await pipe.CompleteAsync();

        return list;
    }

    private SequencePosition Read(ReadOnlySequence<byte> buffer, ref bool isFirst, bool isCompleted, List<Sighting> list)
    {
        const byte newline = (byte)'\n';
        
        var reader = new SequenceReader<byte>(buffer);

        while (!reader.End)
        {
            if (reader.TryReadTo(out ReadOnlySpan<byte> bytes, newline))
            {
                if (isFirst)
                {
                    // Skip the header line
                    isFirst = false;
                    continue;
                }
                list.Add(ParseLine(bytes));
            }
            else if (isCompleted)
            {
                var slice = buffer.Slice(reader.Position);
                if (slice.IsSingleSegment)
                {
                    list.Add(ParseLine(slice.FirstSpan));
                }
                else
                {
                    var justOneArray = new byte[slice.Length];
                    slice.CopyTo(justOneArray);
                    list.Add(ParseLine(justOneArray));
                }

                break;
            }
            else
            {
                break;
            }
        }

        return reader.Position;
    }

    private Sighting ParseLine(ReadOnlySpan<byte> span)
    {
        var sighting = new Sighting
        {
            Time = _dateParser.Parse(ReadNext(ref span)),
            City = ReadNext(ref span),
            State = ReadNext(ref span),
            Country = ReadNext(ref span),
            Shape = ReadNext(ref span),
            Duration = ReadNext(ref span),
            Summary = ReadNext(ref span),
            Posted = _dateParser.Parse(ReadNext(ref span)),
            Images = ReadNext(ref span),
        };

        return sighting;
    }

    private static string? ReadNext(ref ReadOnlySpan<byte> span)
    {
        if (span.Length == 0) return null;
        
        const byte comma = (byte)',';
        const byte doubleQuote = (byte)'"';

        bool inQuotes = false;

        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == doubleQuote)
            {
                inQuotes = !inQuotes;
            }
            else if (span[i] == comma && !inQuotes)
            {
                var str = Encoding.UTF8.GetString(span[..i].Trim(doubleQuote));
                span = span.Slice(i + 1);
                return str;
            }
        }

        return Encoding.UTF8.GetString(span.Trim(doubleQuote));
    }

    [LoggerMessage(1, LogLevel.Information, "Parsed CSV in {Seconds} seconds")]
    partial void LogParseTime(double seconds);

}