namespace NotesApi.Repositories;

public interface IDbConnectionFactory
{
    Microsoft.Data.SqlClient.SqlConnection CreateConnection();
}
