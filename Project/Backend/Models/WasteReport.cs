using Backend.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Models;

public class WasteReport {    
    [JsonIgnore]
    public int Id { get; private set; }

    public string WasteType { get; set; } = string.Empty;
    public string WasteProcessingFacility { get; set; } = string.Empty;
    public double WasteAmount { get; set; }
    private DateTime _wasteDate = DateTime.Now;
    public int? WasteCollectorId { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime WasteDate {
        get {
            return new DateTime(_wasteDate.Year, _wasteDate.Month, _wasteDate.Day, _wasteDate.Hour, _wasteDate.Minute, 0);
        }
        set {
            _wasteDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        }
    }

    public double? Co2Emission {
        get {
            return _cachedCo2Emission;
        }
        set {
            if (_cachedCo2Emission == null) {
                _cachedCo2Emission = value;
            } else {
                throw new InvalidOperationException("CO2 emission can only be set once.");
            }
        }
    }

    private double? _cachedCo2Emission;
    private readonly DatabaseService _databaseService;

    public WasteReport() { 
        _databaseService = new DatabaseService("Database/WasteReport.json");
        var wasteReports = _databaseService.ReadDBAsync().Result;

        if (wasteReports is JArray array && array.Any()) {
            Id = wasteReports.Max(wr => (int)(wr["Id"] ?? 0)) + 1;
        } else {
            Id = 1;
        }
    }
}