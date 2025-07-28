namespace RaffleApp.Core.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid RaffleId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantEmail { get; set; } = string.Empty;
    public string? ParticipantPhone { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "ARS";
    public string? PaymentGatewayId { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navegación
    public Raffle Raffle { get; set; } = null!;
    public ICollection<RaffleNumber> RaffleNumbers { get; set; } = new List<RaffleNumber>();
}

public enum PaymentStatus
{
    Pending,
    Completed,
    Failed,
    Cancelled
}