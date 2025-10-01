using HotelWebApp.Data.Entities;

namespace HotelWebApp.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
        Task<bool> ValidateTokenAsync(string token);
    }
}
