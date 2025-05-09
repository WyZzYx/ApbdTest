namespace Models;

public class PersonData
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = null!;
    public bool HasBounty { get; set; }
}