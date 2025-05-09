using Models;

namespace Repositories;

public interface IRaceRepository
{
    Task<Race?> GetByIdAsync(int id);
}