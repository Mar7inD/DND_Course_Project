using Backend.Models;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Services
{
    /// <summary>
    public class WasteReportService
    {
        private readonly string filePath;
        private readonly DatabaseService _databaseService;
        private readonly WasteTypes _wasteTypes;
        
        public WasteReportService()
        {
            filePath = Path.Combine("Database", "WasteReport.json");
            _databaseService = new DatabaseService(filePath);
            _wasteTypes = new WasteTypes();
        }

        public async Task<string> PostWasteReport(WasteReport wasteReport)
        {
            var wasteReports = await _databaseService.ReadDB();

            // Check if waste report with the same id already exists
            if (wasteReports.Any(report => report["id"]?.Value<int>() == wasteReport.id))
            {
                throw new System.Exception("Waste report with the same id already exists");
            }

            // Check if waste type is valid and return CO2 emissions if valid
            double co2Emissions = await _wasteTypes.isValidWasteReturnCo2Emissions
                                (wasteReport.wasteType, wasteReport.wasteProcessingFacility, wasteReport.wasteAmount);

            wasteReport.co2Emission = co2Emissions;

            wasteReports.Add(JObject.FromObject(wasteReport));
            await _databaseService.WriteDB(wasteReports);

            return "Success";
        }

        public async Task<string> GetAllWasteReports()
        {
            JArray wasteReports = await _databaseService.ReadDB();
            return wasteReports.ToString();
        }

        public async Task<WasteReport?> GetWasteReportById(int id)
        {
            JArray wasteReports = await _databaseService.ReadDB();
            var wasteReportToken = wasteReports
                .FirstOrDefault(report => report["id"]?.Value<int?>() == id);
            var wasteReport = wasteReportToken?.ToObject<WasteReport>();
            return wasteReport;
        }

        public async Task<IEnumerable<WasteReport>> GetWasteReportByType(string type)
        {
            JArray wasteReports = await _databaseService.ReadDB();
            var matchingReports = wasteReports
                .Where(report => report["wasteType"]?.Value<string>() == type)
                .Select(report => report.ToObject<WasteReport>())
                .Where(report => report != null);
            return matchingReports!;
        }
    }
}