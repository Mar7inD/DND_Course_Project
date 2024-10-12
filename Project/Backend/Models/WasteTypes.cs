namespace Backend.Models;

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

    public bool IsValidWasteType(string wasteType)
    {
        return AllowedWasteTypes.Contains(wasteType);
    }
}
