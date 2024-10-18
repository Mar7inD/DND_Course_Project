using Newtonsoft.Json.Linq;
namespace Shared.Models;

public class WasteTypes {

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

    public async Task<double> isValidWasteReturnCo2Emissions(string wasteType, string wasteProcessingFacility, double wasteAmount)
    {
        double co2emissionIndex = await GetWasteTypesByFacilityIndex(wasteType, wasteProcessingFacility);
        
        double calculatedEmission = co2emissionIndex * wasteAmount;

        return calculatedEmission; 
    }

    public List<string> GetWasteTypes() {
        return AllowedWasteTypes.ToList();
    }

    // Compare and return CO2 emission index
    public async Task<double> GetWasteTypesByFacilityIndex(string candidateWasteType, string wasteProcessingFacility) 
    {
        string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Storage", "WasteProcessing.json");
        string jsonData = await File.ReadAllTextAsync(jsonFilePath);
        var wasteProcessingFacilities = JArray.Parse(jsonData);

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
