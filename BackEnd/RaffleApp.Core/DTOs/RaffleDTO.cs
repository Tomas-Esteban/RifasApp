public class RaffleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public PriceConfigurationDto? PriceConfiguration { get; set; }
    public List<RaffleNumberDto> RaffleNumbers { get; set; } = new();
}

public class RaffleNumberDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public bool IsAvailable { get; set; }
    public string? ParticipantName { get; set; }
    public string? ParticipantEmail { get; set; }
    public string? ParticipantPhone { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public decimal PricePaid { get; set; }
}

public class PriceConfigurationDto
{
    public Guid Id { get; set; }
    public decimal PriceFor1 { get; set; }
    public decimal PriceFor2 { get; set; }
    public decimal PriceFor3 { get; set; }
}

public class PaymentRequestDto
{
    public Guid RaffleId { get; set; }
    public List<int>? SelectedNumbers { get; set; } = new();
    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantEmail { get; set; } = string.Empty;
    public string? ParticipantPhone { get; set; }
    public decimal Amount { get; set; }
    public List<int>? Numbers { get; set; } 
}

public class PaymentResponseDto
{
    public string PaymentUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsSuccess { get; set; } 
    public Guid? PaymentId { get; set; } 
    public string? CheckoutUrl { get; set; } 
    public string? Message { get; set; } 
}

public class ReserveNumbersRequest
{
    public List<int>? Numbers { get; set; } = new();
    public string ParticipantEmail { get; set; } = string.Empty;
}

public class CreateRaffleRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal PriceFor1 { get; set; }
    public decimal PriceFor2 { get; set; }
    public decimal PriceFor3 { get; set; }
    public bool IsActive { get; set; } 
}

public class UpdatePricesRequest
{
    public decimal PriceFor1 { get; set; }
    public decimal PriceFor2 { get; set; }
    public decimal PriceFor3 { get; set; }
}

public class PaymentWebhookDto
{
    public Guid PaymentId { get; set; } 
    public string Status { get; set; } = string.Empty;
    public string GatewayPaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class ParticipantDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<int>? PurchasedNumbers { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime PurchaseDate { get; set; }
}

public class RaffleStatsDto
{
    public Guid RaffleId { get; set; }
    public string RaffleName { get; set; } = string.Empty;
    public int TotalNumbers { get; set; } = 100;
    public int SoldNumbers { get; set; }
    public int AvailableNumbers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalParticipants { get; set; }
    public double SalePercentage { get; set; }
    public DateTime? LastSale { get; set; }

    public decimal Revenue { get; set; } 
}