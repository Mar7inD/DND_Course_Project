using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly ILogger<PeopleController> _logger;

    private PersonService _peopleService = new PersonService();

    public PeopleController(ILogger<PeopleController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IPerson>> Get([FromQuery] string? role)
    {   
        if(role != null) 
        {
            return Ok(await _peopleService.GetPeopleByRole(role));
        }

        return Ok(await _peopleService.GetPeople());
    }

    [HttpPost]
    public async Task<IActionResult> PostPerson([FromBody] IPerson person)
    {
        var result = await _peopleService.PostPerson(person);
        return result ? Ok() : BadRequest();
    }
}
