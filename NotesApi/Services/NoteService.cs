using NotesApi.DTOs.Notes;
using NotesApi.Models;
using NotesApi.Repositories;

namespace NotesApi.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;

    public NoteService(INoteRepository noteRepository)
    {
        _noteRepository = noteRepository;
    }

    public async Task<IEnumerable<NoteDto>> GetAllAsync(int userId, string? search, string? sortBy, string? sortOrder)
    {
        var notes = await _noteRepository.GetAllByUserIdAsync(userId, search, sortBy, sortOrder);
        return notes.Select(MapToDto);
    }

    public async Task<NoteDto?> GetByIdAsync(int id, int userId)
    {
        var note = await _noteRepository.GetByIdAsync(id, userId);
        return note == null ? null : MapToDto(note);
    }

    public async Task<NoteDto> CreateAsync(int userId, CreateNoteDto dto)
    {
        var note = new Note
        {
            UserId = userId,
            Title = dto.Title,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var id = await _noteRepository.CreateAsync(note);
        note.Id = id;
        return MapToDto(note);
    }

    public async Task<NoteDto?> UpdateAsync(int id, int userId, UpdateNoteDto dto)
    {
        var existing = await _noteRepository.GetByIdAsync(id, userId);
        if (existing == null) return null;

        existing.Title = dto.Title;
        existing.Content = dto.Content;
        existing.UpdatedAt = DateTime.UtcNow;

        await _noteRepository.UpdateAsync(existing);
        return MapToDto(existing);
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        return await _noteRepository.DeleteAsync(id, userId);
    }

    private static NoteDto MapToDto(Note note) => new()
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content,
        CreatedAt = note.CreatedAt,
        UpdatedAt = note.UpdatedAt
    };
}
