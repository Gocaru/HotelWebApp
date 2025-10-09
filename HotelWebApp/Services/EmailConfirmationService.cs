using Microsoft.Extensions.Caching.Memory;

namespace HotelWebApp.Services
{
    public interface IEmailConfirmationService
    {
        string GenerateAndStoreCode(string userId);
        bool ValidateCode(string userId, string code);
    }

    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly IMemoryCache _cache;
        private const int CodeExpirationMinutes = 15;

        public EmailConfirmationService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateAndStoreCode(string userId)
        {
            var code = GenerateNumericCode();
            var cacheKey = $"EmailConfirmation_{userId}";

            _cache.Set(cacheKey, code, TimeSpan.FromMinutes(CodeExpirationMinutes));

            return code;
        }

        public bool ValidateCode(string userId, string code)
        {
            var cacheKey = $"EmailConfirmation_{userId}";

            if (_cache.TryGetValue(cacheKey, out string? storedCode))
            {
                if (storedCode == code)
                {
                    _cache.Remove(cacheKey); // Remove após validação
                    return true;
                }
            }

            return false;
        }

        private string GenerateNumericCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 dígitos
        }
    }
}