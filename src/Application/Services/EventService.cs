using AutoMapper;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcertTicketSystem.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<EventService> _logger;

        public EventService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EventService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<EventDto>> GetAllEventsAsync()
        {
            try
            {
                var events = await _unitOfWork.Events.GetAllAsync();
                return _mapper.Map<IEnumerable<EventDto>>(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all events");
                throw;
            }
        }

        public async Task<EventDto?> GetEventByIdAsync(Guid id)
        {
            try
            {
                var eventEntity = await _unitOfWork.Events.GetByIdAsync(id);
                return eventEntity == null ? null : _mapper.Map<EventDto>(eventEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting event with ID: {EventId}", id);
                throw;
            }
        }

        public async Task<EventDto> CreateEventAsync(CreateEventDto createEventDto)
        {
            try
            {
                createEventDto.EventDate = createEventDto.EventDate.ToUniversalTime();
                
                var eventEntity = _mapper.Map<Event>(createEventDto);
                eventEntity.Id = Guid.NewGuid();
                eventEntity.AvailableCapacity = eventEntity.TotalCapacity;
                eventEntity.CreatedAt = DateTime.UtcNow;
                eventEntity.UpdatedAt = DateTime.UtcNow;

                var createdEvent = await _unitOfWork.Events.CreateAsync(eventEntity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Event created successfully with ID: {EventId}", createdEvent.Id);
                return _mapper.Map<EventDto>(createdEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating event");
                throw;
            }
        }

        public async Task<EventDto> UpdateEventAsync(Guid id, UpdateEventDto updateEventDto)
        {
            try
            {
                var existingEvent = await _unitOfWork.Events.GetByIdAsync(id);
                if (existingEvent == null)
                {
                    throw new ArgumentException($"Event with ID {id} not found");
                }

                _mapper.Map(updateEventDto, existingEvent);
                existingEvent.UpdatedAt = DateTime.UtcNow;

                var updatedEvent = await _unitOfWork.Events.UpdateAsync(existingEvent);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Event updated successfully with ID: {EventId}", id);
                return _mapper.Map<EventDto>(updatedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating event with ID: {EventId}", id);
                throw;
            }
        }

        public async Task DeleteEventAsync(Guid id)
        {
            try
            {
                var eventExists = await _unitOfWork.Events.ExistsAsync(id);
                if (!eventExists)
                {
                    throw new ArgumentException($"Event with ID {id} not found");
                }

                await _unitOfWork.Events.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Event deleted successfully with ID: {EventId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting event with ID: {EventId}", id);
                throw;
            }
        }
    }
}
