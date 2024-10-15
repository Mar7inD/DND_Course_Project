using Backend.Services;
using Newtonsoft.Json.Linq;
namespace Backend.Models;

public class WasteTypes {

    private DatabaseService _databaseService;
    private static readonly HashSet<string> AllowedWasteTypes = new HashSet<string>
    {
        "Organic",
        "Plastic",
        "Metal",
        "Wood",
        "Paper",
        "Electronic",
        "Inceneration"
    };

    public WasteTypes()
    {
        _databaseService = new DatabaseService("Database/WasteProcessing.json");
    }

    public async Task<double> isValidWasteReturnCo2Emissions(string wasteType, string wasteProcessingFacility, double wasteAmount)
    {
        double calculatedEmission = 0.0;

        double co2emissionIndex = await GetWasteTypesByFacilityIndex(wasteType, wasteProcessingFacility);
        
        calculatedEmission = co2emissionIndex * wasteAmount;

        return calculatedEmission; 
    }

    public List<string> GetWasteTypes() {
        return AllowedWasteTypes.ToList();
    }

    // Compare and return CO2 emission index
    public async Task<double> GetWasteTypesByFacilityIndex(string candidateWasteType, string wasteProcessingFacility) {
        var wasteProcessingFacilities = await _databaseService.ReadDBAsync();

        foreach (JObject wasteObject in wasteProcessingFacilities)
        {   
            foreach (var wasteType in wasteObject.Properties())
            {
                string wasteTypeName = wasteType.Name;
                JObject facilities = (JObject)wasteType.Value;

                if (wasteTypeName.Equals(candidateWasteType, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var facility in facilities.Properties())
                    {
                        string facilityName = facility.Name;
                        JToken facilityValue = facility.Value;

                        if (facilityName.Equals(wasteProcessingFacility, StringComparison.OrdinalIgnoreCase))
                        {   
                            if (facilityValue == null || facilityValue.Type == JTokenType.Null)
                            {
                                throw new Exception("Waste type and processing facility mismatch.");
                            }
                            else 
                            {
                                return facilityValue.ToObject<double>();
                            }
                        }
                    }   
                }
            }
        }
        throw new Exception("Waste type not found.");
    }
}
