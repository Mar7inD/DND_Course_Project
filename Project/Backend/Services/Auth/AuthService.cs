using Backend.Data;
using Microsoft.EntityFrameworkCore;



namespace Backend.Services;
public class AuthService : IAuthService {

    private readonly AppDbContext _dbContext;

    public AuthService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PersonBase> ValidateUser(string employeeId, string password)
    {
        PersonBase? existingUser = _dbContext.People            
        .FirstOrDefault(u => u.EmployeeId.ToLower() == employeeId.ToLower()) 
        ?? throw new Exception("User not found");

        if (!BCrypt.Net.BCrypt.Verify(password, existingUser.Password))
        {
            throw new Exception("Password mismatch");
        }

        return Task.FromResult(existingUser);
    }

    public async Task<string> AddPerson(PersonBase person)
    {
        try
        {
            // Detach any tracked entities with the same EmployeeId
            var trackedEntities = _dbContext.ChangeTracker.Entries<PersonBase>()
                .Where(e => e.Entity.EmployeeId == person.EmployeeId)
                .ToList();

            foreach (var trackedEntity in trackedEntities)
            {
                _dbContext.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            // Check if a person with the same EmployeeId exists
            var existingPerson = await _dbContext.People
                .FirstOrDefaultAsync(p => p.EmployeeId == person.EmployeeId);

            if (existingPerson != null)
            {
                if (!existingPerson.IsActive)
                {
                    // Reactivate the existing person
                    existingPerson.Name = person.Name;
                    existingPerson.Email = person.Email;
                    existingPerson.Role = person.Role;
                    existingPerson.Password = BCrypt.Net.BCrypt.HashPassword(person.Password);
                    existingPerson.IsActive = true;
                    existingPerson.ModifiedOn = DateTime.Now;

                    await _dbContext.SaveChangesAsync();
                    return "Existing inactive user re-registered successfully.";
                }
                else
                {
                    return "A user with this EmployeeId already exists and is active.";
                }
            }

            // Determine type based on role
            if (person.Role.ToLower() == "manager")
            {
                person = new Manager
                {
                    EmployeeId = person.EmployeeId,
                    Name = person.Name,
                    Email = person.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(person.Password),
                    Role = "Manager",
                    IsActive = person.IsActive,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };
            }
            else if (person.Role.ToLower() == "employee")
            {
                person = new Employee
                {
                    EmployeeId = person.EmployeeId,
                    Name = person.Name,
                    Email = person.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(person.Password),
                    Role = "Employee",
                    IsActive = person.IsActive,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now
                };
            }

            // Add to People DbSet
            _dbContext.People.Add(person);

            await _dbContext.SaveChangesAsync();

            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in AddPerson: {ex.Message}");
            return ex.Message;
        }
    }
}
