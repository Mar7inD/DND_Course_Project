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

    public async Task<bool> isValidWaste(string candidateWasteType, string wasteProcessingFacility)
    {
        var wasteProcessingFacilities = await _databaseService.ReadDB();

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
                            if(facilityValue.Type == JTokenType.Null)
                            {
                                // Can reuse to send also the value in future
                                throw new Exception("Waste type and processing facility missmatch.");
                            }
                            else 
                            {
                                return true;
                            }
                        }
                        
                    }
                }
            }
            throw new Exception("Waste type not found.");
        }
        return false;
    }

    public List<string> GetWasteTypes() {
        return AllowedWasteTypes.ToList();
    }
}
