using System.Threading.Tasks;
using CW10_S31415.DTOs;
using CW10_S31415.Exceptions;
using CW10_S31415.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW10_S31415.Controllers;
[ApiController]
[Route("api/[controller]")]
public class TripsController(IDbService dbService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int? page,[FromQuery] int pageSize = 10)
    {
        if (page == null)
        {
            var result = await dbService.GetTripsAsync();
            return Ok(result);
        }
        else
        {
            var result = await dbService.GetTripsPagedAsync((int)page, pageSize);
            return Ok(result);
        }
    }

    [HttpPost("{id}/clients")]
    public async Task<IActionResult> AddTripToClient([FromRoute] int id,[FromBody] ClientTripCreateDto body)
    {
        try
        {
            await dbService.AddClientToTripAsync(id, body);
            return Ok();
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}

