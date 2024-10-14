namespace Backend.Models
{
    // DTO if needed
    public interface IPersonDTO
    {
        string name { get; set; }
        string email { get; set; }
        string password { get; set; }
        string role { get; set; }
        bool isActive { get; set; }
    }

    public interface IPerson : IPersonDTO
    {
        string employeeId { get; set; }
    }

    public abstract class PersonBase : IPerson
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        private string _employeeId = string.Empty;
        public string password { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;

        public string employeeId
        {
            get => _employeeId;
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

    // Class Person
    public class Person : PersonBase
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string employeeId { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
    }

    public class Employee : Person
    {
        // Additional properties and methods specific to Employee
    }

    public class Manager : Person
    {
        // Additional properties and methods specific to Manager
    }

    public class EmployeeDTO : IPersonDTO
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
    }

    public class ManagerDTO : IPersonDTO
    {
        public string name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string role { get; set; } = string.Empty;
        public bool isActive { get; set; } = true;
    }
}
