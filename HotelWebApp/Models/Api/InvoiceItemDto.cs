namespace HotelWebApp.Models.Api
{
    /// <summary>
    /// Represents a single line item in an invoice (room charge, amenity, activity, or discount)
    /// </summary>
    public class InvoiceItemDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}