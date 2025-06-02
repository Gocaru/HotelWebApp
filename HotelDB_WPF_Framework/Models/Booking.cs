using HotelDB_WPF_Framework.Enums;
using Newtonsoft.Json;
using System;

namespace HotelDB_WPF_Framework.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int GuestId { get; set; }
        
        public int RoomId { get; set; }
        
        public DateTime CheckInDate { get; set; }
        
        public DateTime CheckOutDate { get; set; }
        
        public DateTime ReservationDate { get; set; }
        
        public int NumberOfGuests { get; set; }
        
        public BookingStatus Status { get; set; }

        public string GuestName { get; set; }  // Só para visualização
        
        public string RoomTypeName { get; set; } // Só para visualização

        public int RoomNumber { get; set; }        // Para mostrar o número do quarto
    }
}
