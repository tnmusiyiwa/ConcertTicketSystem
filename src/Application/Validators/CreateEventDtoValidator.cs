using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Domain.Exceptions;

namespace ConcertTicketSystem.Application.Validators
{
    public static class CreateEventDtoValidator
    {
        public static void Validate(CreateEventDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            // Name validation
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                errors.AddError(nameof(dto.Name), "Event name is required");
            }
            else if (dto.Name.Length > 200)
            {
                errors.AddError(nameof(dto.Name), "Event name cannot exceed 200 characters");
            }

            // Venue validation
            if (string.IsNullOrWhiteSpace(dto.Venue))
            {
                errors.AddError(nameof(dto.Venue), "Venue is required");
            }
            else if (dto.Venue.Length > 200)
            {
                errors.AddError(nameof(dto.Venue), "Venue cannot exceed 200 characters");
            }

            // Description validation
            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
            {
                errors.AddError(nameof(dto.Description), "Description cannot exceed 1000 characters");
            }

            // Event date validation
            if (dto.EventDate.Kind != DateTimeKind.Utc)
            {
                errors.AddError(nameof(dto.EventDate), "Event date must be in UTC format");
            }
            else if (dto.EventDate <= DateTime.UtcNow)
            {
                errors.AddError(nameof(dto.EventDate), "Event date must be in the future");
            }
            else if (dto.EventDate > DateTime.UtcNow.AddYears(5))
            {
                errors.AddError(nameof(dto.EventDate), "Event date cannot be more than 5 years in the future");
            }

            // Capacity validation
            if (dto.TotalCapacity <= 0)
            {
                errors.AddError(nameof(dto.TotalCapacity), "Total capacity must be greater than 0");
            }
            else if (dto.TotalCapacity > 1000000)
            {
                errors.AddError(nameof(dto.TotalCapacity), "Total capacity cannot exceed 1,000,000");
            }

            if (errors.Any())
            {
                throw new ValidationException(errors);
            }
        }
    }

    public static class ValidationExtensions
    {
        public static void AddError(this Dictionary<string, List<string>> errors, string field, string message)
        {
            if (!errors.ContainsKey(field))
            {
                errors[field] = new List<string>();
            }
            errors[field].Add(message);
        }
    }
}
