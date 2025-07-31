using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    /// <summary>
    /// Represents a financial invoice generated for a reservation, typically at check-out.
    /// It details the total amount due and tracks the payment status.
    /// </summary>
    public class Invoice : IEntity
    {
        /// <summary>
        /// The unique identifier for the invoice.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The foreign key linking to the reservation this invoice is for.
        /// </summary>
        [Required]
        public int ReservationId { get; set; }

        /// <summary>
        /// Navigation property to the associated Reservation.
        /// </summary>
        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }

        /// <summary>
        /// The foreign key linking to the guest (ApplicationUser) who is responsible for the payment.
        /// </summary>
        [Required]
        public string GuestId { get; set; }

        /// <summary>
        /// Navigation property to the associated Guest.
        /// </summary>
        [ForeignKey("GuestId")]
        public virtual ApplicationUser Guest { get; set; }

        /// <summary>
        /// The date and time when the invoice was generated.
        /// </summary>
        [Required]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// The final, total amount to be paid for the reservation, including room charges and all amenities.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The current payment status of the invoice (e.g., Unpaid, PartiallyPaid, Paid).
        /// </summary>
        [Required]
        public InvoiceStatus Status { get; set; }

        /// <summary>
        /// Navigation property to a collection of payments made against this invoice.
        /// An invoice can have multiple partial payments.
        /// </summary>
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}
