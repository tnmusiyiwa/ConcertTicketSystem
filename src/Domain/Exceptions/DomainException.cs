namespace ConcertTicketSystem.Domain.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string ErrorCode { get; }
        public object? Details { get; }

        protected DomainException(string message, string errorCode, object? details = null) 
            : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }

        protected DomainException(string message, string errorCode, Exception innerException, object? details = null) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
