using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Backend.Models;
using Backend.Services;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WasteReportsController : ControllerBase
{
    private readonly ILogger<WasteReportsController> _logger;

    private WasteReportService _wasteReportService = new WasteReportService();

    private WasteTypes _wasteTypes = new WasteTypes();

    public WasteReportsController(ILogger<WasteReportsController> logger)
    {
        _logger = logger;
    }

    //Get all waste reports or by type
    [HttpGet]
    public async Task<ActionResult<WasteReport>> Get([FromQuery] string type) {

        // Check if type is specified and retrieve waste reports by type
        if(type != null)
        {
            return Ok(await _wasteReportService.GetWasteReportByType(type));
        }

        return Ok(await _wasteReportService.GetAllWasteReports());
    }

    //Get waste report by id
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WasteReport>> Get(int id) {
        return Ok(await _wasteReportService.GetWasteReportById(id));
    }
    
    [HttpPost]
    public async Task<IActionResult> PostWasteReport([FromBody] WasteReport wasteReport) {

        if(!_wasteTypes.IsValidWasteType(wasteReport.wasteType))
        {
            return BadRequest("Invalid waste type");
        }

        var result = await _wasteReportService.AddWasteReport(wasteReport);
        return result ? Ok() : BadRequest();
    }
}

