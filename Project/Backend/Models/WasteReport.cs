namespace Backend.Models;

public class WasteReport {
    public int id { get; set; }
    public string wasteType { get; set; } = string.Empty;
    public double wasteAmount { get; set; }
    private DateTime _wasteDate = DateTime.Now;
    public string? wasteCollector { get; set; }

    public DateTime wasteDate {
        get {
            return new DateTime(_wasteDate.Year, _wasteDate.Month, _wasteDate.Day, _wasteDate.Hour, _wasteDate.Minute, 0);
        }
        set {
            _wasteDate = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0);
        }
    }
}
