using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.Services;
using System.Threading.Tasks;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WasteReportsController : ControllerBase
{
    private readonly ILogger<WasteReportsController> _logger;
    private readonly WasteReportService _wasteReportService;

    private readonly Co2CalculatorService _co2CalculatorService;

    public WasteReportsController(ILogger<WasteReportsController> logger)
    {
        _logger = logger;
        _wasteReportService = new WasteReportService();
        _co2CalculatorService = new Co2CalculatorService();
    }

    // Get all waste reports or by type
    [HttpGet]
    public async Task<ActionResult<List<WasteReport>>> Get([FromQuery] string? type)
    {
        try
        {
            // Check if type is specified and retrieve waste reports by type
            if (type != null)
            {
                var filteredReports = await _wasteReportService.GetWasteReportByType(type);
                return Ok(filteredReports);
            }

            // Retrieve all waste reports
            var allReports = await _wasteReportService.GetAllWasteReports();
            return Ok(allReports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while retrieving waste reports");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    // Get waste report by id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WasteReport>> Get(int id)
    {
        return Ok(await _wasteReportService.GetWasteReportById(id));
    }

    [HttpGet("co2emission")]
    public async Task<ActionResult<double>> GetCo2EmissionTotal([FromQuery] DateOnly startDate, [FromQuery] DateOnly? endDate, [FromQuery] int? userId, [FromQuery] string? type)
    {
        var effectiveEndDate = endDate ?? DateOnly.FromDateTime(DateTime.Now);
        return Ok(await _co2CalculatorService.GetCo2EmissionTotal(startDate, effectiveEndDate, userId, type));
    }

    [HttpGet("co2emission/{id:int}")]
    public async Task<ActionResult<double>> GetCo2EmissionForReport(int id)
    {
        return Ok(await _co2CalculatorService.GetCo2EmissionForReport(id));
    }

    [HttpPost]
    public async Task<IActionResult> PostWasteReport([FromBody] WasteReport wasteReport)
    {
        try 
        {
            await _wasteReportService.PostWasteReport(wasteReport);
            return Ok();
        }
        catch (System.Exception e) {
            _logger.LogError(e, "Failed to post waste report");
            return BadRequest(e.Message);
        }
    }
}

