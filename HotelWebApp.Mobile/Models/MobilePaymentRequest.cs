using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Mobile.Models
{
    public class MobilePaymentRequest
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "CreditCard";

        [Required]
        [StringLength(19, MinimumLength = 13)]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 3)]
        public string CVV { get; set; } = string.Empty;
    }
}