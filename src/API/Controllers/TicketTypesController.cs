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
            try
            {
                var ticketTypes = await _ticketTypeService.GetAllTicketTypesAsync();
                return Ok(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all ticket types");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get ticket type by ID
        /// </summary>
        /// <param name="id">Ticket type ID</param>
        /// <returns>Ticket type details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TicketTypeDto>> GetTicketType(Guid id)
        {
            try
            {
                var ticketType = await _ticketTypeService.GetTicketTypeByIdAsync(id);
                if (ticketType == null)
                {
                    return NotFound($"Ticket type with ID {id} not found");
                }
                return Ok(ticketType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket type with ID: {TicketTypeId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get ticket types for a specific event
        /// </summary>
        /// <param name="eventId">Event ID</param>
        /// <returns>List of ticket types for the event</returns>
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<TicketTypeDto>>> GetTicketTypesByEvent(Guid eventId)
        {
            try
            {
                var ticketTypes = await _ticketTypeService.GetTicketTypesByEventIdAsync(eventId);
                return Ok(ticketTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting ticket types for event ID: {EventId}", eventId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new ticket type
        /// </summary>
        /// <param name="createTicketTypeDto">Ticket type creation data</param>
        /// <returns>Created ticket type</returns>
        [HttpPost]
        public async Task<ActionResult<TicketTypeDto>> CreateTicketType([FromBody] CreateTicketTypeDto createTicketTypeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdTicketType = await _ticketTypeService.CreateTicketTypeAsync(createTicketTypeDto);
                return CreatedAtAction(nameof(GetTicketType), new { id = createdTicketType.Id }, createdTicketType);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating ticket type");
                return StatusCode(500, "An error occurred while processing your request");
            }
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
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedTicketType = await _ticketTypeService.UpdateTicketTypeAsync(id, updateTicketTypeDto);
                return Ok(updatedTicketType);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating ticket type with ID: {TicketTypeId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete a ticket type
        /// </summary>
        /// <param name="id">Ticket type ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketType(Guid id)
        {
            try
            {
                await _ticketTypeService.DeleteTicketTypeAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting ticket type with ID: {TicketTypeId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
