namespace RaffleApp.Core.Models;

public class PriceConfiguration
{
    public Guid Id { get; set; }
    public Guid RaffleId { get; set; }
    public decimal PriceFor1 { get; set; }
    public decimal PriceFor2 { get; set; }
    public decimal PriceFor3 { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navegación
    public Raffle Raffle { get; set; } = null!;
}