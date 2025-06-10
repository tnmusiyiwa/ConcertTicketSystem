using System.ComponentModel.DataAnnotations;

namespace ConcertTicketSystem.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string Venue { get; set; } = string.Empty;
        
        public DateTime EventDate { get; set; }
        
        public int TotalCapacity { get; set; }
        
        public int AvailableCapacity { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
