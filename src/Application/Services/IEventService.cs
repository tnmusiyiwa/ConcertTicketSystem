using ConcertTicketSystem.Application.DTOs;

namespace ConcertTicketSystem.Application.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
        Task<EventDto?> GetEventByIdAsync(Guid id);
        Task<EventDto> CreateEventAsync(CreateEventDto createEventDto);
        Task<EventDto> UpdateEventAsync(Guid id, UpdateEventDto updateEventDto);
        Task DeleteEventAsync(Guid id);
    }
}
