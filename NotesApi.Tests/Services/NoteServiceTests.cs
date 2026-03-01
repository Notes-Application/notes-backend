using FluentAssertions;
using Moq;
using NotesApi.DTOs.Notes;
using NotesApi.Models;
using NotesApi.Repositories;
using NotesApi.Services;

namespace NotesApi.Tests.Services;

public class NoteServiceTests
{
    private readonly Mock<INoteRepository> _mockRepo;
    private readonly NoteService _noteService;

    public NoteServiceTests()
    {
        _mockRepo = new Mock<INoteRepository>();
        _noteService = new NoteService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateNote_ShouldReturnNoteDto_WhenValidInput()
    {
        // Arrange
        var userId = 1;
        var dto = new CreateNoteDto { Title = "Test Note", Content = "Test Content" };
        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Note>())).ReturnsAsync(1);

        // Act
        var result = await _noteService.CreateAsync(userId, dto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Note");
        result.Content.Should().Be("Test Content");
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenNoteNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Note?)null);

        // Act
        var result = await _noteService.GetByIdAsync(99, 1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnNote_WhenNoteExists()
    {
        // Arrange
        var note = new Note
        {
            Id = 1,
            UserId = 1,
            Title = "My Note",
            Content = "My Content",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync(note);

        // Act
        var result = await _noteService.GetByIdAsync(1, 1);

        // Assert
        result.Should().NotBeNull();
        result!.Title.Should().Be("My Note");
    }

    [Fact]
    public async Task UpdateNote_ShouldReturnNull_WhenNoteDoesNotBelongToUser()
    {
        // Arrange - note belongs to userId 2, but userId 1 is trying to update
        _mockRepo.Setup(r => r.GetByIdAsync(1, 1)).ReturnsAsync((Note?)null);

        // Act
        var result = await _noteService.UpdateAsync(1, 1, new UpdateNoteDto { Title = "New Title" });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteNote_ShouldReturnFalse_WhenNoteNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(99, 1)).ReturnsAsync(false);

        // Act
        var result = await _noteService.DeleteAsync(99, 1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteNote_ShouldReturnTrue_WhenNoteDeleted()
    {
        // Arrange
        _mockRepo.Setup(r => r.DeleteAsync(1, 1)).ReturnsAsync(true);

        // Act
        var result = await _noteService.DeleteAsync(1, 1);

        // Assert
        result.Should().BeTrue();
    }
}
