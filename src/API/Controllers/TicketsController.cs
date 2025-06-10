using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        /// <summary>
        /// Get all tickets
        /// </summary>
        /// <returns>List of all tickets</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetAllTickets()
        {
            try
            {
                var tickets = await _ticketService.GetAllTicketsAsync();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all tickets");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get ticket by ID
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Ticket details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            try
            {
                var ticket = await _ticketService.GetTicketByIdAsync(id);
                if (ticket == null)
                {
                    return NotFound($"Ticket with ID {id} not found");
                }
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket with ID: {TicketId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get tickets for a specific event
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of tickets for the event</returns>
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByEvent(Guid eventId)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByEventIdAsync(eventId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tickets for event ID: {EventId}", eventId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get tickets for a specific customer
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <returns>List of tickets for the customer</returns>
        [HttpGet("customer/{email}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByCustomer(string email)
        {
            try
            {
                var tickets = await _ticketService.GetTicketsByCustomerEmailAsync(email);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tickets for customer: {CustomerEmail}", email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Reserve a ticket
        /// </summary>
        /// <param name="reserveTicketDto">Ticket reservation data</param>
        /// <returns>Reserved ticket</returns>
        [HttpPost("reserve")]
        public async Task<ActionResult<TicketDto>> ReserveTicket([FromBody] ReserveTicketDto reserveTicketDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var reservedTicket = await _ticketService.ReserveTicketAsync(reserveTicketDto);
                return CreatedAtAction(nameof(GetTicket), new { id = reservedTicket.Id }, reservedTicket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while reserving ticket");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Purchase a reserved ticket
        /// </summary>
        /// <param name="purchaseTicketDto">Ticket purchase data</param>
        /// <returns>Purchased ticket</returns>
        [HttpPost("purchase")]
        public async Task<ActionResult<TicketDto>> PurchaseTicket([FromBody] PurchaseTicketDto purchaseTicketDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var purchasedTicket = await _ticketService.PurchaseTicketAsync(purchaseTicketDto);
                return Ok(purchasedTicket);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while purchasing ticket");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Cancel a ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Cancelled ticket</returns>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<TicketDto>> CancelTicket(Guid id)
        {
            try
            {
                var cancelledTicket = await _ticketService.CancelTicketAsync(id);
                return Ok(cancelledTicket);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cancelling ticket with ID: {TicketId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Check ticket availability for a ticket type
        /// </summary>
        /// <param name="ticketTypeId">Ticket type ID</param>
        /// <returns>Ticket availability information</returns>
        [HttpGet("availability/{ticketTypeId}")]
        public async Task<ActionResult<TicketAvailabilityDto>> GetTicketAvailability(Guid ticketTypeId)
        {
            try
            {
                var availability = await _ticketService.GetTicketAvailabilityAsync(ticketTypeId);
                return Ok(availability);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket availability for ticket type ID: {TicketTypeId}", ticketTypeId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
