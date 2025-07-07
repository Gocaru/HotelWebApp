using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelWebApp.Data.Entities
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }

        [Required]
        public string GuestId { get; set; }

        [ForeignKey("GuestId")]
        public virtual ApplicationUser Guest { get; set; }

        [Required]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; }

        // Uma fatura pode ter vários pagamentos (ex: pagamento parcial)
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}
