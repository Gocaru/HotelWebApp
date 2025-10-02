namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Response model for successful authentication containing JWT token and user data
    /// </summary>
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
}
