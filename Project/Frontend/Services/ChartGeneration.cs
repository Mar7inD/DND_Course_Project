using BlazorBootstrap;

namespace Frontend.Services;

public class ChartGeneration
{
    public Tuple<ChartData, LineChartOptions> GenerateLineChart(List<WasteReport> wasteReports)
        {
            var colors = ColorUtility.CategoricalTwelveColors;

            // Get the date range for the last 7 days
            var sevenDaysAgo = DateTime.Now.Date.AddDays(-7);

            // Filter data for the last 7 days and group by date
            var groupedData = wasteReports
                .Where(r => r.WasteDate.Date >= sevenDaysAgo) // Filter for last 7 days
                .GroupBy(r => r.WasteDate.Date) // Group by the date part of WasteDate
                .Select(g => new
                {
                    Date = g.Key,
                    TotalCo2Emission = g.Sum(r => r.Co2Emission ?? 0) // Sum CO2 emissions, handling nulls
                })
                .OrderBy(g => g.Date) // Ensure dates are in ascending order
                .ToList();

            // Extract labels and data from grouped results
            var labels = groupedData.Select(g => g.Date.ToString("dd-MM-yyyy")).ToList();
            var co2EmissionData = groupedData.Select(g => (double?)g.TotalCo2Emission).ToList(); // Convert to nullable double

            // Create the dataset for the line chart
            var dataset = new LineChartDataset
            {
                Label = "CO2 Emissions",
                Data = co2EmissionData, // Use aggregated data
                BackgroundColor = colors[0],
                BorderColor = colors[0],
                BorderWidth = 2,
                HoverBorderWidth = 4,
            };

            // Prepare chart data and options
            var datasets = new List<IChartDataset> { dataset };
            ChartData lineChartData = new ChartData { Labels = labels, Datasets = datasets };

            LineChartOptions lineChartOptions = new();
            lineChartOptions.Locale = "en-US";
            lineChartOptions.Responsive = true;
            lineChartOptions.Interaction = new Interaction { Mode = InteractionMode.Index };

            lineChartOptions.Scales.X!.Title = new ChartAxesTitle { Text = "Date", Display = true };
            lineChartOptions.Scales.Y!.Title = new ChartAxesTitle { Text = "CO2 Emissions (kg)", Display = true };

            return new Tuple<ChartData, LineChartOptions>(lineChartData, lineChartOptions);
        }

        public Tuple<ChartData, PieChartOptions> GeneratePieChart(List<WasteReport> wasteReports)
        {
            var colors = ColorUtility.CategoricalTwelveColors;
            var wasteTypeGroups = wasteReports.GroupBy(r => r.WasteType)
                .Select(g => new { WasteType = g.Key, Amount = g.Sum(r => r.WasteAmount) })
                .ToList();

            ChartData pieChartData = new ChartData
            {
                Labels = wasteTypeGroups.Select(g => g.WasteType).ToList(),
                Datasets = new List<IChartDataset>
                {
                    new PieChartDataset
                    {
                        Label = "Waste Type Distribution",
                        Data = wasteTypeGroups.Select(g => (double?)g.Amount).ToList(),
                        BackgroundColor = colors.Take(wasteTypeGroups.Count).ToList(),
                        HoverBackgroundColor = colors.Take(wasteTypeGroups.Count).ToList()
                    }
                }
            };

            PieChartOptions pieChartOptions = new();

            return new Tuple<ChartData, PieChartOptions> (pieChartData, pieChartOptions);
        }

        public Tuple<ChartData, BarChartOptions> GenerateBarChart(List<WasteReport> wasteReports)
        {
            var colors = ColorUtility.CategoricalTwelveColors;
            var facilities = wasteReports.Select(r => r.WasteProcessingFacility).Distinct().ToList();
            var wasteTypes = wasteReports.Select(r => r.WasteType).Distinct().ToList();

            var datasets = new List<IChartDataset>();
            int colorIndex = 0;

            foreach (var wasteType in wasteTypes)
            {
                var dataset = new BarChartDataset()
                {
                    Label = wasteType,
                    Data = facilities.Select(facility => (double?)wasteReports
                        .Where(r => r.WasteProcessingFacility == facility && r.WasteType == wasteType)
                        .Sum(r => r.WasteAmount))
                        .ToList(),
                    BackgroundColor = new List<string> { colors[(colorIndex % colors.Length)] },
                    BorderColor = new List<string> { colors[(colorIndex % colors.Length)] },
                    BorderWidth = new List<double> { 0 }
                };

                datasets.Add(dataset);
                colorIndex++;
            }

            ChartData barChartData = new ChartData { Labels = facilities, Datasets = datasets };

            BarChartOptions barChartOptions = new();
            barChartOptions.Responsive = true;
            barChartOptions.Interaction = new Interaction { Mode = InteractionMode.Y };
            barChartOptions.IndexAxis = "y";

            barChartOptions.Scales.X!.Title = new ChartAxesTitle { Text = "Waste Amount (kg)", Display = true };
            barChartOptions.Scales.Y!.Title = new ChartAxesTitle { Text = "Processing Facility", Display = true };

            barChartOptions.Scales.X.Stacked = true;
            barChartOptions.Scales.Y.Stacked = true;

            barChartOptions.Plugins.Title!.Text = "Waste Type";
            barChartOptions.Plugins.Title.Display = true;

            return new Tuple<ChartData, BarChartOptions>(barChartData, barChartOptions);
        }
}