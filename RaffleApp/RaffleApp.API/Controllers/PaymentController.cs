using Microsoft.AspNetCore.Mvc;
using RaffleApp.Core.Services;

namespace RaffleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IRaffleService _raffleService;
    private readonly IPaymentService _paymentService;

    public PaymentController(IRaffleService raffleService, IPaymentService paymentService)
    {
        _raffleService = raffleService;
        _paymentService = paymentService;
    }

    /// <summary>
    /// Inicia el proceso de pago
    /// </summary>
    /// <param name="request">Datos del pago</param>
    /// <returns>URL de la pasarela de pago</returns>
    [HttpPost("initiate")]
    public async Task<ActionResult<PaymentResponseDto>> InitiatePayment(PaymentRequestDto request)
    {
        // Validaciones
        if (request.SelectedNumbers == null || !request.SelectedNumbers.Any())
        {
            return BadRequest("Debe seleccionar al menos un número");
        }

        if (request.SelectedNumbers.Count > 3)
        {
            return BadRequest("Máximo 3 números por compra");
        }

        if (string.IsNullOrEmpty(request.ParticipantName))
        {
            return BadRequest("Nombre del participante es requerido");
        }

        if (string.IsNullOrEmpty(request.ParticipantEmail))
        {
            return BadRequest("Email del participante es requerido");
        }

        // Verificar que el precio sea correcto
        var expectedPrice = await _raffleService.CalculatePriceAsync(request.RaffleId, request.SelectedNumbers.Count);
        if (Math.Abs(request.Amount - expectedPrice) > 0.01m)
        {
            return BadRequest($"El monto no coincide. Esperado: {expectedPrice}, Recibido: {request.Amount}");
        }

        try
        {
            var paymentResponse = await _paymentService.CreatePaymentAsync(request);
            return Ok(paymentResponse);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }

    /// <summary>
    /// Confirma un pago completado
    /// </summary>
    /// <param name="paymentId">ID del pago</param>
    /// <returns>Confirmación</returns>
    [HttpPost("confirm/{paymentId}")]
    public async Task<ActionResult> ConfirmPayment(Guid paymentId)
    {
        var success = await _raffleService.ConfirmPurchaseAsync(paymentId);
        
        if (!success)
        {
            return NotFound("Pago no encontrado");
        }

        return Ok(new { message = "Pago confirmado exitosamente", paymentId });
    }

    /// <summary>
    /// Webhook para recibir notificaciones de la pasarela de pago
    /// </summary>
    /// <param name="notification">Datos de la notificación</param>
    /// <returns>Confirmación</returns>
    [HttpPost("webhook")]
    public async Task<ActionResult> PaymentWebhook([FromBody] PaymentWebhookDto notification)
    {
        try
        {
            await _paymentService.ProcessWebhookAsync(notification);
            return Ok();
        }
        catch (Exception ex)
        {
            // Log error but return OK to avoid retries from payment gateway
            Console.WriteLine($"Webhook error: {ex.Message}");
            return Ok();
        }
    }

    /// <summary>
    /// Obtiene el estado de un pago
    /// </summary>
    /// <param name="paymentId">ID del pago</param>
    /// <returns>Estado del pago</returns>
    [HttpGet("{paymentId}/status")]
    public async Task<ActionResult> GetPaymentStatus(Guid paymentId)
    {
        var status = await _paymentService.GetPaymentStatusAsync(paymentId);
        
        if (status == null)
        {
            return NotFound("Pago no encontrado");
        }

        return Ok(status);
    }
}