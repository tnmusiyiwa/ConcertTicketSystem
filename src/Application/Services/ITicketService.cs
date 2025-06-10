using ConcertTicketSystem.Application.DTOs;

namespace ConcertTicketSystem.Application.Services
{
    public interface ITicketService
    {
        Task<IEnumerable<TicketDto>> GetAllTicketsAsync();
        Task<TicketDto?> GetTicketByIdAsync(Guid id);
        Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(Guid eventId);
        Task<IEnumerable<TicketDto>> GetTicketsByCustomerEmailAsync(string customerEmail);
        Task<TicketDto> ReserveTicketAsync(ReserveTicketDto reserveTicketDto);
        Task<TicketDto> PurchaseTicketAsync(PurchaseTicketDto purchaseTicketDto);
        Task<TicketDto> CancelTicketAsync(Guid ticketId);
        Task<TicketAvailabilityDto> GetTicketAvailabilityAsync(Guid ticketTypeId);
        Task CleanupExpiredReservationsAsync();
    }
}
