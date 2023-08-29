using Microsoft.AspNetCore.Mvc;
using UfoData;
using UfoWeb.Models;

namespace UfoWeb.Controllers;

[Route("api/sightings")]
public class SightingsApiController : ControllerBase
{
    private const int PageSize = 100;
    private readonly DataHerbClient _dataHerbClient;

    public SightingsApiController(DataHerbClient dataHerbClient)
    {
        _dataHerbClient = dataHerbClient;
    }

    [HttpGet]
    public async Task<ActionResult<SightingsResult>> Index([FromQuery]int page, CancellationToken cancellationToken)
    {
        var sightings = await _dataHerbClient.GetSightings(cancellationToken);
        if (page > 0)
        {
            sightings = sightings.Skip(page * PageSize).Take(PageSize).ToList();
        }
        else
        {
            sightings = sightings.Take(PageSize).ToList();
        }

        return new SightingsResult
        {
            Sightings = sightings.ToArray(),
            Page = page
        };
    }
}