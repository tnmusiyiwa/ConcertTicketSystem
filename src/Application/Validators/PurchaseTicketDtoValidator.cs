using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Domain.Exceptions;

namespace ConcertTicketSystem.Application.Validators
{
    public static class PurchaseTicketDtoValidator
    {
        public static void Validate(PurchaseTicketDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            // Ticket ID validation
            if (dto.TicketId == Guid.Empty)
            {
                errors.AddError(nameof(dto.TicketId), "Ticket ID is required");
            }

            // Payment transaction ID validation
            if (string.IsNullOrWhiteSpace(dto.PaymentTransactionId))
            {
                errors.AddError(nameof(dto.PaymentTransactionId), "Payment transaction ID is required");
            }
            else if (dto.PaymentTransactionId.Length > 200)
            {
                errors.AddError(nameof(dto.PaymentTransactionId), "Payment transaction ID cannot exceed 200 characters");
            }
            else if (dto.PaymentTransactionId.Length < 3)
            {
                errors.AddError(nameof(dto.PaymentTransactionId), "Payment transaction ID must be at least 3 characters long");
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }
        }
    }
}
