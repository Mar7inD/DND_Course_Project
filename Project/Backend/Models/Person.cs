namespace Backend.Models
{
    public interface IPerson
    {
        string name { get; set; }
        string email { get; set; }
        string employeeId { get; set; }
        string password { get; set; }
        string role { get; set; }
    }

    public abstract class PersonBase : IPerson
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        private string _employeeId = string.Empty;
        public string password { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;

        public string employeeId
        {
            get
            {
                return _employeeId;
            }
            set
            {
                if (value.Length != 6 || !value.All(char.IsDigit))
                {
                    throw new ArgumentException("Invalid employee ID. It must be exactly 6 digits.");
                }
                _employeeId = value;
            }
        }
    }

    public class Employee : PersonBase
    {
        // Additional properties and methods specific to Employee
    }

    public class Manager : PersonBase
    {
        // Additional properties and methods specific to Manager
    }
}
