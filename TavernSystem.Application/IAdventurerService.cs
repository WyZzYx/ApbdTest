using Application.DTO;

namespace Application;

public interface IAdventurerService
{
    Task<IEnumerable<AdventurerSummaryDto>> GetAllAsync();
    Task<AdventurerDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateAdventurerRequest req);
}

