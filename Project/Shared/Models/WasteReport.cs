using Newtonsoft.Json;

namespace Shared.Models;

public class WasteReport
{
    public int Id { get; set; }
    public string WasteType { get; set; } = string.Empty;
    public string WasteProcessingFacility { get; set; } = string.Empty;
    public double WasteAmount { get; set; }
    public DateTime WasteDate { get; set; } = DateTime.Now;
    public int? WasteCollectorId { get; set; }
    public bool IsActive { get; set; } = true;

    public double? Co2Emission { get; set; }
}