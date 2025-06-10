using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketTypesController : ControllerBase
    {
        private readonly ITicketTypeService _ticketTypeService;
        private readonly ILogger<TicketTypesController> _logger;

        public TicketTypesController(ITicketTypeService ticketTypeService, ILogger<TicketTypesController> logger)
        {
            _ticketTypeService = ticketTypeService;
            _logger = logger;
        }

        /// <summary>
        /// Get all ticket types
        /// </summary>
        /// <returns>List of all ticket types</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetAllTicketTypes()
        {
            var ticketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
            return Ok(ticketTypes);
        }

        /// <summary>
        /// Get ticket type by ID
        /// </summary>
        /// <param name="id">Ticket type ID</param>
        /// <returns>Ticket type details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketTypeDto>> GetTicketType(Guid id)
        {
            var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
            if (ticketType == null)
            {
                return NotFound($"Ticket type with ID {id} not found");
            }
            return Ok(ticketType);
        }

        /// <summary>
        /// Get ticket types for a specific event
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of ticket types for the event</returns>
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetTicketTypesByEvent(Guid eventId)
        {
            var ticketTypes = await _ticketTypeService.GetTicketTypesByEventIdAsync(eventId);
            return Ok(ticketTypes);
        }

        /// <summary>
        /// Create a new ticket type
        /// </summary>
        /// <param name="createTicketTypeDto">Ticket type creation data</param>
        /// <returns>Created ticket type</returns>
        [HttpPost]
        public async Task<ActionResult<TicketTypeDto>> CreateTicketType([FromBody] CreateTicketTypeDto createTicketTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdTicketType = await _ticketTypeService.CreateTicketTypeAsync(createTicketTypeDto);
            return CreatedAtAction(nameof(GetTicketType), new { id = createdTicketType.Id }, createdTicketType);
        }

        /// <summary>
        /// Update an existing ticket type
        /// </summary>
        /// <param name="id">Ticket type ID</param>
        /// <param name="updateTicketTypeDto">Ticket type update data</param>
        /// <returns>Updated ticket type</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<TicketTypeDto>> UpdateTicketType(Guid id, [FromBody] UpdateTicketTypeDto updateTicketTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedTicketType = await _ticketTypeService.UpdateTicketTypeAsync(id, updateTicketTypeDto);
            return Ok(updatedTicketType);
        }

        /// <summary>
        /// Delete a ticket type
        /// </summary>
        /// <param name="id">Ticket type ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketType(Guid id)
        {
            await _ticketTypeService.DeleteTicketTypeAsync(id);
            return NoContent();
        }
    }
}
