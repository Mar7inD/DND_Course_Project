using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Backend.Models;

namespace Backend.Services;
    public class PersonService
    {
        private readonly string filePath;
        private readonly DatabaseService _databaseService;
        public JsonSerializerSettings settings = new JsonSerializerSettings();
        

        public PersonService()
        {
            // Set the file path
            filePath = Path.Combine("Database", "People.json");
            
            // Initialize the database service
            _databaseService = new DatabaseService(filePath);
            
            // Add the custom converter to the settings
            settings.Converters.Add(new PersonConverter());
        }

        public async Task<List<IPerson>> GetPeople()
        {
            try
            {
                var peopleArray = await _databaseService.ReadDB();
                var peopleList = JsonConvert.DeserializeObject<List<IPerson>>(peopleArray.ToString(), settings);
                return peopleList!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPeople: {ex.Message}");
                return new List<IPerson>();
            }
        }

        public async Task<List<IPerson>> GetPeopleByRole(string role)
        {
             try
            {
                var peopleArray = await _databaseService.ReadDBByRole(role);
                var peopleList = JsonConvert.DeserializeObject<List<IPerson>>(peopleArray.ToString(), settings);
                return peopleList!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPeopleByRole: {ex.Message}");
                return new List<IPerson>();
            }
        }

        public async Task<string> PostPerson(IPerson person)
        {
            try
            {
                Console.WriteLine($"Post person name: {person.name}");
                var peopleArray = await _databaseService.ReadDB();
                if (peopleArray.Any(p => p["employeeId"]?.Value<string>() == person.employeeId.ToString()))
                {
                    throw new Exception("Employee ID already exists.");
                }
                peopleArray.Add(JObject.FromObject(person));
                await _databaseService.WriteDB(peopleArray);
                Console.WriteLine(person.isActive);
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostPerson: {ex.Message}");
                return ex.Message;
            }
        }

        public async Task<string> PutPerson(int employeeId, IPersonDTO person)
        {
            try
            {
                var peopleArray = await _databaseService.ReadDB();
                var existingPerson = peopleArray.FirstOrDefault(p => p["employeeId"]?.Value<string>() == employeeId.ToString());
        
                // Check if the employee ID exists
                if (existingPerson != null)
                {
                    foreach (var property in JObject.FromObject(person).Properties())
                    {
                        existingPerson[property.Name] = property.Value;
                    }
                }
                else
                {
                    throw new Exception("Employee ID not found.");
                }

                // Write the updated array back to the database
                await _databaseService.WriteDB(peopleArray);
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PutPerson: {ex.Message}");
                return ex.Message;
            }
        }
}
