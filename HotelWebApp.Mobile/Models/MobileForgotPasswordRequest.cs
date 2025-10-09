using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Models
{
    public class MobileForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("appUrl")]
        public string AppUrl { get; set; } = string.Empty;
    }
}
