using System.Threading.Tasks;
using CW10_S31415.Exceptions;
using CW10_S31415.Services;
using Microsoft.AspNetCore.Mvc;

namespace CW10_S31415.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ClientsController(IDbService dbService): ControllerBase
{
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteClient(int id)
    {
        try
        {
            await dbService.DeleteClientAsync(id);
            return NoContent();
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