using System.ComponentModel.DataAnnotations;

namespace ConcertTicketSystem.Application.DTOs
{
    public class TicketTypeDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string EventName { get; set; } = string.Empty;
    }

    public class CreateTicketTypeDto
    {
        [Required]
        public Guid EventId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(1, int.MaxValue)]
        public int TotalQuantity { get; set; }
    }

    public class UpdateTicketTypeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(1, int.MaxValue)]
        public int TotalQuantity { get; set; }
        
        public bool IsActive { get; set; }
    }
}
