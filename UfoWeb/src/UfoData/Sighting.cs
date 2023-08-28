using MessagePack;

namespace UfoData;

[MessagePackObject]
public class SightingCacheItem
{
    [Key(0)]
    public Sighting[] Sightings { get; set; }
    
    [Key(1)]
    public DateTimeOffset Timestamp { get; set; }
}

[MessagePackObject]
public class Sighting
{
    [Key(0)]
    public DateTimeOffset Time { get; set; }
    [Key(1)]
    public string? City { get; set; }
    [Key(2)]
    public string? State { get; set; }
    [Key(3)]
    public string? Country { get; set; }
    [Key(4)]
    public string? Shape { get; set; }
    [Key(5)]
    public string? Duration { get; set; }
    [Key(6)]
    public string? Summary { get; set; }
    [Key(7)]
    public DateTimeOffset Posted { get; set; }
    [Key(8)]
    public string? Images { get; set; }
}