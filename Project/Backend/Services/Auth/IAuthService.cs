
namespace Backend.Services;

public interface IAuthService
{
    Task<PersonBase> ValidateUser(string username, string password);
    Task<string> AddPerson(PersonBase user);
}