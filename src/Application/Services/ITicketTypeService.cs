using ConcertTicketSystem.Application.DTOs;

namespace ConcertTicketSystem.Application.Services
{
    public interface ITicketTypeService
    {
        Task<IEnumerable<TicketTypeDto>> GetAllTicketTypesAsync();
        Task<TicketTypeDto?> GetTicketTypeByIdAsync(Guid id);
        Task<IEnumerable<TicketTypeDto>> GetTicketTypesByEventIdAsync(Guid eventId);
        Task<TicketTypeDto> CreateTicketTypeAsync(CreateTicketTypeDto createTicketTypeDto);
        Task<TicketTypeDto> UpdateTicketTypeAsync(Guid id, UpdateTicketTypeDto updateTicketTypeDto);
        Task DeleteTicketTypeAsync(Guid id);
    }
}
