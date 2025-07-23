namespace HotelWebApp.Models
{
    public class SchedulerBatchUpdateViewModel
    {
        // O DataManager envia as listas de registos adicionados, alterados e apagados.
        // O nome das propriedades (Added, Changed, Deleted) tem de corresponder ao que o scheduler envia.
        public List<SchedulerEventViewModel>? Added { get; set; }
        public List<SchedulerEventViewModel>? Changed { get; set; }
        public List<SchedulerEventViewModel>? Deleted { get; set; }

        // O scheduler também pode enviar outros parâmetros
        public IDictionary<string, object>? Params { get; set; }
    }
}
