using Models;

namespace Repositories;

public interface IExperienceLevelRepository
{
    Task<ExperienceLevel?> GetByIdAsync(int id);
}