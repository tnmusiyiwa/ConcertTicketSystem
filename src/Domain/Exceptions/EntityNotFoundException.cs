namespace ConcertTicketSystem.Domain.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityType, Guid id)
            : base($"{entityType} with ID '{id}' was not found.", "ENTITY_NOT_FOUND", new { EntityType = entityType, Id = id })
        {
        }

        public EntityNotFoundException(string entityType, string identifier, string identifierName = "identifier")
            : base($"{entityType} with {identifierName} '{identifier}' was not found.", "ENTITY_NOT_FOUND", 
                  new { EntityType = entityType, Identifier = identifier, IdentifierName = identifierName })
        {
        }
    }
}
