using System.Text.RegularExpressions;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Domain.Exceptions;

namespace ConcertTicketSystem.Application.Validators
{
    public static class ReserveTicketDtoValidator
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static void Validate(ReserveTicketDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            // Ticket Type ID validation
            if (dto.TicketTypeId == Guid.Empty)
            {
                errors.AddError(nameof(dto.TicketTypeId), "Ticket type ID is required");
            }

            // Customer email validation
            if (string.IsNullOrWhiteSpace(dto.CustomerEmail))
            {
                errors.AddError(nameof(dto.CustomerEmail), "Customer email is required");
            }
            else if (dto.CustomerEmail.Length > 200)
            {
                errors.AddError(nameof(dto.CustomerEmail), "Customer email cannot exceed 200 characters");
            }
            else if (!EmailRegex.IsMatch(dto.CustomerEmail))
            {
                errors.AddError(nameof(dto.CustomerEmail), "Customer email is not in a valid format");
            }

            // Customer name validation
            if (string.IsNullOrWhiteSpace(dto.CustomerName))
            {
                errors.AddError(nameof(dto.CustomerName), "Customer name is required");
            }
            else if (dto.CustomerName.Length > 200)
            {
                errors.AddError(nameof(dto.CustomerName), "Customer name cannot exceed 200 characters");
            }
            else if (dto.CustomerName.Length < 2)
            {
                errors.AddError(nameof(dto.CustomerName), "Customer name must be at least 2 characters long");
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }
        }
    }
}
