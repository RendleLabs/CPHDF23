using Microsoft.AspNetCore.Mvc;
using UfoData;
using UfoWeb.Models;

namespace UfoWeb.Controllers;

public class SightingsController : Controller
{
    private const int PageSize = 100;
    private readonly DataHerbClient _dataHerbClient;

    public SightingsController(DataHerbClient dataHerbClient)
    {
        _dataHerbClient = dataHerbClient;
    }

    // GET
    public async Task<IActionResult> Index([FromQuery]int page, CancellationToken cancellationToken)
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
        return View(new SightingsViewModel(sightings, page));
    }
}