using Models;

namespace Repositories;

public interface IAdventurerRepository
{
    Task<IEnumerable<Adventurer>> GetAllAsync();
    Task<Adventurer?> GetByIdAsync(int id);
    Task CreateAsync(Adventurer adventurer);
    Task<bool> ExistsByPersonDataIdAsync(string personDataId);
}