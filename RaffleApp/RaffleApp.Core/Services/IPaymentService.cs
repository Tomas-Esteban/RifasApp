namespace RaffleApp.Core.Services;
    public interface IPaymentService
{
    Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request);
    Task ProcessWebhookAsync(PaymentWebhookDto notification);
    Task<PaymentStatusDto?> GetPaymentStatusAsync(Guid paymentId);
}    


