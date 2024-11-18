using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Shared.Models;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WasteReportsController : ControllerBase
{
    private readonly ILogger<WasteReportsController> _logger;
    private readonly WasteReportService _wasteReportService;
    private readonly Co2CalculatorService _co2CalculatorService;

    public WasteReportsController(
        ILogger<WasteReportsController> logger,
        WasteReportService wasteReportService,
        Co2CalculatorService co2CalculatorService)
    {
        _logger = logger;
        _wasteReportService = wasteReportService;
        _co2CalculatorService = co2CalculatorService;
    }

    [HttpGet]
    public async Task<ActionResult<List<WasteReport>>> Get([FromQuery] string? type)
    {
        try
        {
            if (type != null)
            {
                var filteredReports = await _wasteReportService.GetWasteReportByType(type);
                return Ok(filteredReports);
            }

            var allReports = await _wasteReportService.GetAllWasteReports();
            return Ok(allReports);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving waste reports");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WasteReport>> Get(int id)
    {
        var report = await _wasteReportService.GetWasteReportById(id);
        if (report == null)
        {
            return NotFound("Waste report not found");
        }
        return Ok(report);
    }

    [HttpPost]
    public async Task<IActionResult> PostWasteReport([FromBody] WasteReport wasteReport)
    {
        try
        {
            await _wasteReportService.PostWasteReport(wasteReport);
            return Ok("Waste report created successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create waste report");
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutWasteReport(int id, [FromBody] WasteReport wasteReport)
    {
        try
        {
            if (id != wasteReport.Id)
            {
                return BadRequest("Waste report ID mismatch.");
            }

            await _wasteReportService.PutWasteReport(id, wasteReport);
            return Ok("Waste report updated successfully.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Waste report not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update waste report");
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWasteReport(int id)
    {
        try
        {
            await _wasteReportService.DeleteWasteReport(id);
            return Ok("Waste report deleted successfully.");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Waste report not found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete waste report");
            return BadRequest(ex.Message);
        }
    }
}
