using Newtonsoft.Json.Linq;
using Backend.Models;
using Backend.Services;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace Backend.Services;

public class Co2CalculatorService {
    private readonly DatabaseService _databaseService;
    private readonly WasteTypes _wasteTypes;

    public Co2CalculatorService()
    {
        _databaseService = new DatabaseService("Database/WasteReports.json");
        _wasteTypes = new WasteTypes();
    }

    public double CalculateCo2Emission(List<JToken> wasteReports) {
        
        var co2EmissionTotal = new List<double>();

        foreach (var wasteType in _wasteTypes.GetWasteTypes())
        {   
            double totalTons = 0.0;
            
            
            foreach (var wasteReport in wasteReports)
            {
                if (wasteReport[wasteType]!.Value<string>() == wasteType)
                {
                    totalTons += wasteReport["wasteAmount"]!.Value<double>();
                    
                }

                switch (wasteType)
                {
                    case "Organic":
                        co2EmissionTotal.Add(totalTons * 0.5);
                        break;
                    case "Plastic":
                        co2EmissionTotal.Add(totalTons * 6);
                        break;
                    case "Metal":
                        co2EmissionTotal.Add(totalTons * 5);
                        break;
                    case "Wood":
                        co2EmissionTotal.Add(totalTons * 1.5);
                        break;
                    case "Paper":
                        co2EmissionTotal.Add(totalTons * 2);
                        break;
                    case "Electronic":
                        co2EmissionTotal.Add(totalTons * 10);
                        break;
                    case "Inceneration":
                        co2EmissionTotal.Add(totalTons * 3);
                        break;
                }
            }

            
        }

        return co2EmissionTotal.Sum();
    }

    public async Task<double> GetCo2EmissionTotalByPeriod(DateOnly startDate, DateOnly endDate) {
        var wasteReports = (await _databaseService.ReadDB()).ToList();
        var totalCo2Emission = 0.0;

        wasteReports = wasteReports.Where(wr => wr["wasteDate"] != null && wr["wasteDate"]?.Value<DateOnly>() >= startDate && wr["wasteDate"]?.Value<DateOnly>() <= endDate).ToList();

            totalCo2Emission = CalculateCo2Emission(wasteReports);

        return 2;
    }
}