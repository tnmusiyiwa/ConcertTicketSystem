using ConcertTicketSystem.Domain.Entities;

namespace ConcertTicketSystem.Domain.Interfaces
{
    public interface ITicketTypeRepository
    {
        Task<IEnumerable<TicketType>> GetAllAsync();
        Task<TicketType?> GetByIdAsync(Guid id);
        Task<IEnumerable<TicketType>> GetByEventIdAsync(Guid eventId);
        Task<TicketType> CreateAsync(TicketType ticketType);
        Task<TicketType> UpdateAsync(TicketType ticketType);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
