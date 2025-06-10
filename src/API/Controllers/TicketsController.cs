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
            var tickets = await _ticketService.GetAllTicketsAsync();
            return Ok(tickets);
        }

        /// <summary>
        /// Get ticket by ID
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Ticket details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketDto>> GetTicket(Guid id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound($"Ticket with ID {id} not found");
            }
            return Ok(ticket);
        }

        /// <summary>
        /// Get tickets for a specific event
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of tickets for the event</returns>
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByEvent(Guid eventId)
        {
            var tickets = await _ticketService.GetTicketsByEventIdAsync(eventId);
            return Ok(tickets);
        }

        /// <summary>
        /// Get tickets for a specific customer
        /// </summary>
        /// <param name="email">Customer email</param>
        /// <returns>List of tickets for the customer</returns>
        [HttpGet("customer/{email}")]
        public async Task<ActionResult<IEnumerable<TicketDto>>> GetTicketsByCustomer(string email)
        {
            var tickets = await _ticketService.GetTicketsByCustomerEmailAsync(email);
            return Ok(tickets);
        }

        /// <summary>
        /// Reserve a ticket
        /// </summary>
        /// <param name="reserveTicketDto">Ticket reservation data</param>
        /// <returns>Reserved ticket</returns>
        [HttpPost("reserve")]
        public async Task<ActionResult<TicketDto>> ReserveTicket([FromBody] ReserveTicketDto reserveTicketDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reservedTicket = await _ticketService.ReserveTicketAsync(reserveTicketDto);
            return CreatedAtAction(nameof(GetTicket), new { id = reservedTicket.Id }, reservedTicket);
        }

        /// <summary>
        /// Purchase a reserved ticket
        /// </summary>
        /// <param name="purchaseTicketDto">Ticket purchase data</param>
        /// <returns>Purchased ticket</returns>
        [HttpPost("purchase")]
        public async Task<ActionResult<TicketDto>> PurchaseTicket([FromBody] PurchaseTicketDto purchaseTicketDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchasedTicket = await _ticketService.PurchaseTicketAsync(purchaseTicketDto);
            return Ok(purchasedTicket);
        }

        /// <summary>
        /// Cancel a ticket
        /// </summary>
        /// <param name="id">Ticket ID</param>
        /// <returns>Cancelled ticket</returns>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<TicketDto>> CancelTicket(Guid id)
        {
            var cancelledTicket = await _ticketService.CancelTicketAsync(id);
            return Ok(cancelledTicket);
        }

        /// <summary>
        /// Check ticket availability for a ticket type
        /// </summary>
        /// <param name="ticketTypeId">Ticket type ID</param>
        /// <returns>Ticket availability information</returns>
        [HttpGet("availability/{ticketTypeId}")]
        public async Task<ActionResult<TicketAvailabilityDto>> GetTicketAvailability(Guid ticketTypeId)
        {
            var availability = await _ticketService.GetTicketAvailabilityAsync(ticketTypeId);
            return Ok(availability);
        }
    }
}
