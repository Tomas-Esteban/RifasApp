namespace RaffleApp.Core.Services;

public interface IRaffleService
{
    Task<RaffleDto?> GetRaffleByIdAsync(Guid id);
    Task<List<RaffleNumberDto>> GetAvailableNumbersAsync(Guid raffleId);
    Task<List<RaffleNumberDto>> GetAllNumbersAsync(Guid raffleId);
    Task<bool> ReserveNumbersAsync(Guid raffleId, List<int> numbers, string participantEmail);
    Task<bool> ConfirmPurchaseAsync(Guid paymentId);
    Task<decimal> CalculatePriceAsync(Guid raffleId, int quantity);
}
