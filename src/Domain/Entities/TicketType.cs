using System.ComponentModel.DataAnnotations;

namespace ConcertTicketSystem.Domain.Entities
{
    public class TicketType
    {
        public Guid Id { get; set; }
        
        public Guid EventId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public decimal Price { get; set; }
        
        public int TotalQuantity { get; set; }
        
        public int AvailableQuantity { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual Event Event { get; set; } = null!;
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
