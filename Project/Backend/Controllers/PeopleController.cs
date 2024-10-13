using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Diagnostics.Eventing.Reader;

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
    public async Task<ActionResult<IEnumerable<IPerson>>> Get([FromQuery] string? role, string? active)
    {   
        var people = await _peopleService.GetPeople();

        if(role != null && active != null)
        {
            
            return  Ok(people
                    .Where(p => p.role == role && p.isActive == bool.Parse(active)));
        }
        else if(role != null) 
        {
            return Ok(people
                    .Where(p => p.role == role).ToList());
        }
        else if(active != null)
        {
            return Ok(people
                    .Where(p => p.isActive == bool.Parse(active))
                    .ToList());
        }
        

        return Ok(people);
    }

    [HttpPost]
    public async Task<IActionResult> PostPerson([FromBody] IPerson person)
    {
        var result = await _peopleService.PostPerson(person);
        return result == "Success" ? Ok() : BadRequest(result);
    }

    [HttpPut("{employeeId:int}")]
    public async Task<IActionResult> PutPerson([FromBody] IPersonDTO person, int employeeId)
    {
        var result = await _peopleService.PutPerson(employeeId, person);
        return result == "Success" ? Ok() : BadRequest(result);
    }
}
