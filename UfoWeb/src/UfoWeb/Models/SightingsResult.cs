using UfoData;

namespace UfoWeb.Models;

public class SightingsResult
{
    public Sighting[] Sightings { get; set; }
    public int Page { get; set; }
}