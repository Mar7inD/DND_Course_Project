namespace Shared.Models;

// DTO if needed
public interface IPersonDTO
{
    string Name { get; set; }
    string Email { get; set; }
    string Password { get; set; }
    string Role { get; set; }
    bool IsActive { get; set; }
}

public interface IPerson : IPersonDTO
{
    string EmployeeId { get; set; }
}

public class PersonBase : IPerson
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    private string _employeeId = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public string EmployeeId
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

public class Employee : PersonBase
{
    // Additional properties and methods specific to Employee
}

public class Manager : PersonBase
{
    // Additional properties and methods specific to Manager
}

public class EmployeeDTO : IPersonDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class ManagerDTO : IPersonDTO
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}