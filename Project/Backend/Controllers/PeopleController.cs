using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Backend.Services;
using Shared.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly ILogger<PeopleController> _logger;
    private readonly PersonService _personService;
    private readonly IConfiguration _config;

    public PeopleController(
        ILogger<PeopleController> logger,
        PersonService personService,
        IConfiguration config)
    {
        _logger = logger;
        _personService = personService;
        _config = config;
    }

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

    [HttpPost("register")]
    public async Task<IActionResult> RegisterPerson([FromBody] PersonBase person)
    {
        try
        {
            var result = await _personService.AddPerson(person);

            if (result == "Success" || result.Contains("re-registered"))
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register person");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] PersonBase person)
    {
        var foundPerson = await _personService.GetPersonById(person.EmployeeId);

        if (foundPerson == null || !foundPerson.IsActive)
        {
            return Unauthorized("Invalid employee ID or inactive account.");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(person.Password, foundPerson.Password);
        if (!isPasswordValid)
        {
            return Unauthorized("Invalid password.");
        }

        // Generate JWT
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            null,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        // Add Role to the response
        var response = new { Token = jwtToken, EmployeeId = foundPerson.EmployeeId, Role = foundPerson.Role };

        return Ok(response);
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
