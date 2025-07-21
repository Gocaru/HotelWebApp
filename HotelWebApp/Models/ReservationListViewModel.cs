namespace HotelWebApp.Models
{
    public class ReservationListViewModel
    {
        public int Id { get; set; }

        public string GuestName { get; set; }
        
        public string RoomNumber { get; set; }

        public string RoomDetails { get; set; }

        public DateTime CheckInDate { get; set; }
        
        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        public decimal TotalCost { get; set; }
        
        public string StatusText { get; set; } // O nome do estado (ex: "Confirmed")
        
        public string StatusBadgeClass { get; set; } // A classe CSS para a cor (ex: "bg-primary")

        // Propriedades para ajudar a lógica dos botões de ação
        public bool CanCheckIn { get; set; }
        
        public bool CanCheckOut { get; set; }
        
        public bool CanEdit { get; set; }
        
        public bool CanDelete { get; set; }

       
    }
}
