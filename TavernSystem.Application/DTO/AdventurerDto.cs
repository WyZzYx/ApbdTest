namespace Application.DTO;

public class AdventurerDto
{
    public int Id { get; set; }
    public string Nickname { get; set; } = null!;
    public string Race { get; set; } = null!;
    public string ExperienceLevel { get; set; } = null!;
    public PersonDataDto PersonData { get; set; } = null!;
}