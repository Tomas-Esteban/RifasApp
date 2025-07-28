using Microsoft.AspNetCore.Mvc;
using RaffleApp.Core.Services;

namespace RaffleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(Roles = "Admin")] // Descomenta cuando implementes autenticación
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IRaffleService _raffleService;

    public AdminController(IAdminService adminService, IRaffleService raffleService)
    {
        _adminService = adminService;
        _raffleService = raffleService;
    }

    /// <summary>
    /// Obtiene todas las rifas
    /// </summary>
    /// <returns>Lista de rifas</returns>
    [HttpGet("raffles")]
    public async Task<ActionResult<List<RaffleDto>>> GetAllRaffles()
    {
        var raffles = await _adminService.GetAllRafflesAsync();
        return Ok(raffles);
    }

    /// <summary>
    /// Crea una nueva rifa
    /// </summary>
    /// <param name="request">Datos de la nueva rifa</param>
    /// <returns>Rifa creada</returns>
    [HttpPost("raffles")]
    public async Task<ActionResult<RaffleDto>> CreateRaffle(CreateRaffleRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            return BadRequest("El nombre de la rifa es requerido");
        }

        if (request.EndDate <= request.StartDate)
        {
            return BadRequest("La fecha de fin debe ser posterior a la fecha de inicio");
        }

        var raffle = await _adminService.CreateRaffleAsync(request);
        return CreatedAtAction(nameof(RaffleController.GetRaffle), "Raffle", new { id = raffle.Id }, raffle);
    }

    /// <summary>
    /// Actualiza la configuración de precios de una rifa
    /// </summary>
    /// <param name="raffleId">ID de la rifa</param>
    /// <param name="prices">Nueva configuración de precios</param>
    /// <returns>Confirmación</returns>
    [HttpPut("raffles/{raffleId}/prices")]
    public async Task<ActionResult> UpdatePrices(Guid raffleId, UpdatePricesRequest prices)
    {
        if (prices.PriceFor1 <= 0 || prices.PriceFor2 <= 0 || prices.PriceFor3 <= 0)
        {
            return BadRequest("Todos los precios deben ser mayores a cero");
        }

        var success = await _adminService.UpdatePriceConfigurationAsync(raffleId, prices);
        
        if (!success)
        {
            return NotFound("Rifa no encontrada");
        }

        return Ok(new { message = "Precios actualizados exitosamente" });
    }

    /// <summary>
    /// Obtiene todos los participantes de una rifa
    /// </summary>
    /// <param name="raffleId">ID de la rifa</param>
    /// <returns>Lista de participantes</returns>
    [HttpGet("raffles/{raffleId}/participants")]
    public async Task<ActionResult<List<ParticipantDto>>> GetParticipants(Guid raffleId)
    {
        var participants = await _adminService.GetParticipantsAsync(raffleId);
        return Ok(participants);
    }

    /// <summary>
    /// Obtiene estadísticas de una rifa
    /// </summary>
    /// <param name="raffleId">ID de la rifa</param>
    /// <returns>Estadísticas</returns>
    [HttpGet("raffles/{raffleId}/stats")]
    public async Task<ActionResult<RaffleStatsDto>> GetRaffleStats(Guid raffleId)
    {
        var stats = await _adminService.GetRaffleStatsAsync(raffleId);
        
        if (stats == null)
        {
            return NotFound("Rifa no encontrada");
        }

        return Ok(stats);
    }

    /// <summary>
    /// Activa o desactiva una rifa
    /// </summary>
    /// <param name="raffleId">ID de la rifa</param>
    /// <param name="isActive">Estado activo/inactivo</param>
    /// <returns>Confirmación</returns>
    [HttpPatch("raffles/{raffleId}/status")]
    public async Task<ActionResult> UpdateRaffleStatus(Guid raffleId, [FromBody] bool isActive)
    {
        var success = await _adminService.UpdateRaffleStatusAsync(raffleId, isActive);
        
        if (!success)
        {
            return NotFound("Rifa no encontrada");
        }

        return Ok(new { message = $"Rifa {(isActive ? "activada" : "desactivada")} exitosamente" });
    }
}