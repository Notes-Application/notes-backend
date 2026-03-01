using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NotesApi.DTOs.Auth;
using NotesApi.Models;
using NotesApi.Repositories;
using NotesApi.Services;

namespace NotesApi.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "Jwt:Key", "test-secret-key-that-is-long-enough-32chars!" },
                { "Jwt:Issuer", "NotesApi" },
                { "Jwt:Audience", "NotesApp" }
            })
            .Build();

        _authService = new AuthService(_mockUserRepo.Object, config);
    }

    [Fact]
    public async Task Register_ShouldReturnNull_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new User { Id = 1, Email = "test@example.com", Username = "test" };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(existingUser);

        // Act
        var result = await _authService.RegisterAsync(new RegisterDto
        {
            Username = "newuser",
            Email = "test@example.com",
            Password = "password123"
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Register_ShouldReturnToken_WhenValidInput()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _mockUserRepo.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _mockUserRepo.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);

        // Act
        var result = await _authService.RegisterAsync(new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123"
        });

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().NotBeEmpty();
        result.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task Login_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(new LoginDto
        {
            Email = "notfound@example.com",
            Password = "password123"
        });

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Login_ShouldReturnNull_WhenPasswordIsWrong()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("test@example.com")).ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(new LoginDto
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        });

        // Assert
        result.Should().BeNull();
    }
}
