using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Backend.Services;
    public class PersonService
    {
        private readonly DatabaseService _databaseService;
        public JsonSerializerSettings settings = new JsonSerializerSettings();
        

        public PersonService()
        {            
            // Initialize the database service
            _databaseService = new DatabaseService("Database/People.json");

            // Add the custom converter to the settings
            settings.Converters.Add(new PersonConverter());
        }

        public async Task<List<PersonBase>> GetPeople(string? role, string? active)
        {
            try
            {
                var peopleArray = await _databaseService.ReadDBAsync();
                peopleArray = new JArray(peopleArray
                                            .Where(p => (role == null || p["Role"]?.Value<string>() == role) && 
                                                        (active == null || p["IsActive"]!.Value<bool>() == bool.Parse(active))));
                
                var peopleList = JsonConvert.DeserializeObject<List<PersonBase>>(peopleArray.ToString(), settings);
                return peopleList!;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPeople: {ex.Message}");
                return new List<PersonBase>();
            }
        }

        public async Task<string> PostPerson(IPerson person)
        {
            try
            {
                Console.WriteLine(person.IsActive);
                var peopleArray = await _databaseService.ReadDBAsync();
                if (peopleArray.Any(p => p["EmployeeId"]?.Value<string>() == person.EmployeeId.ToString()))
                {
                    throw new Exception("Employee ID already exists.");
                }
                peopleArray.Add(JObject.FromObject(person));
                await _databaseService.WriteDBAsync(peopleArray);
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
                var peopleArray = await _databaseService.ReadDBAsync();
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
                await _databaseService.WriteDBAsync(peopleArray);
                return "Success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PutPerson: {ex.Message}");
                return ex.Message;
            }
        }
}
