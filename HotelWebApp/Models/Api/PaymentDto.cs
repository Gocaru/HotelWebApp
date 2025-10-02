namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents a payment transaction made against an invoice
    /// </summary>
    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? TransactionId { get; set; }
    }
}
