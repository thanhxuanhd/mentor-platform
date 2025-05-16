namespace Domain.Models
{
    public class GeneralResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; } = null;
        public int? TotalCount { get; set; } = 0;
    }
}
