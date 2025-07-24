namespace HotelWebApp.Data.Entities
{
    public class ChangeRequest : IEntity
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }

        public string RequestDetails { get; set; } // Mensagem do hóspede

        public DateTime RequestedOn { get; set; }

        public RequestStatus Status { get; set; } // Novo enum: Pending, Approved, Rejected

        public string? EmployeeNotes { get; set; } // Notas do funcionário ao processar o pedido
        public DateTime? ProcessedOn { get; set; }
    }
}
