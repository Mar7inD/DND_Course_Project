using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Backend.Services
{
    public class WasteReportService
    {
        private readonly AppDbContext _context;
        private readonly WasteTypes _wasteTypes;

        public WasteReportService(AppDbContext context)
        {
            _context = context;
            _wasteTypes = new WasteTypes();
        }

        // Create a new waste report
        public async Task<string> PostWasteReport(WasteReport wasteReport)
        {
            // Check if waste type is valid and calculate CO2 emissions
            wasteReport.Co2Emission = await _wasteTypes.isValidWasteReturnCo2Emissions(
                wasteReport.WasteType, 
                wasteReport.WasteProcessingFacility, 
                wasteReport.WasteAmount
            );

            // Add waste report to database
            _context.WasteReports.Add(wasteReport);
            await _context.SaveChangesAsync();

            return "Success";
        }

        // Get all active waste reports
        public async Task<List<WasteReport>> GetAllWasteReports()
        {
            return await _context.WasteReports
                .Where(report => report.IsActive)
                .ToListAsync();
        }

        // Get a waste report by ID
        public async Task<WasteReport?> GetWasteReportById(int id)
        {
            return await _context.WasteReports.FindAsync(id);
        }

        // Get waste reports by waste type
        public async Task<List<WasteReport>> GetWasteReportByType(string type)
        {
            return await _context.WasteReports
                .Where(report => report.WasteType == type && report.IsActive)
                .ToListAsync();
        }

        // Update a waste report
        public async Task PutWasteReport(int id, WasteReport wasteReport)
        {
            var existingReport = await _context.WasteReports.FindAsync(id);
            if (existingReport == null)
            {
                throw new KeyNotFoundException("Waste report not found");
            }

            // Update properties
            existingReport.WasteType = wasteReport.WasteType;
            existingReport.WasteProcessingFacility = wasteReport.WasteProcessingFacility;
            existingReport.WasteAmount = wasteReport.WasteAmount;
            existingReport.WasteDate = wasteReport.WasteDate;
            existingReport.WasteCollectorId = wasteReport.WasteCollectorId;
            existingReport.IsActive = wasteReport.IsActive;

            // Recalculate CO2 emissions
            existingReport.Co2Emission = await _wasteTypes.isValidWasteReturnCo2Emissions(
                wasteReport.WasteType, 
                wasteReport.WasteProcessingFacility, 
                wasteReport.WasteAmount
            );

            existingReport.ModifiedOn = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        // Mark a waste report as inactive (soft delete)
        public async Task DeleteWasteReport(int id)
        {
            var existingReport = await _context.WasteReports.FindAsync(id);
            if (existingReport == null)
            {
                throw new KeyNotFoundException("Waste report not found");
            }

            existingReport.IsActive = false;
            existingReport.ModifiedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
