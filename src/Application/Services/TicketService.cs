using AutoMapper;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using ConcertTicketSystem.Domain.Entities;
using ConcertTicketSystem.Domain.Enums;
using ConcertTicketSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConcertTicketSystem.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;
        private readonly TimeSpan _reservationDuration = TimeSpan.FromMinutes(15);

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketDto>> GetAllTicketsAsync()
        {
            try
            {
                var tickets = await _unitOfWork.Tickets.GetAllAsync();
                return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all tickets");
                throw;
            }
        }

        public async Task<TicketDto?> GetTicketByIdAsync(Guid id)
        {
            try
            {
                var ticket = await _unitOfWork.Tickets.GetByIdAsync(id);
                return ticket == null ? null : _mapper.Map<TicketDto>(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket with ID: {TicketId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByEventIdAsync(Guid eventId)
        {
            try
            {
                var tickets = await _unitOfWork.Tickets.GetByEventIdAsync(eventId);
                return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tickets for event ID: {EventId}", eventId);
                throw;
            }
        }

        public async Task<IEnumerable<TicketDto>> GetTicketsByCustomerEmailAsync(string customerEmail)
        {
            try
            {
                var tickets = await _unitOfWork.Tickets.GetByCustomerEmailAsync(customerEmail);
                return _mapper.Map<IEnumerable<TicketDto>>(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tickets for customer: {CustomerEmail}", customerEmail);
                throw;
            }
        }

        public async Task<TicketDto> ReserveTicketAsync(ReserveTicketDto reserveTicketDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(reserveTicketDto.TicketTypeId);
                if (ticketType == null)
                {
                    throw new ArgumentException($"Ticket type with ID {reserveTicketDto.TicketTypeId} not found");
                }

                if (ticketType.AvailableQuantity <= 0)
                {
                    throw new InvalidOperationException("No tickets available for this type");
                }

                // Create reservation
                var ticket = new Ticket
                {
                    Id = Guid.NewGuid(),
                    EventId = ticketType.EventId,
                    TicketTypeId = reserveTicketDto.TicketTypeId,
                    CustomerEmail = reserveTicketDto.CustomerEmail,
                    CustomerName = reserveTicketDto.CustomerName,
                    Status = TicketStatus.Reserved,
                    Price = ticketType.Price,
                    ReservedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.Add(_reservationDuration),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Update available quantity
                ticketType.AvailableQuantity--;
                ticketType.UpdatedAt = DateTime.UtcNow;

                var createdTicket = await _unitOfWork.Tickets.CreateAsync(ticket);
                await _unitOfWork.TicketTypes.UpdateAsync(ticketType);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Ticket reserved successfully with ID: {TicketId} for customer: {CustomerEmail}", 
                    createdTicket.Id, reserveTicketDto.CustomerEmail);

                return _mapper.Map<TicketDto>(createdTicket);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error occurred while reserving ticket");
                throw;
            }
        }

        public async Task<TicketDto> PurchaseTicketAsync(PurchaseTicketDto purchaseTicketDto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var ticket = await _unitOfWork.Tickets.GetByIdAsync(purchaseTicketDto.TicketId);
                if (ticket == null)
                {
                    throw new ArgumentException($"Ticket with ID {purchaseTicketDto.TicketId} not found");
                }

                if (ticket.Status != TicketStatus.Reserved)
                {
                    throw new InvalidOperationException("Ticket is not in reserved status");
                }

                if (ticket.ExpiresAt.HasValue && ticket.ExpiresAt.Value < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Ticket reservation has expired");
                }

                // Update ticket to purchased
                ticket.Status = TicketStatus.Purchased;
                ticket.PurchasedAt = DateTime.UtcNow;
                ticket.PaymentTransactionId = purchaseTicketDto.PaymentTransactionId;
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.ExpiresAt = null; // Clear expiration

                var updatedTicket = await _unitOfWork.Tickets.UpdateAsync(ticket);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Ticket purchased successfully with ID: {TicketId}, Payment ID: {PaymentId}", 
                    ticket.Id, purchaseTicketDto.PaymentTransactionId);

                return _mapper.Map<TicketDto>(updatedTicket);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error occurred while purchasing ticket");
                throw;
            }
        }

        public async Task<TicketDto> CancelTicketAsync(Guid ticketId)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
                if (ticket == null)
                {
                    throw new ArgumentException($"Ticket with ID {ticketId} not found");
                }

                if (ticket.Status == TicketStatus.Cancelled)
                {
                    throw new InvalidOperationException("Ticket is already cancelled");
                }

                var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticket.TicketTypeId);
                if (ticketType != null)
                {
                    // Restore available quantity
                    ticketType.AvailableQuantity++;
                    ticketType.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.TicketTypes.UpdateAsync(ticketType);
                }

                // Update ticket status
                ticket.Status = TicketStatus.Cancelled;
                ticket.CancelledAt = DateTime.UtcNow;
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.ExpiresAt = null;

                var updatedTicket = await _unitOfWork.Tickets.UpdateAsync(ticket);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Ticket cancelled successfully with ID: {TicketId}", ticketId);
                return _mapper.Map<TicketDto>(updatedTicket);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error occurred while cancelling ticket with ID: {TicketId}", ticketId);
                throw;
            }
        }

        public async Task<TicketAvailabilityDto> GetTicketAvailabilityAsync(Guid ticketTypeId)
        {
            try
            {
                var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticketTypeId);
                if (ticketType == null)
                {
                    throw new ArgumentException($"Ticket type with ID {ticketTypeId} not found");
                }

                var availableQuantity = await _unitOfWork.Tickets.GetAvailableQuantityAsync(ticketTypeId);

                return new TicketAvailabilityDto
                {
                    TicketTypeId = ticketTypeId,
                    TicketTypeName = ticketType.Name,
                    AvailableQuantity = availableQuantity,
                    Price = ticketType.Price,
                    IsAvailable = availableQuantity > 0 && ticketType.IsActive
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket availability for ticket type ID: {TicketTypeId}", ticketTypeId);
                throw;
            }
        }

        public async Task CleanupExpiredReservationsAsync()
        {
            try
            {
                var expiredTickets = await _unitOfWork.Tickets.GetExpiredReservationsAsync();
                
                if (!expiredTickets.Any())
                {
                    return;
                }

                await _unitOfWork.BeginTransactionAsync();

                foreach (var ticket in expiredTickets)
                {
                    // Update ticket status to expired
                    ticket.Status = TicketStatus.Expired;
                    ticket.UpdatedAt = DateTime.UtcNow;
                    await _unitOfWork.Tickets.UpdateAsync(ticket);

                    // Restore available quantity
                    var ticketType = await _unitOfWork.TicketTypes.GetByIdAsync(ticket.TicketTypeId);
                    if (ticketType != null)
                    {
                        ticketType.AvailableQuantity++;
                        ticketType.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.TicketTypes.UpdateAsync(ticketType);
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Cleaned up {Count} expired ticket reservations", expiredTickets.Count());
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error occurred while cleaning up expired reservations");
                throw;
            }
        }
    }
}
