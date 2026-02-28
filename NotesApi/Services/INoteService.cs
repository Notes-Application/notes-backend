using NotesApi.DTOs.Notes;

namespace NotesApi.Services;

public interface INoteService
{
    Task<IEnumerable<NoteDto>> GetAllAsync(int userId, string? search, string? sortBy, string? sortOrder);
    Task<NoteDto?> GetByIdAsync(int id, int userId);
    Task<NoteDto> CreateAsync(int userId, CreateNoteDto dto);
    Task<NoteDto?> UpdateAsync(int id, int userId, UpdateNoteDto dto);
    Task<bool> DeleteAsync(int id, int userId);
}
