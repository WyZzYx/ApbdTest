using Microsoft.Data.SqlClient;
using Models;

namespace Repositories;

public class PersonDataRepository : IPersonDataRepository
{
    private readonly string _connString;
    public PersonDataRepository(IConfiguration config) => 
        _connString = config.GetConnectionString("ConnectionString");

    public async Task<PersonData?> GetByIdAsync(string id)
    {
        using var conn = new SqlConnection(_connString);
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
SELECT Id, FirstName, MiddleName, LastName, HasBounty
FROM PersonData WHERE Id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var rdr = await cmd.ExecuteReaderAsync();
        if (!await rdr.ReadAsync()) return null;
        return new PersonData {
            Id = rdr.GetString(0),
            FirstName = rdr.GetString(1),
            MiddleName = rdr.IsDBNull(2) ? null : rdr.GetString(2),
            LastName = rdr.GetString(3),
            HasBounty = rdr.GetBoolean(4)
        };
    }
}
