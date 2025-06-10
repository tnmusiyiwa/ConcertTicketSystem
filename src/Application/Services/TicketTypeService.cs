using AutoMapper;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcertTicketSystem.Application.Services
{
    public class TicketTypeService : ITicketTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketTypeService> _logger;

        public TicketTypeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketTypeService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketTypeDto>> GetAllTicketTypesAsync()
        {
            try
            {
                var ticketTypes = await _unitOfWork.TicketTypes.GetAllAsync();
                return _mapper.Map<IEnumerable<TicketTypeDto>>(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all ticket types");
                throw;
            }
        }

        public async Task<TicketTypeDto?> GetTicketTypeByIdAsync(Guid id)
        {
            try
            {
                var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(id);
                return ticketType == null ? null : _mapper.Map<TicketTypeDto>(ticketType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket type with ID: {TicketTypeId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TicketTypeDto>> GetTicketTypesByEventIdAsync(Guid eventId)
        {
            try
            {
                var ticketTypes = await _unitOfWork.TicketTypes.GetByEventIdAsync(eventId);
                return _mapper.Map<IEnumerable<TicketTypeDto>>(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket types for event ID: {EventId}", eventId);
                throw;
            }
        }

        public async Task<TicketTypeDto> CreateTicketTypeAsync(CreateTicketTypeDto createTicketTypeDto)
        {
            try
            {
                // Verify event exists
                var eventExists = await _unitOfWork.Events.ExistsAsync(createTicketTypeDto.EventId);
                if (!eventExists)
                {
                    throw new ArgumentException($"Event with ID {createTicketTypeDto.EventId} not found");
                }

                var ticketType = _mapper.Map<TicketType>(createTicketTypeDto);
                ticketType.Id = Guid.NewGuid();
                ticketType.AvailableQuantity = ticketType.TotalQuantity;
                ticketType.CreatedAt = DateTime.UtcNow;
                ticketType.UpdatedAt = DateTime.UtcNow;

                var createdTicketType = await _unitOfWork.TicketTypes.CreateAsync(ticketType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Ticket type created successfully with ID: {TicketTypeId}", createdTicketType.Id);
                return _mapper.Map<TicketTypeDto>(createdTicketType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ticket type");
                throw;
            }
        }

        public async Task<TicketTypeDto> UpdateTicketTypeAsync(Guid id, UpdateTicketTypeDto updateTicketTypeDto)
        {
            try
            {
                var existingTicketType = await _unitOfWork.TicketTypes.GetByIdAsync(id);
                if (existingTicketType == null)
                {
                    throw new ArgumentException($"Ticket type with ID {id} not found");
                }

                _mapper.Map(updateTicketTypeDto, existingTicketType);
                existingTicketType.UpdatedAt = DateTime.UtcNow;

                var updatedTicketType = await _unitOfWork.TicketTypes.UpdateAsync(existingTicketType);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Ticket type updated successfully with ID: {TicketTypeId}", id);
                return _mapper.Map<TicketTypeDto>(updatedTicketType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ticket type with ID: {TicketTypeId}", id);
                throw;
            }
        }

        public async Task DeleteTicketTypeAsync(Guid id)
        {
            try
            {
                var ticketTypeExists = await _unitOfWork.TicketTypes.ExistsAsync(id);
                if (!ticketTypeExists)
                {
                    throw new ArgumentException($"Ticket type with ID {id} not found");
                }

                await _unitOfWork.TicketTypes.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Ticket type deleted successfully with ID: {TicketTypeId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ticket type with ID: {TicketTypeId}", id);
                throw;
            }
        }
    }
}
