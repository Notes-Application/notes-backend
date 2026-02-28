using Dapper;
using NotesApi.Models;

namespace NotesApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email",
            new { Email = email }
        );
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Username = @Username",
            new { Username = username }
        );
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt)
            VALUES (@Username, @Email, @PasswordHash, @CreatedAt);
            SELECT SCOPE_IDENTITY();
            """,
            user
        );
    }
}
