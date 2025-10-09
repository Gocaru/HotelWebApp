namespace HotelWebApp.Mobile.Models
{
    public class InvoiceItemDto
    {
        public string Description { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }

        public string UnitPriceFormatted => $"€{UnitPrice:N2}";
        public string TotalPriceFormatted => $"€{TotalPrice:N2}";
    }
}