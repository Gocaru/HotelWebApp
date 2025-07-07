using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }

        [Required]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        // Para pagamentos com cartão, etc.
        [Display(Name = "Transaction ID")]
        public string? TransactionId { get; set; }
    }
}
