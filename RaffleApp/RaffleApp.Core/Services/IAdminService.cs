namespace RaffleApp.Core.Services;

    public interface  IAdminService
    {
        Task<List<RaffleDto>> GetAllRafflesAsync();
        Task<RaffleDto> CreateRaffleAsync(CreateRaffleRequest request);
        Task<bool> UpdatePriceConfigurationAsync(Guid raffleId, UpdatePricesRequest prices);
        Task<List<ParticipantDto>> GetParticipantsAsync(Guid raffleId);
        Task<RaffleStatsDto?> GetRaffleStatsAsync(Guid raffleId);
        Task<bool> UpdateRaffleStatusAsync(Guid raffleId, bool isActive);
    }