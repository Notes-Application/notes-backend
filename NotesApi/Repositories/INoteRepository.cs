using NotesApi.Models;

namespace NotesApi.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId, string? search, string? sortBy, string? sortOrder);
    Task<Note?> GetByIdAsync(int id, int userId);
    Task<int> CreateAsync(Note note);
    Task<bool> UpdateAsync(Note note);
    Task<bool> DeleteAsync(int id, int userId);
}
