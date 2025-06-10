using System.ComponentModel.DataAnnotations;
using ConcertTicketSystem.Domain.Common;

namespace ConcertTicketSystem.Domain.Entities
{
    public class Event: BaseEntity
    {
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
        
        public virtual ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
