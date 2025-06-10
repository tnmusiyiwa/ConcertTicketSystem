using ConcertTicketSystem.Domain.Entities;

namespace ConcertTicketSystem.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(Guid id);
        Task<Event> CreateAsync(Event eventEntity);
        Task<Event> UpdateAsync(Event eventEntity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
