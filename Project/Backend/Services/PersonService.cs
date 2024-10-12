using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Backend.Models;
using Backend.Converters;

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

        public async Task<bool> PostPerson(IPerson person)
        {
            try
            {
                var peopleArray = await _databaseService.ReadDB();
                peopleArray.Add(JObject.FromObject(person));
                return await _databaseService.WriteDB(peopleArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostPerson: {ex.Message}");
                return false;
            }
        }
}
