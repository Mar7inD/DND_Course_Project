using Newtonsoft.Json.Linq;
using Backend.Models;
using Backend.Services;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Backend.Services;

public class Co2CalculatorService {
    private readonly DatabaseService _databaseService;

    public Co2CalculatorService()
    {
        _databaseService = new DatabaseService("Database/WasteReport.json");
    }

    public async Task<double> GetCo2EmissionTotal(DateOnly? startDate, DateOnly? endDate, int? userId, string? wasteType) {
        var wasteReports = (await _databaseService.ReadDBAsync()).ToList();
        double totalCo2Emission;

        // Filter reports 
        wasteReports = wasteReports.Where(wr => {
            var wasteDateToken = wr["wasteDate"];
            if (wasteDateToken != null) {
                var wasteDateTime = wasteDateToken.Value<DateTime>();
                var wasteDateOnly = DateOnly.FromDateTime(wasteDateTime);
                if (wasteDateOnly >= startDate && wasteDateOnly <= endDate) {
                    var userIdToken = wr["wasteCollectorId"];
                    var wasteTypeToken = wr["wasteType"];
                    bool userIdMatches = userId == null || (userIdToken != null && userIdToken.Value<int>() == userId);
                    bool wasteTypeMatches = wasteType == null || (wasteTypeToken != null && wasteTypeToken.Value<string>() == wasteType);
                    return userIdMatches && wasteTypeMatches;
                }
            }
            return false;
        }).ToList();

        // Sum all CO2 emissions
        totalCo2Emission = wasteReports.Sum(wr => wr["co2Emission"]!.Value<double>());

        return totalCo2Emission;
    }

    public async Task<double> GetCo2EmissionForReport(int id = 13) {
        var report = (await _databaseService.ReadDBAsync()).FirstOrDefault(wr => wr["id"]?.Value<int>() == id);

        return report?["co2Emission"]?.Value<double>() ?? 0.0;
    }
}