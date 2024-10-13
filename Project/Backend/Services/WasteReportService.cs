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

            
            if(!await _wasteTypes.isValidWaste(wasteReport.wasteType, wasteReport.wasteProcessingFacility))
            {
                throw new System.Exception("Invalid waste type, processing facility or potential mismatch between the two.");
            }
            

            if (wasteReports.Any(report => report["id"]?.Value<int>() == wasteReport.id))
            {
                throw new System.Exception("Waste report with the same id already exists");
            }
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