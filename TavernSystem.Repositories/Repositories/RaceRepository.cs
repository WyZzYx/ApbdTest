using Models;

namespace Repositories;

using Microsoft.Data.SqlClient;
public class RaceRepository : IRaceRepository
{
    private readonly string _connString;
    public RaceRepository(IConfiguration config) => 
        _connString = config.GetConnectionString("TavernSystemDb");

    public async Task<Race?> GetByIdAsync(int id)
    {
        using var conn = new SqlConnection(_connString);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name FROM Races WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var rdr = await cmd.ExecuteReaderAsync();
        if (!await rdr.ReadAsync()) return null;
        return new Race { Id = rdr.GetInt32(0), Name = rdr.GetString(1) };
    }
}