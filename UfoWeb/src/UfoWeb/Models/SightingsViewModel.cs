using UfoData;

namespace UfoWeb.Models;

public class SightingsViewModel
{
    public SightingsViewModel(IList<Sighting> sightings, int page)
    {
        Sightings = sightings;
        Page = page;
    }

    public IList<Sighting> Sightings { get; }
    public int Page { get; }
}