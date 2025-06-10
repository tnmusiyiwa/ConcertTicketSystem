using System.ComponentModel.DataAnnotations;
using ConcertTicketSystem.Domain.Common;
using ConcertTicketSystem.Domain.Enums;

namespace ConcertTicketSystem.Domain.Entities
{
    public class Ticket: BaseEntity
    {

        public Guid EventId { get; set; }
        
        public Guid TicketTypeId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string CustomerEmail { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string CustomerName { get; set; } = string.Empty;
        
        public TicketStatus Status { get; set; }
        
        public decimal Price { get; set; }
        
        public DateTime? ReservedAt { get; set; }
        
        public DateTime? PurchasedAt { get; set; }
        
        public DateTime? CancelledAt { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        
        [StringLength(200)]
        public string? PaymentTransactionId { get; set; }
        
        public virtual Event Event { get; set; } = null!;
        public virtual TicketType TicketType { get; set; } = null!;
    }
}
