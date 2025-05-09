using Microsoft.Data.SqlClient;
using Models;

namespace Repositories;

public class ExperienceLevelRepository : IExperienceLevelRepository
{
    private readonly string _connString;
    public ExperienceLevelRepository(IConfiguration config) => 
        _connString = config.GetConnectionString("ConnectionString");

    public async Task<ExperienceLevel?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(_connString);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Level FROM ExperienceLevels WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var rdr = await cmd.ExecuteReaderAsync();
        if (!await rdr.ReadAsync()) return null;
        return new ExperienceLevel { Id = rdr.GetInt32(0), Level = rdr.GetString(1) };
    }
}