namespace ConcertTicketSystem.Domain.Exceptions
{
    public class EventNotActiveException : BusinessRuleException
    {
        public EventNotActiveException(Guid eventId)
            : base("Event is not active and tickets cannot be reserved or purchased", 
                  "EVENT_NOT_ACTIVE", 
                  new { EventId = eventId })
        {
        }
    }

    public class EventCapacityExceededException : BusinessRuleException
    {
        public EventCapacityExceededException(Guid eventId, int totalCapacity, int currentCapacity)
            : base($"Event capacity would be exceeded. Total: {totalCapacity}, Current: {currentCapacity}", 
                  "EVENT_CAPACITY_EXCEEDED", 
                  new { EventId = eventId, TotalCapacity = totalCapacity, CurrentCapacity = currentCapacity })
        {
        }
    }

    public class EventInPastException : BusinessRuleException
    {
        public EventInPastException(Guid eventId, DateTime eventDate)
            : base($"Cannot perform operation on past event. Event date: {eventDate:yyyy-MM-dd HH:mm:ss}", 
                  "EVENT_IN_PAST", 
                  new { EventId = eventId, EventDate = eventDate })
        {
        }
    }
}
