using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Enums;
using ConcertTicketSystem.Domain.Interfaces;
using ConcertTicketSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ConcertTicketSystem.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.TicketType)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(Guid id)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Ticket>> GetByEventIdAsync(Guid eventId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.TicketType)
                .Where(t => t.EventId == eventId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetByCustomerEmailAsync(string customerEmail)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.TicketType)
                .Where(t => t.CustomerEmail == customerEmail)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetExpiredReservationsAsync()
        {
            return await _context.Tickets
                .Where(t => t.Status == TicketStatus.Reserved && 
                           t.ExpiresAt.HasValue && 
                           t.ExpiresAt.Value < DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            return ticket;
        }

        public async Task<Ticket> UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            return ticket;
        }

        public async Task DeleteAsync(Guid id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Tickets.AnyAsync(t => t.Id == id);
        }

        public async Task<int> GetAvailableQuantityAsync(Guid ticketTypeId)
        {
            var ticketType = await _context.TicketTypes.FindAsync(ticketTypeId);
            return ticketType?.AvailableQuantity ?? 0;
        }
    }
}
