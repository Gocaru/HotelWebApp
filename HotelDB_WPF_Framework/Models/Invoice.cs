using HotelDB_WPF_Framework.Enums;
using System;

namespace HotelDB_WPF_Framework.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int BookingId { get; set; }
        public DateTime IssueDate { get; set; }
        public decimal StayTotal { get; set; }
        public decimal ExtrasTotal { get; set; }
        public InvoicePaymentMethod PaymentMethod { get; set; }
        public decimal Total { get; set; }

        // Propriedades apenas para visualização:
        public string GuestName { get; set; }         
        public DateTime CheckIn { get; set; }         
        public DateTime CheckOut { get; set; }        
    }
}
