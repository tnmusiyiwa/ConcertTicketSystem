using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Interfaces;
using ConcertTicketSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketSystem.Infrastructure.Repositories
{
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketType>> GetAllAsync()
        {
            return await _context.TicketTypes
                .Include(tt => tt.Event)
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<TicketType?> GetByIdAsync(Guid id)
        {
            return await _context.TicketTypes
                .Include(tt => tt.Event)
                .FirstOrDefaultAsync(tt => tt.Id == id);
        }

        public async Task<IEnumerable<TicketType>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.TicketTypes
                .Include(tt => tt.Event)
                .Where(tt => tt.EventId == eventId)
                .OrderBy(tt => tt.Price)
                .ToListAsync();
        }

        public async Task<TicketType> CreateAsync(TicketType ticketType)
        {
            _context.TicketTypes.Add(ticketType);
            return ticketType;
        }

        public async Task<TicketType> UpdateAsync(TicketType ticketType)
        {
            _context.TicketTypes.Update(ticketType);
            return ticketType;
        }

        public async Task DeleteAsync(Guid id)
        {
            var ticketType = await _context.TicketTypes.FindAsync(id);
            if (ticketType != null)
            {
                _context.TicketTypes.Remove(ticketType);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.TicketTypes.AnyAsync(tt => tt.Id == id);
        }
    }
}
