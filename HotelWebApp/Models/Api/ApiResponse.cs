namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Response wrapper for all API endpoints containing success status, data, and error information
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
