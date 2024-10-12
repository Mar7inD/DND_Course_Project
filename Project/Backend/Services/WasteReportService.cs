using Backend.Models;
using Newtonsoft.Json.Linq;

namespace Backend.Services;

public class WasteReportService() {

    private string filePath = Path.Combine("Database", "WasteReport.json");
    
    public void CheckDB() {
        // Check if database directory exists
        if (!Directory.Exists("Database"))
        {
            throw new DirectoryNotFoundException("Database directory not found.");
        }
    }

    public async Task<JArray> ReadDB() {

        // Create the file content variable
        var wasteReports = new JArray();

        try
        {    
            CheckDB();

            // Read the file
            var jsonContent = await File.ReadAllTextAsync(filePath);

            // Check if file is empty
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                wasteReports = new JArray();
            }
            else
            {
                wasteReports = JArray.Parse(jsonContent);
            }
        }
        catch (DirectoryNotFoundException e)
        {
            throw new Exception("Database read error occurred.", e);
        }

        return wasteReports;
    }

    public async Task<bool> AddWasteReport(WasteReport wasteReport) {

        try 
        {
            // Read the file
            var wasteReports = await ReadDB();

            // Add the new waste report to the list
            wasteReports.Add(JObject.FromObject(wasteReport));
            
            // Write the new list to the file
            await File.WriteAllTextAsync(filePath, wasteReports.ToString());
        }
        catch (DirectoryNotFoundException e) 
        {
            throw new Exception("Database update error occurred.", e);
        }

        return true;
    }

    public async Task<string> GetAllWasteReports() {
        JArray wasteReports = await ReadDB();
        return wasteReports.ToString();
    }

    public async Task<WasteReport?> GetWasteReportById(int id) {
        JArray wasteReports = await ReadDB();
        
        var wasteReportToken = wasteReports
            .FirstOrDefault(report => report["id"]?.Value<int?>() == id);

        var wasteReport = wasteReportToken?.ToObject<WasteReport>();
        return wasteReport;
    }

    public async Task<IEnumerable<WasteReport>> GetWasteReportByType(string type) {
        JArray wasteReports = await ReadDB();
        
        var matchingReports = wasteReports
            .Where(report => report["wasteType"]?.Value<string>() == type)
            .Select(report => report.ToObject<WasteReport>())
            .Where(report => report != null);

        return matchingReports!;
    }
}