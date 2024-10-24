using Newtonsoft.Json.Linq;


namespace Backend.Services
{
    /// <summary>
    public class WasteReportService
    {
        private readonly DatabaseService _databaseService;
        private readonly WasteTypes _wasteTypes;

        public WasteReportService()
        {
            _databaseService = new DatabaseService("Database/WasteReport.json");
            _wasteTypes = new WasteTypes();
        }

        public async Task<string> PostWasteReport(WasteReport wasteReport)
        {
            var wasteReports = await _databaseService.ReadDBAsync();

            // Assign new ID if it's not set
            if (wasteReport.Id == 0)
            {
                int newId = wasteReports.Any() ? wasteReports.Max(wr => (int)(wr["Id"] ?? 0)) + 1 : 1;
                wasteReport.Id = newId;
            }

            // Check if waste type is valid and return CO2 emissions if valid
            double co2Emissions = await _wasteTypes.isValidWasteReturnCo2Emissions(
                wasteReport.WasteType, wasteReport.WasteProcessingFacility, wasteReport.WasteAmount
            );
            wasteReport.Co2Emission = co2Emissions;

            wasteReports.Add(JObject.FromObject(wasteReport));
            await _databaseService.WriteDBAsync(wasteReports);

            return "Success";
        }

        public async Task<List<WasteReport>> GetAllWasteReports()
        {
            JArray wasteReportsArray = await _databaseService.ReadDBAsync();

            return wasteReportsArray.ToObject<List<WasteReport>>() ?? new List<WasteReport>();
        }

        public async Task<WasteReport?> GetWasteReportById(int id)
        {
            JArray wasteReports = await _databaseService.ReadDBAsync();
            var wasteReportToken = wasteReports.FirstOrDefault(report => report["Id"]?.Value<int>() == id);
            return wasteReportToken?.ToObject<WasteReport>();
        }

        public async Task<IEnumerable<WasteReport>> GetWasteReportByType(string type)
        {
            JArray wasteReports = await _databaseService.ReadDBAsync();
            var matchingReports = wasteReports
                .Where(report => report["WasteType"]?.Value<string>() == type)
                .Select(report => report.ToObject<WasteReport>())
                .Where(report => report != null);
            return matchingReports!;
        }

        public async Task PutWasteReport(int id, WasteReport wasteReport)
        {
            JArray wasteReports = await _databaseService.ReadDBAsync();
            var existingReport = wasteReports.FirstOrDefault(report => report["Id"]?.Value<int>() == id);

            if (existingReport == null)
            {
                throw new KeyNotFoundException("Waste report not found");
            }

            // Update the waste report properties, including the new CO2 emission value
            existingReport["WasteType"] = wasteReport.WasteType;
            existingReport["WasteProcessingFacility"] = wasteReport.WasteProcessingFacility;
            existingReport["WasteAmount"] = wasteReport.WasteAmount;
            existingReport["WasteDate"] = wasteReport.WasteDate;
            existingReport["WasteCollectorId"] = wasteReport.WasteCollectorId;
            existingReport["IsActive"] = wasteReport.IsActive;
            existingReport["Co2Emission"] = wasteReport.Co2Emission; // CO2 updated here

            await _databaseService.WriteDBAsync(wasteReports);
        }


        // DELETE - Delete a waste report by id
        public async Task DeleteWasteReport(int id)
        {
            JArray wasteReports = await _databaseService.ReadDBAsync();
            var existingReport = wasteReports.FirstOrDefault(report => report["Id"]?.Value<int>() == id);

            if (existingReport == null)
            {
                throw new KeyNotFoundException("Waste report not found");
            }

            wasteReports.Remove(existingReport); // Remove the report
            await _databaseService.WriteDBAsync(wasteReports);
        }

    }
}