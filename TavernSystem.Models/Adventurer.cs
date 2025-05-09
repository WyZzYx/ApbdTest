namespace Models;

public class Adventurer
{
    
    public int Id { get; set; }
    public string Nickname { get; set; } = null!;
    public int RaceId { get; set; }
    public int ExperienceLevelId { get; set; }
    public string PersonDataId { get; set; } = null!;

    public Race? Race { get; set; }
    public ExperienceLevel? ExperienceLevel { get; set; }
    public PersonData? PersonData { get; set; }

}