namespace HotelWebApp.Models
{
    public class SchedulerEventViewModel
    {
        public int Id { get; set; }        // ID da Reserva

        public string Subject { get; set; } // Texto que aparece no evento

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsAllDay { get; set; }

        public string? Location { get; set; } // Pode ser o número do quarto
        
        public string? Description { get; set; } // Pode ter mais detalhes sobre a reserva
        
        public string? RecurrenceRule { get; set; } // Para eventos recorrentes (em principio não vamos usar, mas é bom ter)

        // Propriedade para dar cor aos eventos
        public string? CategoryColor { get; set; }

        public int RoomId { get; set; }

    }
}
