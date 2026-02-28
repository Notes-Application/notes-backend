using Dapper;
using NotesApi.Models;

namespace NotesApi.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public NoteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Note>> GetAllByUserIdAsync(int userId, string? search, string? sortBy, string? sortOrder)
    {
        var sql = """
            SELECT * FROM Notes 
            WHERE UserId = @UserId
            AND (@Search IS NULL OR Title LIKE @SearchPattern OR Content LIKE @SearchPattern)
            ORDER BY
            CASE WHEN @SortBy = 'title' AND @SortOrder = 'asc' THEN Title END ASC,
            CASE WHEN @SortBy = 'title' AND @SortOrder = 'desc' THEN Title END DESC,
            CASE WHEN @SortBy = 'createdAt' AND @SortOrder = 'asc' THEN CreatedAt END ASC,
            CASE WHEN @SortBy = 'createdAt' AND @SortOrder = 'desc' THEN CreatedAt END DESC,
            CASE WHEN @SortBy IS NULL THEN CreatedAt END DESC
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Note>(sql, new
        {
            UserId = userId,
            Search = search,
            SearchPattern = $"%{search}%",
            SortBy = sortBy,
            SortOrder = sortOrder ?? "desc"
        });
    }

    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Note>(
            "SELECT * FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId }
        );
    }

    public async Task<int> CreateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(
            """
            INSERT INTO Notes (UserId, Title, Content, CreatedAt, UpdatedAt)
            VALUES (@UserId, @Title, @Content, @CreatedAt, @UpdatedAt);
            SELECT SCOPE_IDENTITY();
            """,
            note
        );
    }

    public async Task<bool> UpdateAsync(Note note)
    {
        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(
            """
            UPDATE Notes 
            SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt
            WHERE Id = @Id AND UserId = @UserId
            """,
            note
        );
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(
            "DELETE FROM Notes WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId }
        );
        return affected > 0;
    }
}
