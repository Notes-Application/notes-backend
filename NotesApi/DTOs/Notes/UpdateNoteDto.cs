namespace NotesApi.DTOs.Notes;

public class UpdateNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}
