namespace HotelWebApp.Models
{
    /// <summary>
    /// ViewModel for the application's generic error page.
    /// It provides the necessary data to display details about a specific error.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the request that resulted in an error.
        /// This is useful for tracking and debugging purposes.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// A read-only property that determines whether the RequestId should be displayed.
        /// It will be true only if a RequestId has been generated.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
