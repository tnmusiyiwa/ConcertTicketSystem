using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Domain.Exceptions;

namespace ConcertTicketSystem.Application.Validators
{
    public static class CreateTicketTypeDtoValidator
    {
        public static void Validate(CreateTicketTypeDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            // Event ID validation
            if (dto.EventId == Guid.Empty)
            {
                errors.AddError(nameof(dto.EventId), "Event ID is required");
            }

            // Name validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                errors.AddError(nameof(dto.Name), "Ticket type name is required");
            }
            else if (dto.Name.Length > 100)
            {
                errors.AddError(nameof(dto.Name), "Ticket type name cannot exceed 100 characters");
            }

            // Description validation
            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 500)
            {
                errors.AddError(nameof(dto.Description), "Description cannot exceed 500 characters");
            }

            // Price validation
            if (dto.Price <= 0)
            {
                errors.AddError(nameof(dto.Price), "Price must be greater than 0");
            }
            else if (dto.Price > 999999.99m)
            {
                errors.AddError(nameof(dto.Price), "Price cannot exceed $999,999.99");
            }

            // Quantity validation
            if (dto.TotalQuantity <= 0)
            {
                errors.AddError(nameof(dto.TotalQuantity), "Total quantity must be greater than 0");
            }
            else if (dto.TotalQuantity > 1000000)
            {
                errors.AddError(nameof(dto.TotalQuantity), "Total quantity cannot exceed 1,000,000");
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }
        }
    }
}
