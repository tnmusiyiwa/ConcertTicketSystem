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
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        /// <summary>
        /// Get event by ID
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Event details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EventDto>> GetEvent(Guid id)
        {
            var eventDto = await _eventService.GetEventByIdAsync(id);
            if (eventDto == null)
            {
                return NotFound($"Event with ID {id} not found");
            }
            return Ok(eventDto);
        }

        /// <summary>
        /// Create a new event
        /// </summary>
        /// <param name="createEventDto">Event creation data</param>
        /// <returns>Created event</returns>
        [HttpPost]
        public async Task<ActionResult<EventDto>> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEvent = await _eventService.CreateEventAsync(createEventDto);
            return CreatedAtAction(nameof(GetEvent), new { id = createdEvent.Id }, createdEvent);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedEvent = await _eventService.UpdateEventAsync(id, updateEventDto);
            return Ok(updatedEvent);
        }

        /// <summary>
        /// Delete an event
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
    }
}
