using Microsoft.EntityFrameworkCore;
using RaffleApp.Core.Data; // Assuming ApplicationDbContext is here
using RaffleApp.Core.Models; // Assuming PaymentRequestDto, PaymentResponseDto, PaymentWebhookDto, PaymentStatusDto, etc., are here
using System;
using System.Threading.Tasks;
using System.Linq; // For .Any(), .Where(), etc.

namespace RaffleApp.Core.Services; // Ensure this namespace matches your other services

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _context;

    public PaymentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentRequestDto request)
    {
        // This is a placeholder. In a real app, you'd integrate with a payment gateway (e.g., Stripe, Mercado Pago).
        // For now, we'll simulate a payment and reserve numbers.

        // 1. Validate numbers and reserve them (similar to RaffleService.ReserveNumbersAsync)
        var raffleNumbersToReserve = await _context.RaffleNumbers
            .Where(rn => rn.RaffleId == request.RaffleId && request.Numbers.Contains(rn.Number) && rn.IsAvailable)
            .ToListAsync();

        if (raffleNumbersToReserve.Count != request.Numbers.Count)
        {
            // Some numbers were not available, or request had duplicates/invalid numbers
            return new PaymentResponseDto { IsSuccess = false, Message = "One or more numbers are not available." };
        }

        foreach (var raffleNumber in raffleNumbersToReserve)
        {
            raffleNumber.ReservedAt = DateTime.Now;
            raffleNumber.ParticipantEmail = request.ParticipantEmail; // Link reservation to email
            // You might set a reservation expiry here
        }

        // 2. Create a new Payment record (pending status)
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            RaffleId = request.RaffleId,
            Amount = request.Amount,
            Currency = "ARS", // Or whatever your currency is
            Status = PaymentStatus.Pending,
            PaymentGatewayId = "SIMULATED_PAYMENT", // In a real app, this comes from the gateway
            ParticipantName = request.ParticipantName,
            ParticipantEmail = request.ParticipantEmail,
            ParticipantPhone = request.ParticipantPhone,
            CreatedAt = DateTime.Now,
            // Link numbers to payment
            RaffleNumbers = raffleNumbersToReserve
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        // 3. Return a response with a "checkout URL" or similar for the frontend
        return new PaymentResponseDto
        {
            IsSuccess = true,
            PaymentId = payment.Id,
            CheckoutUrl = $"https://simulated-payment-gateway.com/checkout/{payment.Id}", // Placeholder URL
            Message = "Payment initiated. Please complete checkout."
        };
    }

    public async Task ProcessWebhookAsync(PaymentWebhookDto notification)
    {
        // This method would be called by your payment gateway when a payment status changes.
        // You'd verify the webhook signature, then process the notification.

        // Example: Assume `notification.PaymentId` and `notification.Status` are reliable
        var payment = await _context.Payments
            .Include(p => p.RaffleNumbers)
            .FirstOrDefaultAsync(p => p.Id == notification.PaymentId);

        if (payment == null) return; // Payment not found in your system

        if (notification.Status == "completed" && payment.Status == PaymentStatus.Pending)
        {
            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.Now;

            foreach (var raffleNumber in payment.RaffleNumbers)
            {
                raffleNumber.IsAvailable = false; // Mark as sold
                raffleNumber.PurchasedAt = DateTime.Now;
                raffleNumber.ParticipantName = payment.ParticipantName;
                raffleNumber.ParticipantEmail = payment.ParticipantEmail;
                raffleNumber.ParticipantPhone = payment.ParticipantPhone;
                raffleNumber.ReservedAt = null; // Clear reservation after purchase
            }
        }
        else if (notification.Status == "failed" || notification.Status == "cancelled")
        {
            payment.Status = PaymentStatus.Failed;
            // Optionally, un-reserve numbers if payment failed after a reservation period
            foreach (var raffleNumber in payment.RaffleNumbers)
            {
                if (!raffleNumber.IsAvailable && raffleNumber.ReservedAt.HasValue && !raffleNumber.PurchasedAt.HasValue)
                {
                    raffleNumber.ReservedAt = null; // Un-reserve if it was only reserved, not purchased
                    raffleNumber.ParticipantEmail = null; // Clear participant data for this number
                    // You might need a mechanism to make these numbers available again, or put them back into a pool
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<PaymentStatusDto?> GetPaymentStatusAsync(Guid paymentId)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);

        if (payment == null) return null;

        return new PaymentStatusDto
        {
            PaymentId = payment.Id,
            Status = payment.Status.ToString(), // Convert enum to string
            Amount = payment.Amount,
            CompletedAt = payment.CompletedAt
        };
    }
}