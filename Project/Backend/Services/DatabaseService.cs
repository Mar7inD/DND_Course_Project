using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Backend.Services;
public class DatabaseService
{
    private readonly string filePath;

    public DatabaseService(string filePath)
    {
        this.filePath = filePath;
    }

    private void CheckDB()
    {
        // Check if database directory exists
        if (!Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            throw new DirectoryNotFoundException("Database directory not found.");
        }
    }

    public async Task<JArray> ReadDBAsync()
    {
        // Create the file content variable
        var content = new JArray();//

        try
        {
            CheckDB();

            // Read the file
            var jsonContent = await File.ReadAllTextAsync(filePath);

            // Check if file is empty
            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                content = new JArray();
            }
            else
            {
                content = JArray.Parse(jsonContent);
            }
        }
        catch (DirectoryNotFoundException e)
        {
            throw new Exception("Database read error occurred.", e);
        }

        return content;
    }

    public async Task<bool> WriteDBAsync(JArray content)
    {
        try
        {
            CheckDB();
            await File.WriteAllTextAsync(filePath, content.ToString());
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in WriteDB: {ex.Message}");
            return false;
        }
    }
}