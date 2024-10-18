using Microsoft.AspNetCore.Mvc;
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
    public async Task<ActionResult<List<PersonBase>>> Get([FromQuery] string? role, string? active)
    {   
        return Ok(await _peopleService.GetPeople(role, active));
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
