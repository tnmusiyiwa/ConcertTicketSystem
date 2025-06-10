namespace ConcertTicketSystem.Domain.Exceptions
{
    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message, string errorCode = "BUSINESS_RULE_VIOLATION", object? details = null)
            : base(message, errorCode, details)
        {
        }
    }
}
