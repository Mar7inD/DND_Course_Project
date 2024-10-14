namespace Backend.Models;

public class WasteReport {
    public int id { get; set; }
    public string wasteType { get; set; } = string.Empty;
    public string wasteProcessingFacility { get; set; } = string.Empty;
    public double wasteAmount { get; set; }
    private DateTime _wasteDate = DateTime.Now;
    public int? wasteCollectorId { get; set; }
    public bool isActive { get; set; } = true;
    private double? _cachedCo2Emission;

    public DateTime wasteDate {
        get {
            return new DateTime(_wasteDate.Year, _wasteDate.Month, _wasteDate.Day, _wasteDate.Hour, _wasteDate.Minute, 0);
        }
        set {
            _wasteDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        }
    }

    public double? co2Emission {
        get {
            return _cachedCo2Emission;
        }
        set {
            if (_cachedCo2Emission == null) 
            {
                _cachedCo2Emission = value;
            }
            else
            {
                throw new System.Exception("CO2 emission can only be set once");
            }
        }
    }

}
