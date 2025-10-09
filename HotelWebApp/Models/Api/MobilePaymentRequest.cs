using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for processing payments from the mobile app with card details
    /// </summary>
    public class MobilePaymentRequest
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "CreditCard";

        // Card details for simulation
        [Required]
        [StringLength(19, MinimumLength = 13)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Invalid expiry date format (MM/YY)")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; } = string.Empty;
    }
}