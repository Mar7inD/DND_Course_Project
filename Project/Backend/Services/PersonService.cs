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

        public async Task<List<PersonBase>> GetPeople(string? role = null, string? active = null, string? employeeId = null)
        {
            try
            {
                var peopleArray = await _databaseService.ReadDBAsync();
                
                // Apply all filters
                peopleArray = new JArray(peopleArray
                                            .Where(p => (role == null || p["Role"]?.Value<string>() == role) &&
                                                        (active == null || p["IsActive"]?.Value<bool>() == bool.Parse(active)) &&
                                                        (employeeId == null || p["EmployeeId"]?.Value<string>() == employeeId)));

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
                // Ensure the password is hashed
                person.Password = BCrypt.Net.BCrypt.HashPassword(person.Password);

                // Read the existing people data from the database
                var peopleArray = await _databaseService.ReadDBAsync();
                
                // Check if a person with the same EmployeeId exists and is inactive
                var existingPerson = peopleArray
                    .FirstOrDefault(p => p["EmployeeId"]?.Value<string>() == person.EmployeeId.ToString());

                if (existingPerson != null)
                {
                    if (existingPerson["IsActive"]?.Value<bool>() == false)
                    {
                        // Reactivate the existing person by setting IsActive to true
                        existingPerson["IsActive"] = true;
                        existingPerson["Password"] = person.Password; // Update password if needed
                        existingPerson["ModifiedOn"] = DateTime.UtcNow; // Optional: Track reactivation date
                    }
                    else
                    {
                        // If the person is active, throw an exception
                        throw new Exception("Employee ID already exists and is active.");
                    }
                }
                else
                {
                    // Add new person if no record with the EmployeeId exists
                    var newPerson = JObject.FromObject(person);
                    newPerson["CreatedOn"] = DateTime.UtcNow; // Optionally add a creation timestamp
                    peopleArray.Add(newPerson);
                }

                // Write the updated data back to the database
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
