using System.ComponentModel.DataAnnotations;

namespace ConcertTicketSystem.Application.DTOs
{
    public class EventDto
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
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public List<TicketTypeDto> TicketTypes { get; set; } = new();
    }

    public class CreateEventDto
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
        
        [Range(1, int.MaxValue)]
        public int TotalCapacity { get; set; }
    }

    public class UpdateEventDto
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
        
        [Range(1, int.MaxValue)]
        public int TotalCapacity { get; set; }
        
        public bool IsActive { get; set; }
    }
}
