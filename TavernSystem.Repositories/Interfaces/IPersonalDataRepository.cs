using Models;

namespace Repositories;

public interface IPersonDataRepository
{
    Task<PersonData?> GetByIdAsync(string id);
}