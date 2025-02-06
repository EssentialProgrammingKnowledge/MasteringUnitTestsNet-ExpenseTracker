using System.Text.Json.Serialization;

namespace ExpenseTracker.API.DTO
{
    public record ErrorMessage(string Code, string Message, Dictionary<string, object>? Parameters = null)
    {
        public string Code { get; set; } = Code;
        public string Message { get; set; } = Message;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, object>? Parameters { get; set; } = Parameters;
    }
}
