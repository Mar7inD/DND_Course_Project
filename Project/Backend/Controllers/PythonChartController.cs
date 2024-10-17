
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Python.Runtime;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PythonChartController : ControllerBase
{
    [HttpPost]
    public string GenerateChartWithPython([FromBody] ChartRequest data) 
    {   
        string path = "../Frontend/wwwroot/images/plot.png";
        string scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "Python", "GenerateChart.py");

        if (!System.IO.File.Exists(scriptPath))
        {
            throw new FileNotFoundException($"The Python script file was not found: {scriptPath}");
        }

        try
        {
            using (Py.GIL())
            {
                dynamic py = Py.CreateScope();

                // Pass your data to the Python scope
                py.dates = data.Dates;
                py.co2_data = data.Co2Data;
                py.output_path = path;

                // Read and execute the script
                string script = System.IO.File.ReadAllText(scriptPath);
                py.Exec(script);

                dynamic result = py.generate_chart(py.dates, py.co2_data, py.output_path);
                Console.WriteLine("Runned Python script");
                // Return the path to the generated chart
                return "images/plot.png";
            }
        }
        catch (Exception ex)
        {   
            Console.WriteLine($"Error executing Python: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}

