using Microsoft.EntityFrameworkCore;
using RaffleApp.Core.Models;
using RaffleApp.Core.Data;
namespace RaffleApp.Core.Services;

public class RaffleService : IRaffleService
{
    private readonly ApplicationDbContext _context;

    public RaffleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RaffleDto?> GetRaffleByIdAsync(Guid id)
    {
        var raffle = await _context.Raffles
            .Include(r => r.PriceConfiguration)
            .Include(r => r.RaffleNumbers)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (raffle == null) return null;

        return new RaffleDto
        {
            Id = raffle.Id,
            Name = raffle.Name,
            Description = raffle.Description,
            StartDate = raffle.StartDate,
            EndDate = raffle.EndDate,
            IsActive = raffle.IsActive,
            PriceConfiguration = raffle.PriceConfiguration != null ? new PriceConfigurationDto
            {
                Id = raffle.PriceConfiguration.Id,
                PriceFor1 = raffle.PriceConfiguration.PriceFor1,
                PriceFor2 = raffle.PriceConfiguration.PriceFor2,
                PriceFor3 = raffle.PriceConfiguration.PriceFor3
            } : null,
            RaffleNumbers = raffle.RaffleNumbers.Select(rn => new RaffleNumberDto
            {
                Id = rn.Id,
                Number = rn.Number,
                IsAvailable = rn.IsAvailable,
                ParticipantName = rn.ParticipantName,
                ParticipantEmail = rn.ParticipantEmail,
                ParticipantPhone = rn.ParticipantPhone,
                PurchasedAt = rn.PurchasedAt
            }).OrderBy(rn => rn.Number).ToList()
        };
    }

    public async Task<List<RaffleNumberDto>> GetAvailableNumbersAsync(Guid raffleId)
    {
        var numbers = await _context.RaffleNumbers
            .Where(rn => rn.RaffleId == raffleId && rn.IsAvailable)
            .OrderBy(rn => rn.Number)
            .Select(rn => new RaffleNumberDto
            {
                Id = rn.Id,
                Number = rn.Number,
                IsAvailable = rn.IsAvailable
            })
            .ToListAsync();

        return numbers;
    }

    public async Task<List<RaffleNumberDto>> GetAllNumbersAsync(Guid raffleId)
    {
        var numbers = await _context.RaffleNumbers
            .Where(rn => rn.RaffleId == raffleId)
            .OrderBy(rn => rn.Number)
            .Select(rn => new RaffleNumberDto
            {
                Id = rn.Id,
                Number = rn.Number,
                IsAvailable = rn.IsAvailable,
                ParticipantName = rn.ParticipantName,
                ParticipantEmail = rn.ParticipantEmail,
                ParticipantPhone = rn.ParticipantPhone,
                PurchasedAt = rn.PurchasedAt
            })
            .ToListAsync();

        return numbers;
    }

    public async Task<bool> ReserveNumbersAsync(Guid raffleId, List<int> numbers, string participantEmail)
    {
        var raffleNumbers = await _context.RaffleNumbers
            .Where(rn => rn.RaffleId == raffleId && numbers.Contains(rn.Number) && rn.IsAvailable)
            .ToListAsync();

        if (raffleNumbers.Count != numbers.Count)
        {
            return false; // Algunos números no están disponibles
        }

        foreach (var raffleNumber in raffleNumbers)
        {
            raffleNumber.ReservedAt = DateTime.Now;
            raffleNumber.ParticipantEmail = participantEmail;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ConfirmPurchaseAsync(Guid paymentId)
    {
        var payment = await _context.Payments
            .Include(p => p.RaffleNumbers)
            .FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return false;

        payment.Status = PaymentStatus.Completed;
        payment.CompletedAt = DateTime.Now;

        foreach (var raffleNumber in payment.RaffleNumbers)
        {
            raffleNumber.IsAvailable = false;
            raffleNumber.PurchasedAt = DateTime.Now;
            raffleNumber.ParticipantName = payment.ParticipantName;
            raffleNumber.ParticipantEmail = payment.ParticipantEmail;
            raffleNumber.ParticipantPhone = payment.ParticipantPhone;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<decimal> CalculatePriceAsync(Guid raffleId, int quantity)
    {
        var priceConfig = await _context.PriceConfigurations
            .FirstOrDefaultAsync(pc => pc.RaffleId == raffleId);

        if (priceConfig == null) return 0;

        return quantity switch
        {
            1 => priceConfig.PriceFor1,
            2 => priceConfig.PriceFor2,
            3 => priceConfig.PriceFor3,
            _ => priceConfig.PriceFor1 * quantity
        };
    }
}
