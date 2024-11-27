using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Services;
using System.Security.Claims;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize (Roles = "Manager")]
public class PeopleController (
    ILogger<PeopleController> _logger, 
    PersonService _personService) : ControllerBase
{


    [HttpGet]
    public async Task<ActionResult<List<PersonBase>>> Get([FromQuery] string? role, [FromQuery] bool? active)
    {
        try
        {
            var people = await _personService.GetPeople(role, active);
            return Ok(people);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving people");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{employeeId}")]
    public async Task<ActionResult<PersonBase>> GetPersonById(string employeeId)
    {
        var person = await _personService.GetPersonById(employeeId);
        if (person == null)
        {
            return NotFound("Person not found.");
        }
        return Ok(person);
    }

    

    

    [HttpPut("{employeeId}")]
    public async Task<IActionResult> UpdatePerson(string employeeId, [FromBody] PersonBase updatedPerson)
    {
        try
        {
            if (employeeId != updatedPerson.EmployeeId)
            {
                return BadRequest("Employee ID mismatch.");
            }

            var result = await _personService.UpdatePerson(employeeId, updatedPerson);

            if (result == "Success")
            {
                return Ok("Person successfully updated.");
            }
            else
            {
                return NotFound(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update person");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeletePerson(string employeeId)
    {
        try
        {
            var result = await _personService.DeletePerson(employeeId);

            if (result == "Success")
            {
                return Ok("Person successfully deactivated.");
            }
            else
            {
                return NotFound(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to deactivate person");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

}
