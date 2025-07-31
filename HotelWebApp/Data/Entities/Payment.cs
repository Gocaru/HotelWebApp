using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a single payment transaction made against an invoice.
    /// An invoice can have multiple payment records.
    /// </summary>
    public class Payment : IEntity
    {
        /// <summary>
        /// The unique identifier for the payment transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The foreign key linking to the invoice this payment is for.
        /// </summary>
        [Required]
        public int InvoiceId { get; set; }

        /// <summary>
        /// Navigation property to the associated Invoice.
        /// </summary>
        [ForeignKey("InvoiceId")]
        public virtual Invoice Invoice { get; set; }


        /// <summary>
        /// The exact date and time when the payment was processed.
        /// </summary>
        [Required]
        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }

        /// <summary>
        /// The amount of money paid in this specific transaction.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        /// <summary>
        /// The method used for the payment (e.g., Cash, CreditCard).
        /// </summary>
        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod PaymentMethod { get; set; }

        /// <summary>
        /// An optional, external transaction identifier.
        /// Useful for referencing payments in external systems like credit card gateways.
        /// </summary>
        [Display(Name = "Transaction ID")]
        public string? TransactionId { get; set; }
    }
}
