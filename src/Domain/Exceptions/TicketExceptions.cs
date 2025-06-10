namespace ConcertTicketSystem.Domain.Exceptions
{
    public class TicketNotAvailableException : BusinessRuleException
    {
        public TicketNotAvailableException(Guid ticketTypeId, int requestedQuantity = 1)
            : base($"Tickets are not available for this ticket type. Requested: {requestedQuantity}", 
                  "TICKET_NOT_AVAILABLE", 
                  new { TicketTypeId = ticketTypeId, RequestedQuantity = requestedQuantity })
        {
        }
    }

    public class TicketReservationExpiredException : BusinessRuleException
    {
        public TicketReservationExpiredException(Guid ticketId, DateTime expiredAt)
            : base($"Ticket reservation has expired at {expiredAt:yyyy-MM-dd HH:mm:ss} UTC", 
                  "TICKET_RESERVATION_EXPIRED", 
                  new { TicketId = ticketId, ExpiredAt = expiredAt })
        {
        }
    }

    public class InvalidTicketStatusException : BusinessRuleException
    {
        public InvalidTicketStatusException(Guid ticketId, string currentStatus, string expectedStatus)
            : base($"Ticket is in '{currentStatus}' status but expected '{expectedStatus}'", 
                  "INVALID_TICKET_STATUS", 
                  new { TicketId = ticketId, CurrentStatus = currentStatus, ExpectedStatus = expectedStatus })
        {
        }
    }

    public class TicketAlreadyCancelledException : BusinessRuleException
    {
        public TicketAlreadyCancelledException(Guid ticketId)
            : base("Ticket is already cancelled and cannot be modified", 
                  "TICKET_ALREADY_CANCELLED", 
                  new { TicketId = ticketId })
        {
        }
    }
}
