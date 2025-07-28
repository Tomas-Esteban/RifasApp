namespace RaffleApp.Core.Models;

public class RaffleNumber
{
    public Guid Id { get; set; }
    public Guid RaffleId { get; set; }
    public int Number { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string? ParticipantName { get; set; }
    public string? ParticipantEmail { get; set; }
    public string? ParticipantPhone { get; set; }
    public DateTime? ReservedAt { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public Guid? PaymentId { get; set; }

    // Navegación
    public Raffle Raffle { get; set; } = null!;
    public Payment? Payment { get; set; }
    public decimal PricePaid { get; set; } 
}
