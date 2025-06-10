using System.Net;
using System.Text.Json;
using ConcertTicketSystem.Application.DTOs;
using ConcertTicketSystem.Domain.Exceptions;

namespace ConcertTicketSystem.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                ValidationException validationEx => new ErrorResponseDto
                {
                    Title = "Validation Error",
                    Detail = validationEx.Message,
                    ErrorCode = validationEx.ErrorCode,
                    Status = (int)HttpStatusCode.BadRequest,
                    ValidationErrors = validationEx.ValidationErrors,
                    TraceId = context.TraceIdentifier
                },
                EntityNotFoundException notFoundEx => new ErrorResponseDto
                {
                    Title = "Resource Not Found",
                    Detail = notFoundEx.Message,
                    ErrorCode = notFoundEx.ErrorCode,
                    Status = (int)HttpStatusCode.NotFound,
                    Details = notFoundEx.Details,
                    TraceId = context.TraceIdentifier
                },
                BusinessRuleException businessEx => new ErrorResponseDto
                {
                    Title = "Business Rule Violation",
                    Detail = businessEx.Message,
                    ErrorCode = businessEx.ErrorCode,
                    Status = (int)HttpStatusCode.BadRequest,
                    Details = businessEx.Details,
                    TraceId = context.TraceIdentifier
                },
                ArgumentException argEx => new ErrorResponseDto
                {
                    Title = "Invalid Argument",
                    Detail = argEx.Message,
                    ErrorCode = "INVALID_ARGUMENT",
                    Status = (int)HttpStatusCode.BadRequest,
                    TraceId = context.TraceIdentifier
                },
                _ => new ErrorResponseDto
                {
                    Title = "Internal Server Error",
                    Detail = _environment.IsDevelopment() 
                        ? exception.Message 
                        : "An unexpected error occurred. Please try again later.",
                    ErrorCode = "INTERNAL_SERVER_ERROR",
                    Status = (int)HttpStatusCode.InternalServerError,
                    TraceId = context.TraceIdentifier,
                    Details = _environment.IsDevelopment() 
                        ? new { StackTrace = exception.StackTrace, InnerException = exception.InnerException?.Message }
                        : null
                }
            };

            context.Response.StatusCode = errorResponse.Status;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}
