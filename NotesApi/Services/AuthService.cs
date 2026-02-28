using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using NotesApi.DTOs.Auth;
using NotesApi.Models;
using NotesApi.Repositories;

namespace NotesApi.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Check if email or username already exists
        var existingEmail = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingEmail != null) return null;

        var existingUsername = await _userRepository.GetByUsernameAsync(dto.Username);
        if (existingUsername != null) return null;

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            CreatedAt = DateTime.UtcNow
        };

        var id = await _userRepository.CreateAsync(user);
        user.Id = id;

        return new AuthResponseDto
        {
            Token = GenerateJwtToken(user),
            Username = user.Username,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

        return new AuthResponseDto
        {
            Token = GenerateJwtToken(user),
            Username = user.Username,
            Email = user.Email
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
