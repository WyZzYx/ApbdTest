using System.Data;
using System.Net.NetworkInformation;
using System.Transactions;
using Microsoft.Data.SqlClient;
using Models;

namespace Repositories;

public class AdventurerRepository : IAdventurerRepository
    {
        private readonly string _connString;
        public AdventurerRepository(IConfiguration config)
        {
            _connString = config.GetConnectionString("ConnectionString");
        }

        public async Task<IEnumerable<Adventurer>> GetAllAsync()
        {
            var list = new List<Adventurer>();
            using var conn = new SqlConnection(_connString);
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Nickname FROM Adventurers";
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                list.Add(new Adventurer {
                    Id = rdr.GetInt32(0),
                    Nickname = rdr.GetString(1)
                });
            }
            return list;
        }

        public async Task<Adventurer?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connString);
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
SELECT a.Id, a.Nickname,
       r.Id, r.Name,
       e.Id, e.Level,
       p.Id, p.FirstName, p.MiddleName, p.LastName, p.HasBounty
FROM Adventurers a
JOIN Races r ON a.RaceId = r.Id
JOIN ExperienceLevels e ON a.ExperienceLevelId = e.Id
JOIN PersonData p ON a.PersonDataId = p.Id
WHERE a.Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new Adventurer {
                Id = rdr.GetInt32(0),
                Nickname = rdr.GetString(1),
                Race = new Race { Id = rdr.GetInt32(2), Name = rdr.GetString(3) },
                ExperienceLevel = new ExperienceLevel { Id = rdr.GetInt32(4), Level = rdr.GetString(5) },
                PersonData = new PersonData {
                    Id = rdr.GetString(6),
                    FirstName = rdr.GetString(7),
                    MiddleName = rdr.IsDBNull(8) ? null : rdr.GetString(8),
                    LastName = rdr.GetString(9),
                    HasBounty = rdr.GetBoolean(10)
                }
            };
        }

        public async Task CreateAsync(Adventurer adv)
        {
            using var conn = new SqlConnection(_connString);
            await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            try
            {
                // Check for existing registration
                using var check = conn.CreateCommand();
                check.Transaction = tx;
                check.CommandText = "SELECT COUNT(1) FROM Adventurers WHERE PersonDataId = @pid";
                check.Parameters.AddWithValue("@pid", adv.PersonDataId);
                var exists = (int)await check.ExecuteScalarAsync() > 0;
                if (exists)
                    throw new InvalidOperationException("Person already registered");

                // Insert
                using var ins = conn.CreateCommand();
                ins.Transaction = tx;
                ins.CommandText = @"
INSERT INTO Adventurers (Nickname, RaceId, ExperienceLevelId, PersonDataId)
VALUES (@nick, @rid, @eid, @pid)";
                ins.Parameters.AddWithValue("@nick", adv.Nickname);
                ins.Parameters.AddWithValue("@rid", adv.RaceId);
                ins.Parameters.AddWithValue("@eid", adv.ExperienceLevelId);
                ins.Parameters.AddWithValue("@pid", adv.PersonDataId);
                await ins.ExecuteNonQueryAsync();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<bool> ExistsByPersonDataIdAsync(string pid)
        {
            using var conn = new SqlConnection(_connString);
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM Adventurers WHERE PersonDataId = @pid";
            cmd.Parameters.AddWithValue("@pid", pid);
            return (int)await cmd.ExecuteScalarAsync() > 0;
        }
    }