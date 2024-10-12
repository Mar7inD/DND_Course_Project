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
    private readonly WasteTypes _wasteTypes;

    public WasteReportsController(ILogger<WasteReportsController> logger)
    {
        _logger = logger;
        _wasteReportService = new WasteReportService();
        _wasteTypes = new WasteTypes();
    }

    // Get all waste reports or by type
    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string? type)
    {
        // Check if type is specified and retrieve waste reports by type
        if (type != null)
        {
            return Ok(await _wasteReportService.GetWasteReportByType(type));
        }

        return Ok(await _wasteReportService.GetAllWasteReports());
    }

    // Get waste report by id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WasteReport>> Get(int id)
    {
        return Ok(await _wasteReportService.GetWasteReportById(id));
    }

    [HttpPost]
    public async Task<IActionResult> PostWasteReport([FromBody] WasteReport wasteReport)
    {
        if (!_wasteTypes.IsValidWasteType(wasteReport.wasteType))
        {
            return BadRequest("Invalid waste type");
        }
        
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

