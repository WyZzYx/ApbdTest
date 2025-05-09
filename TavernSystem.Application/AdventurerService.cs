using System.Text.RegularExpressions;
using Application.DTO;
using Models;
using Repositories;

namespace Application;

public class AdventurerService : IAdventurerService
    {
        private readonly IAdventurerRepository _advRepo;
        private readonly IPersonDataRepository _pdRepo;
        private readonly IRaceRepository _raceRepo;
        private readonly IExperienceLevelRepository _expRepo;
        
        private static readonly Regex PersonDataIdRegex =
            new(@"^[A-Z]{2}(?:0[0-9]{3}|[1-9][0-9]{3})(?:0[1-9]|1[0-1])(?:0[1-9]|[12][0-8])[0-9]{4}[A-Z]{2}$",
                  RegexOptions.Compiled);

        public AdventurerService(
            IAdventurerRepository advRepo,
            IPersonDataRepository pdRepo,
            IRaceRepository raceRepo,
            IExperienceLevelRepository expRepo)
        {
            _advRepo = advRepo;
            _pdRepo = pdRepo;
            _raceRepo = raceRepo;
            _expRepo = expRepo;
        }

        public async Task<IEnumerable<AdventurerSummaryDto>> GetAllAsync()
        {
            var all = await _advRepo.GetAllAsync();
            return all.Select(a => new AdventurerSummaryDto {
                Id = a.Id,
                Nickname = a.Nickname
            });
        }

        public async Task<AdventurerDto?> GetByIdAsync(int id)
        {
            var a = await _advRepo.GetByIdAsync(id);
            if (a is null) return null;
            return new AdventurerDto {
                Id = a.Id,
                Nickname = a.Nickname,
                Race = a.Race!.Name,
                ExperienceLevel = a.ExperienceLevel!.Level,
                PersonData = new PersonDataDto {
                    Id = a.PersonData!.Id,
                    FirstName = a.PersonData.FirstName,
                    MiddleName = a.PersonData.MiddleName,
                    LastName = a.PersonData.LastName,
                    HasBounty = a.PersonData.HasBounty
                }
            };
        }

        public async Task<int> CreateAsync(CreateAdventurerRequest req)
        {
            if (!PersonDataIdRegex.IsMatch(req.PersonDataId))
                throw new ArgumentException("Invalid personDataId format");

            var pd = await _pdRepo.GetByIdAsync(req.PersonDataId)
                     ?? throw new KeyNotFoundException("Person data not found");

            if (pd.HasBounty)
                throw new InvalidOperationException("Person has active bounty");

            if (await _advRepo.ExistsByPersonDataIdAsync(req.PersonDataId))
                throw new InvalidOperationException("Already registered");

            var adv = new Adventurer {
                Nickname = req.Nickname,
                RaceId = req.RaceId,
                ExperienceLevelId = req.ExperienceLevelId,
                PersonDataId = req.PersonDataId
            };
            await _advRepo.CreateAsync(adv);

            var list = await _advRepo.GetAllAsync();
            return list.Max(x => x.Id);
        }
    }