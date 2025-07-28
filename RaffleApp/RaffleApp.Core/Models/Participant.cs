namespace RaffleApp.Core.Models;

public class Participant
{
    public Guid Id { get; set; }
    public Guid RaffleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }

    public Raffle Raffle { get; set; } = null!;
}