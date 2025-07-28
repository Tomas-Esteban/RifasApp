namespace RaffleApp.Core.Models;

public class Raffle
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navegación
    public PriceConfiguration? PriceConfiguration { get; set; }
    public ICollection<RaffleNumber> RaffleNumbers { get; set; } = new List<RaffleNumber>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<Participant> Participants { get; set; } = new List<Participant>();
}