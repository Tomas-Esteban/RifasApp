using Microsoft.EntityFrameworkCore;
using RaffleApp.Core.Data; // Assuming ApplicationDbContext is here
using RaffleApp.Core.Models; // Assuming RaffleDto, CreateRaffleRequest, etc., are here
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaffleApp.Core.Services; // Ensure this namespace matches your other services

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<RaffleDto>> GetAllRafflesAsync()
    {
        // Example implementation: Return all raffles
        // You'll need to map your Raffle entity to RaffleDto
        return _context.Raffles
            .Select(r => new RaffleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                IsActive = r.IsActive
                // Map other properties as needed
            })
            .ToListAsync();
    }

    public Task<RaffleDto> CreateRaffleAsync(CreateRaffleRequest request)
    {
        // Example implementation: Create a new raffle
        var newRaffle = new Raffle
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            // You might need to initialize PriceConfiguration and RaffleNumbers here
        };
        _context.Raffles.Add(newRaffle);
        _context.SaveChangesAsync();

        // Return the created raffle as DTO
        return Task.FromResult(new RaffleDto
        {
            Id = newRaffle.Id,
            Name = newRaffle.Name,
            Description = newRaffle.Description,
            StartDate = newRaffle.StartDate,
            EndDate = newRaffle.EndDate,
            IsActive = newRaffle.IsActive
        });
    }

    public async Task<bool> UpdatePriceConfigurationAsync(Guid raffleId, UpdatePricesRequest prices)
    {
        var priceConfig = await _context.PriceConfigurations.FirstOrDefaultAsync(pc => pc.RaffleId == raffleId);
        if (priceConfig == null) return false;

        priceConfig.PriceFor1 = prices.PriceFor1;
        priceConfig.PriceFor2 = prices.PriceFor2;
        priceConfig.PriceFor3 = prices.PriceFor3;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ParticipantDto>> GetParticipantsAsync(Guid raffleId)
    {
        // This assumes you have a way to derive participants from RaffleNumbers
        // or a dedicated Participant entity. Example uses RaffleNumbers.
        var participants = await _context.RaffleNumbers
            .Where(rn => rn.RaffleId == raffleId && !rn.IsAvailable) // Only purchased numbers
            .Select(rn => new ParticipantDto
            {
                Name = rn.ParticipantName,
                Email = rn.ParticipantEmail,
                Phone = rn.ParticipantPhone
            })
            .Distinct() // Get unique participants
            .ToListAsync();

        return participants;
    }

    public async Task<RaffleStatsDto?> GetRaffleStatsAsync(Guid raffleId)
    {
        var raffle = await _context.Raffles
            .Include(r => r.RaffleNumbers)
            .FirstOrDefaultAsync(r => r.Id == raffleId);

        if (raffle == null) return null;

        var totalNumbers = raffle.RaffleNumbers.Count;
        var availableNumbers = raffle.RaffleNumbers.Count(rn => rn.IsAvailable);
        var soldNumbers = totalNumbers - availableNumbers;

        return new RaffleStatsDto
        {
            TotalNumbers = totalNumbers,
            AvailableNumbers = availableNumbers,
            SoldNumbers = soldNumbers,
            // Calculate other stats like revenue if applicable
            Revenue = raffle.RaffleNumbers.Where(rn => !rn.IsAvailable).Sum(rn => rn.PricePaid) // Assuming PricePaid on RaffleNumber
        };
    }

    public async Task<bool> UpdateRaffleStatusAsync(Guid raffleId, bool isActive)
    {
        var raffle = await _context.Raffles.FirstOrDefaultAsync(r => r.Id == raffleId);
        if (raffle == null) return false;

        raffle.IsActive = isActive;
        await _context.SaveChangesAsync();
        return true;
    }
}