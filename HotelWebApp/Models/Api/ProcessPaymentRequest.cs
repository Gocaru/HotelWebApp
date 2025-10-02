using HotelWebApp.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Request model for processing a payment on an invoice
    /// </summary>
    public class ProcessPaymentRequest
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
