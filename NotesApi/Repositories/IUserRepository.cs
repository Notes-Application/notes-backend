using NotesApi.Models;

namespace NotesApi.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(User user);
}
