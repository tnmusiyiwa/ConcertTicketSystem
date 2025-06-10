using System.ComponentModel.DataAnnotations;
using ConcertTicketSystem.Domain.Enums;

namespace ConcertTicketSystem.Application.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid TicketTypeId { get; set; }
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public TicketStatus Status { get; set; }
        public decimal Price { get; set; }
        public DateTime? ReservedAt { get; set; }
        public DateTime? PurchasedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? PaymentTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public string EventName { get; set; } = string.Empty;
        public string TicketTypeName { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
    }

    public class ReserveTicketDto
    {
        [Required]
        public Guid TicketTypeId { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;
    }

    public class PurchaseTicketDto
    {
        [Required]
        public Guid TicketId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string PaymentTransactionId { get; set; } = string.Empty;
    }

    public class TicketAvailabilityDto
    {
        public Guid TicketTypeId { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
