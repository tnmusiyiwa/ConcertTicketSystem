using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Enums;

namespace ConcertTicketSystem.Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(Guid id);
        Task<IEnumerable<Ticket>> GetByEventIdAsync(Guid eventId);
        Task<IEnumerable<Ticket>> GetByCustomerEmailAsync(string customerEmail);
        Task<IEnumerable<Ticket>> GetExpiredReservationsAsync();
        Task<Ticket> CreateAsync(Ticket ticket);
        Task<Ticket> UpdateAsync(Ticket ticket);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<int> GetAvailableQuantityAsync(Guid ticketTypeId);
    }
}
