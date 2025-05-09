namespace Application.DTO;

public class CreateAdventurerRequest
{
    public string Nickname { get; set; } = null!;
    public int RaceId { get; set; }
    public int ExperienceLevelId { get; set; }
    public string PersonDataId { get; set; } = null!;
}