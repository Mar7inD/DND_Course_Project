using Newtonsoft.Json;

namespace Backend.Models
{
    public class WasteReport
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string WasteType { get; set; } = string.Empty;
        public string WasteProcessingFacility { get; set; } = string.Empty;
        public double WasteAmount { get; set; }
        public DateTime WasteDate { get; set; } = DateTime.Now;
        public int? WasteCollectorId { get; set; }
        public bool IsActive { get; set; } = true;

        public double? Co2Emission { get; set; }

        public WasteReport()
        {
            // Empty constructor
        }

        public WasteReport(int id, string wasteType, string facility, double amount, DateTime wasteDate, double? co2Emission)
        {
            Id = id;
            WasteType = wasteType;
            WasteProcessingFacility = facility;
            WasteAmount = amount;
            WasteDate = wasteDate;
            Co2Emission = co2Emission;
        }
    }
}
