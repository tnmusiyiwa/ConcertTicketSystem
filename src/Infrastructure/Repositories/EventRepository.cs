using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Interfaces;
using ConcertTicketSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketSystem.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .OrderBy(e => e.EventDate)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _context.Events
                .Include(e => e.TicketTypes)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> CreateAsync(Event eventEntity)
        {
            _context.Events.Add(eventEntity);
            return eventEntity;
        }

        public async Task<Event> UpdateAsync(Event eventEntity)
        {
            _context.Events.Update(eventEntity);
            return eventEntity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var eventEntity = await _context.Events.FindAsync(id);
            if (eventEntity != null)
            {
                _context.Events.Remove(eventEntity);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }
    }
}
