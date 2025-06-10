using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConcertTicketSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <returns>List of all events</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAllEvents()
        {
            try
            {
                var events = await _eventService.GetAllEventsAsync();
                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all events");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Event details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(Guid id)
        {
            try
            {
                var eventDto = await _eventService.GetEventByIdAsync(id);
                if (eventDto == null)
                {
                    return NotFound($"Event with ID {id} not found");
                }
                return Ok(eventDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting event with ID: {EventId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <param name="createEventDto">Event creation data</param>
        /// <returns>Created event</returns>
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdEvent = await _eventService.CreateEventAsync(createEventDto);
                return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating event");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update an existing event
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <param name="updateEventDto">Event update data</param>
        /// <returns>Updated event</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EventDto>> UpdateEvent(Guid id, [FromBody] UpdateEventDto updateEventDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedEvent = await _eventService.UpdateEventAsync(id, updateEventDto);
                return Ok(updatedEvent);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating event with ID: {EventId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            try
            {
                await _eventService.DeleteEventAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting event with ID: {EventId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
