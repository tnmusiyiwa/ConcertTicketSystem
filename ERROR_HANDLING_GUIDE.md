# Error Handling and Validation Guide

## Overview

The Concert Ticket Management System now includes comprehensive error handling and validation to provide meaningful error messages to users instead of generic "Internal Server Error" responses.

## Error Response Format

All errors now return a standardized JSON response:

\`\`\`json
{
"title": "Error Category",
"detail": "Detailed error message",
"errorCode": "SPECIFIC_ERROR_CODE",
"status": 400,
"traceId": "unique-trace-id",
"timestamp": "2024-01-01T12:00:00Z",
"details": {
// Additional context-specific information
},
"validationErrors": {
// Field-specific validation errors (only for validation failures)
"fieldName": ["Error message 1", "Error message 2"]
}
}
\`\`\`

## Error Categories

### 1. Validation Errors (400 Bad Request)

**Error Code**: `VALIDATION_ERROR`

Returned when input data doesn't meet validation requirements:

- **Field Validation**: Required fields, length limits, format validation
- **Business Rules**: Logical constraints (e.g., event date must be in future)
- **Data Type Validation**: Correct data types and ranges

**Example Response**:
\`\`\`json
{
"title": "Validation Error",
"detail": "One or more validation errors occurred",
"errorCode": "VALIDATION_ERROR",
"status": 400,
"validationErrors": {
"name": ["Event name is required"],
"eventDate": ["Event date must be in the future"],
"totalCapacity": ["Total capacity must be greater than 0"]
}
}
\`\`\`

### 2. Entity Not Found (404 Not Found)

**Error Code**: `ENTITY_NOT_FOUND`

Returned when requested resources don't exist:

\`\`\`json
{
"title": "Resource Not Found",
"detail": "Event with ID '12345678-1234-1234-1234-123456789abc' was not found.",
"errorCode": "ENTITY_NOT_FOUND",
"status": 404,
"details": {
"entityType": "Event",
"id": "12345678-1234-1234-1234-123456789abc"
}
}
\`\`\`

### 3. Business Rule Violations (400 Bad Request)

Various business-specific error codes:

#### Ticket Errors

- **`TICKET_NOT_AVAILABLE`**: No tickets available for reservation
- **`TICKET_RESERVATION_EXPIRED`**: Trying to purchase an expired reservation
- **`INVALID_TICKET_STATUS`**: Operation not allowed for current ticket status
- **`TICKET_ALREADY_CANCELLED`**: Trying to modify a cancelled ticket

#### Event Errors

- **`EVENT_NOT_ACTIVE`**: Trying to book tickets for inactive event
- **`EVENT_CAPACITY_EXCEEDED`**: Operation would exceed event capacity
- **`EVENT_IN_PAST`**: Trying to modify past events
- **`EVENT_HAS_SOLD_TICKETS`**: Trying to delete event with sold tickets

#### Other Business Rules

- **`DUPLICATE_PAYMENT_TRANSACTION`**: Payment transaction ID already exists
- **`CAPACITY_REDUCTION_NOT_ALLOWED`**: Cannot reduce capacity below sold tickets

## Validation Rules

### Event Validation

- **Name**: Required, max 200 characters
- **Venue**: Required, max 200 characters
- **Description**: Optional, max 1000 characters
- **Event Date**: Must be in future, max 5 years ahead
- **Total Capacity**: 1 to 1,000,000

### Ticket Type Validation

- **Name**: Required, max 100 characters
- **Description**: Optional, max 500 characters
- **Price**: $0.01 to $999,999.99
- **Total Quantity**: 1 to 1,000,000
- **Event ID**: Must be valid GUID of existing event

### Ticket Reservation Validation

- **Customer Email**: Required, valid email format, max 200 characters
- **Customer Name**: Required, 2-200 characters
- **Ticket Type ID**: Must be valid GUID

### Ticket Purchase Validation

- **Ticket ID**: Must be valid GUID
- **Payment Transaction ID**: Required, 3-200 characters, must be unique

## Error Handling Examples

### Testing Error Scenarios

Use the provided `test-requests.http` file to test various error scenarios:

1. **Validation Errors**:
   \`\`\`http
   POST https://localhost:5001/api/events
   {
   "name": "",
   "totalCapacity": -5
   }
   \`\`\`

2. **Entity Not Found**:
   \`\`\`http
   GET https://localhost:5001/api/events/invalid-guid
   \`\`\`

3. **Business Rule Violations**:
   \`\`\`http
   POST https://localhost:5001/api/tickets/reserve
   {
   "ticketTypeId": "non-existent-id"
   }
   \`\`\`

## Development Notes

### Exception Hierarchy

- **`DomainException`**: Base class for all domain-specific exceptions
- **`ValidationException`**: Field validation errors
- **`EntityNotFoundException`**: Resource not found errors
- **`BusinessRuleException`**: Business logic violations

### Global Exception Middleware

The `GlobalExceptionMiddleware` automatically:

- Catches all unhandled exceptions
- Maps them to appropriate HTTP status codes
- Returns standardized error responses
- Logs errors for debugging
- Hides sensitive information in production

### Custom Validators

Each DTO has a dedicated validator class:

- `CreateEventDtoValidator`
- `CreateTicketTypeDtoValidator`
- `ReserveTicketDtoValidator`
- `PurchaseTicketDtoValidator`

### Controller Simplification

Controllers are now much cleaner:

- No repetitive try-catch blocks
- Middleware handles all exceptions
- Focus on happy path logic
- Automatic model state validation

## Benefits

1. **Better User Experience**: Clear, actionable error messages
2. **Easier Debugging**: Structured error responses with trace IDs
3. **API Consistency**: All errors follow the same format
4. **Maintainability**: Centralized error handling logic
5. **Security**: Sensitive information hidden in production
6. **Logging**: All errors automatically logged for monitoring

## Error Codes Reference

| Error Code                    | Description                         | HTTP Status |
| ----------------------------- | ----------------------------------- | ----------- |
| VALIDATION_ERROR              | Input validation failed             | 400         |
| ENTITY_NOT_FOUND              | Requested resource not found        | 404         |
| BUSINESS_RULE_VIOLATION       | Generic business rule violation     | 400         |
| TICKET_NOT_AVAILABLE          | No tickets available                | 400         |
| TICKET_RESERVATION_EXPIRED    | Reservation has expired             | 400         |
| INVALID_TICKET_STATUS         | Invalid ticket status for operation | 400         |
| EVENT_NOT_ACTIVE              | Event is not active                 | 400         |
| EVENT_IN_PAST                 | Event is in the past                | 400         |
| DUPLICATE_PAYMENT_TRANSACTION | Payment ID already exists           | 400         |
| INTERNAL_SERVER_ERROR         | Unexpected server error             | 500         |

This comprehensive error handling system ensures that users receive meaningful feedback for all error scenarios while maintaining security and providing excellent debugging capabilities for developers.
