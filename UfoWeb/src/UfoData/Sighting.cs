namespace UfoData;

public class Sighting
{
    public DateTimeOffset Time { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Shape { get; set; }
    public string? Duration { get; set; }
    public string? Summary { get; set; }
    public DateTimeOffset Posted { get; set; }
    public string? Images { get; set; }
}