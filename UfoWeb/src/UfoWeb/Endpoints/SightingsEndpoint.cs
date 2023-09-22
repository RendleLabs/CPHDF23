using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using UfoData;
using UfoDb;
using UfoWeb.Models;

namespace UfoWeb.Endpoints;

public class SightingsEndpoint
{
    private const int PageSize = 100;
    private readonly UfoContext _context;

    public SightingsEndpoint(UfoContext context)
    {
        _context = context;
    }

    public async Task<SightingsResult> Get(int page, CancellationToken cancellationToken)
    {
        try
        {
            DbSighting[] dbSightings;
            if (page > 0)
            {
                dbSightings = await _context.Sightings
                    .OrderBy(s => s.Id)    
                    .Skip(PageSize * page)
                    .Take(PageSize)
                    .ToArrayAsync(cancellationToken: cancellationToken);
            }
            else
            {
                dbSightings = await _context.Sightings
                    .OrderBy(s => s.Id)    
                    .Take(PageSize)
                    .ToArrayAsync(cancellationToken: cancellationToken);
            }

            return new SightingsResult
            {
                Sightings = dbSightings.Select(d => new Sighting
                {
                    City = d.City,
                    Country = d.Country,
                    Duration = d.Duration,
                    Images = d.Images,
                    Shape = d.Shape,
                    State = d.State,
                    Posted = d.Posted,
                    Time = d.Time
                }).ToArray(),
                Page = page
            };
        }
        catch (Exception ex)
        {
            if (Activity.Current is { } activity)
            {
                activity.AddEvent(new ActivityEvent("Exception", tags: new ActivityTagsCollection
                {
                    { "error.type", ex.GetType().Name },
                    { "error.message", ex.Message },
                    { "error.trace", ex.StackTrace }
                }));
            }

            throw;
        }
    }
}