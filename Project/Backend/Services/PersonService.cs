using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Backend.Services;

public class PersonService
{
    private readonly AppDbContext _dbContext;

    public PersonService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<PersonBase>> GetPeople(string? role = null, bool? active = null)
    {
        var query = _dbContext.People.AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(p => p.Role == role);
        }

        if (active.HasValue)
        {
            query = query.Where(p => p.IsActive == active.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<PersonBase?> GetPersonById(string employeeId)
    {
        return await _dbContext.People.FirstOrDefaultAsync(p => p.EmployeeId == employeeId);
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

    public async Task<string> UpdatePerson(string employeeId, PersonBase updatedPerson)
    {
        try
        {
            var existingPerson = await _dbContext.People.FirstOrDefaultAsync(p => p.EmployeeId == employeeId);

            if (existingPerson == null)
            {
                return "Employee ID not found.";
            }

            // Update other properties
            existingPerson.Name = updatedPerson.Name;
            existingPerson.Email = updatedPerson.Email;
            existingPerson.Role = updatedPerson.Role;
            existingPerson.IsActive = updatedPerson.IsActive;
            existingPerson.ModifiedOn = DateTime.Now;

            // Update the password only if it is provided
            if (!string.IsNullOrEmpty(updatedPerson.Password))
            {
                existingPerson.Password = BCrypt.Net.BCrypt.HashPassword(updatedPerson.Password);
            }

            await _dbContext.SaveChangesAsync();
            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PutPerson: {ex.Message}");
            return ex.Message;
        }
    }

    public async Task<string> DeletePerson(string employeeId)
    {
        try
        {
            var person = await _dbContext.People.FirstOrDefaultAsync(p => p.EmployeeId == employeeId);

            if (person != null)
            {
                person.IsActive = false;
                person.ModifiedOn = DateTime.Now; // Update the modified timestamp
                await _dbContext.SaveChangesAsync();
                return "Success";
            }
            else
            {
                return "Employee ID not found.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeletePerson: {ex.Message}");
            return ex.Message;
        }
    }

}
