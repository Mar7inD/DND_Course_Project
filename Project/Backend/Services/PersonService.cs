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
            // Check if a person with the same EmployeeId already exists
            var existingPerson = await _dbContext.People.FirstOrDefaultAsync(p => p.EmployeeId == person.EmployeeId);

            if (existingPerson != null)
            {
                if (!existingPerson.IsActive)
                {
                    // Update the existing person's details and activate them
                    existingPerson.Name = person.Name;
                    existingPerson.Email = person.Email;
                    existingPerson.Role = person.Role;
                    existingPerson.Password = BCrypt.Net.BCrypt.HashPassword(person.Password); // Rehash the password
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

            // If no existing user is found, create a new one
            person.Password = BCrypt.Net.BCrypt.HashPassword(person.Password); // Hash the password
            person.CreatedOn = DateTime.Now;
            person.ModifiedOn = DateTime.Now;

            _dbContext.People.Add(person);
            await _dbContext.SaveChangesAsync();

            return "Success";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in PostPerson: {ex.Message}");
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
