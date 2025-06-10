namespace ConcertTicketSystem.Domain.Exceptions
{
    public class ValidationException : DomainException
    {
        public Dictionary<string, List<string>> ValidationErrors { get; }

        public ValidationException(string message, Dictionary<string, List<string>> validationErrors)
            : base(message, "VALIDATION_ERROR", validationErrors)
        {
            ValidationErrors = validationErrors;
        }

        public ValidationException(string field, string error)
            : this("Validation failed", new Dictionary<string, List<string>>
            {
                { field, new List<string> { error } }
            })
        {
        }

        public ValidationException(Dictionary<string, List<string>> validationErrors)
            : this("One or more validation errors occurred", validationErrors)
        {
        }
    }
}
