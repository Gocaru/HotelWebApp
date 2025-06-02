using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe responsável pela gestão dos serviços extra associados às reservas.
    /// </summary>
    public class CrudExtras
    {
        private string _filePath = @"Data\ExtraServices.txt";

        public CrudExtras()
        {
            FileStorageHelper.EnsureFileExists(_filePath);
        }

        /// <summary>
        /// Adiciona um novo serviço extra ao ficheiro.
        /// </summary>
        /// <param name="service">Serviço extra a adicionar.</param>
        public void Add(ExtraService service)
        {
            string linha = $"{service.ExtraServiceId};{service.BookingId};{service.ServiceType};{service.Price}";
            File.AppendAllLines(_filePath, new[] { linha });
        }

        /// <summary>
        /// Devolve a lista de todos os serviços extra existentes.
        /// </summary>
        public List<ExtraService> GetAll()
        {
            var extras = new List<ExtraService>();

            if (!File.Exists(_filePath))
                return extras;

            var linhas = File.ReadAllLines(_filePath);

            foreach (var linha in linhas)
            {
                var partes = linha.Split(';');

                if (partes.Length == 4 &&
                    int.TryParse(partes[0], out int id) &&
                    int.TryParse(partes[1], out int bookingId) &&
                    Enum.TryParse(partes[2], out ExtraServiceType tipo) &&
                    decimal.TryParse(partes[3], out decimal preco))
                {
                    extras.Add(new ExtraService
                    {
                        ExtraServiceId = id,
                        BookingId = bookingId,
                        ServiceType = tipo,
                        Price = preco
                    });
                }
            }

            return extras;
        }

        /// <summary>
        /// Devolve todos os serviços associados a uma determinada reserva.
        /// </summary>
        public List<ExtraService> GetByBookingId(int bookingId)
        {
            return GetAll().Where(s => s.BookingId == bookingId).ToList();
        }

        /// <summary>
        /// Calcula o total dos serviços extra de uma reserva.
        /// </summary>
        public decimal GetTotalByBooking(int bookingId)
        {
            return GetByBookingId(bookingId).Sum(s => s.Price);
        }

        /// <summary>
        /// Gera um novo ID único para o próximo serviço extra.
        /// </summary>
        public int GenerateNextId()
        {
            var todos = GetAll();
            return todos.Count == 0 ? 1 : todos.Max(s => s.ExtraServiceId) + 1;
        }

        /// <summary>
        /// Remove um serviço extra com base no ID.
        /// </summary>
        public void Delete(int id)
        {
            var todos = GetAll();
            var atualizados = todos.Where(s => s.ExtraServiceId != id).ToList();
            SaveAll(atualizados);
        }

        /// <summary>
        /// Atualiza os dados de um serviço extra.
        /// </summary>
        public void Update(ExtraService servicoAtualizado)
        {
            var todos = GetAll();

            for (int i = 0; i < todos.Count; i++)
            {
                if (todos[i].ExtraServiceId == servicoAtualizado.ExtraServiceId)
                {
                    todos[i] = servicoAtualizado;
                    break;
                }
            }

            SaveAll(todos);
        }

        /// <summary>
        /// Substitui todo o conteúdo do ficheiro com a lista fornecida de serviços.
        /// </summary>
        private void SaveAll(List<ExtraService> extras)
        {
            var linhas = extras.Select(s =>
                $"{s.ExtraServiceId};{s.BookingId};{s.ServiceType};{s.Price}");
            File.WriteAllLines(_filePath, linhas);
        }
    }
}
