namespace HotelWebApp.Models.Api
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
}
