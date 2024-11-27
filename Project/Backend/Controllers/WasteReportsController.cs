using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WasteReportsController(ILogger<WasteReportsController> _logger, WasteReportService _wasteReportService) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<WasteReport>>> Get([FromQuery] string? type, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        try
        {
            List<WasteReport> reports;

            if (type != null)
            {
                reports = await _wasteReportService.GetWasteReportByType(type);
            }
            else
            {
                reports = await _wasteReportService.GetAllWasteReports();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                reports = reports.Where(r => r.WasteDate >= startDate.Value && r.WasteDate <= endDate.Value.AddDays(1)).ToList();
            }

            return Ok(reports);
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
