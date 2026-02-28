namespace NotesApi.DTOs.Notes;

public class CreateNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
}
