using Microsoft.AspNetCore.Mvc;
using RaffleApp.Core.Services;

namespace RaffleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RaffleController : ControllerBase
{
    private readonly IRaffleService _raffleService;

    public RaffleController(IRaffleService raffleService)
    {
        _raffleService = raffleService;
    }

    /// <summary>
    /// Obtiene los detalles de una rifa específica
    /// </summary>
    /// <param name="id">ID de la rifa</param>
    /// <returns>Datos completos de la rifa</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<RaffleDto>> GetRaffle(Guid id)
    {
        var raffle = await _raffleService.GetRaffleByIdAsync(id);
        
        if (raffle == null)
        {
            return NotFound($"Rifa con ID {id} no encontrada");
        }

        return Ok(raffle);
    }

    /// <summary>
    /// Obtiene todos los números de una rifa (disponibles y ocupados)
    /// </summary>
    /// <param name="id">ID de la rifa</param>
    /// <returns>Lista de todos los números con su estado</returns>
    [HttpGet("{id}/numbers")]
    public async Task<ActionResult<List<RaffleNumberDto>>> GetAllNumbers(Guid id)
    {
        var numbers = await _raffleService.GetAllNumbersAsync(id);
        return Ok(numbers);
    }

    /// <summary>
    /// Obtiene solo los números disponibles de una rifa
    /// </summary>
    /// <param name="id">ID de la rifa</param>
    /// <returns>Lista de números disponibles</returns>
    [HttpGet("{id}/numbers/available")]
    public async Task<ActionResult<List<RaffleNumberDto>>> GetAvailableNumbers(Guid id)
    {
        var numbers = await _raffleService.GetAvailableNumbersAsync(id);
        return Ok(numbers);
    }

    /// <summary>
    /// Calcula el precio total según la cantidad de números seleccionados
    /// </summary>
    /// <param name="id">ID de la rifa</param>
    /// <param name="quantity">Cantidad de números (1-3)</param>
    /// <returns>Precio total</returns>
    [HttpGet("{id}/price/{quantity}")]
    public async Task<ActionResult<decimal>> CalculatePrice(Guid id, int quantity)
    {
        if (quantity < 1 || quantity > 3)
        {
            return BadRequest("La cantidad debe ser entre 1 y 3");
        }

        var price = await _raffleService.CalculatePriceAsync(id, quantity);
        
        if (price == 0)
        {
            return NotFound("Configuración de precios no encontrada");
        }

        return Ok(new { quantity, price, raffleId = id });
    }

    /// <summary>
    /// Reserva números temporalmente durante el proceso de pago
    /// </summary>
    /// <param name="id">ID de la rifa</param>
    /// <param name="request">Datos de la reserva</param>
    /// <returns>Confirmación de la reserva</returns>
    [HttpPost("{id}/reserve")]
    public async Task<ActionResult> ReserveNumbers(Guid id, ReserveNumbersRequest request)
    {
        if (request.Numbers == null || !request.Numbers.Any())
        {
            return BadRequest("Debe seleccionar al menos un número");
        }

        if (request.Numbers.Count > 3)
        {
            return BadRequest("Máximo 3 números por reserva");
        }

        if (request.Numbers.Any(n => n < 1 || n > 100))
        {
            return BadRequest("Los números deben estar entre 1 y 100");
        }

        if (string.IsNullOrEmpty(request.ParticipantEmail))
        {
            return BadRequest("Email del participante es requerido");
        }

        var success = await _raffleService.ReserveNumbersAsync(id, request.Numbers, request.ParticipantEmail);
        
        if (!success)
        {
            return Conflict("Algunos números ya no están disponibles");
        }

        return Ok(new { message = "Números reservados exitosamente", reservedNumbers = request.Numbers });
    }
}