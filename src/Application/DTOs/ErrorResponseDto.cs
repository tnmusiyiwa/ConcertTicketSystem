using System.Text.Json.Serialization;

namespace ConcertTicketSystem.Application.DTOs
{
    public class ErrorResponseDto
    {
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public int Status { get; set; }
        public string TraceId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public object? Details { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<string>>? ValidationErrors { get; set; }
    }
}
