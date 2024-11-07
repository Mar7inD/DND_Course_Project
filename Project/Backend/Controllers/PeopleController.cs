using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Backend.Services;
using BCrypt.Net;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeopleController : ControllerBase
{
    private readonly ILogger<PeopleController> _logger;
    private readonly IConfiguration _config;
    private PersonService _peopleService = new PersonService();

    public PeopleController(ILogger<PeopleController> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    // Get person by role or status
    [HttpGet]
    public async Task<ActionResult<List<PersonBase>>> Get([FromQuery] string? role, string? active)
    {   
        return Ok(await _peopleService.GetPeople(role, active));
    }

    // Get person by employeeId
    [HttpGet("{employeeId:int}")]
    public async Task<ActionResult<PersonBase>> GetPersonById(string employeeId)
    {
        var people = await _peopleService.GetPeople(employeeId: employeeId, active: "true");
        var foundPerson = people.FirstOrDefault();

        if (foundPerson == null)
        {
            return NotFound("User not found.");
        }

        return Ok(foundPerson);
    }

    [HttpPost("register")]
    public async Task<IActionResult> PostPerson([FromBody] IPerson person)
    {        
        var result = await _peopleService.PostPerson(person);
        
        if (result == "Success")
        {
            // Generate JWT token directly within this method
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = jwtToken });
        }

        return BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] IPerson person)
    {
        var people = await _peopleService.GetPeople(employeeId: person.EmployeeId, active: "true");
        var foundPerson = people.FirstOrDefault();

        if (foundPerson == null)
        {
            return Unauthorized("Invalid employee ID or inactive account.");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(person.Password, foundPerson.Password);
        if (!isPasswordValid)
        {
            return Unauthorized("Invalid password.");
        }

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

    [HttpPut("{employeeId:int}")]
    public async Task<IActionResult> PutPerson([FromBody] IPersonDTO person, int employeeId)
    {
        var result = await _peopleService.PutPerson(employeeId, person);
        return result == "Success" ? Ok() : BadRequest(result);
    }
}
