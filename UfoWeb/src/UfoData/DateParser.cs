using System.Diagnostics;

namespace UfoData;

public class DateParser
{
    private static readonly DateTimeOffset DefaultDate = new(1900, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly int _currentYear = DateTimeOffset.UtcNow.Year - 2000;

    public DateTimeOffset Parse(string? str)
    {
        if (str is not { Length: > 0 }) return DefaultDate;
        
        DateTimeOffset date = DefaultDate;
        TimeSpan time = TimeSpan.Zero;

        var parts = str.Split(' ', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].Contains('/'))
            {
                date = ParseDate(parts[i]);
            }
            else if (parts[i].Contains(':'))
            {
                time = ParseTime(parts[i]);
            }
        }

        return date + time;
    }

    private DateTimeOffset ParseDate(string str)
    {
        try
        {
            var mdy = str.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        
            if (mdy.Length == 3)
            {
                if (int.TryParse(mdy[0], out var month) && int.TryParse(mdy[1], out var day) &&
                    int.TryParse(mdy[2], out var year))
                {
                    int century = year > _currentYear ? 1900 : 2000;

                    return new DateTimeOffset(year + century, month, day, 0, 0, 0, TimeSpan.Zero);
                }
            }
        }
        catch (Exception ex)
        {
            Activity.Current?.AddEvent(new ActivityEvent("DateParserError", tags: new ActivityTagsCollection
            {
                { "error.type", ex.GetType().Name },
                { "error.message", ex.Message }
            }));
        }

        return DefaultDate;
    }

    private TimeSpan ParseTime(string str)
    {
        var hms = str.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (hms.Length > 0 && int.TryParse(hms[0], out var h))
        {
            if (hms.Length > 1 && int.TryParse(hms[1], out var m))
            {
                if (hms.Length > 2 && int.TryParse(hms[2], out var s))
                {
                    return new TimeSpan(0, h, m, s);
                }

                return new TimeSpan(0, h, m, 0);
            }

            return new TimeSpan(0, h, 0, 0);
        }

        return TimeSpan.Zero;
    }
}