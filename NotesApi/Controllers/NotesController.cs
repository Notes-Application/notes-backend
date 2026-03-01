using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApi.DTOs.Notes;
using NotesApi.Services;

namespace NotesApi.Controllers;

[ApiController]
[Route("api/v1/notes")]
[Authorize]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("User ID not found in token"));

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder)
    {
        var notes = await _noteService.GetAllAsync(GetUserId(), search, sortBy, sortOrder);
        return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var note = await _noteService.GetByIdAsync(id, GetUserId());
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateNoteDto dto)
    {
        var note = await _noteService.CreateAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateNoteDto dto)
    {
        var note = await _noteService.UpdateAsync(id, GetUserId(), dto);
        if (note == null) return NotFound();
        return Ok(note);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _noteService.DeleteAsync(id, GetUserId());
        if (!deleted) return NotFound();
        return NoContent();
    }
}
