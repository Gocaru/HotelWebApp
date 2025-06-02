using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe responsável pelas operações CRUD sobre os hóspedes.
    /// </summary>
    public class CrudGuests
    {
        private string _filePath = @"Data\guests.txt"; // Caminho onde os dados dos hóspedes são guardados

        public CrudGuests()
        {
            FileStorageHelper.EnsureFileExists(_filePath);
        }

        /// <summary>
        /// Adiciona um novo hóspede ao ficheiro.
        /// </summary>
        /// <param name="guest">Objeto Guest a adicionar.</param>
        public void Add(Guest guest)
        {
            string linha = $"{guest.GuestId};{guest.Name};{guest.Contact};{guest.Email};{guest.IdentificationDocument}";
            FileStorageHelper.AppendLine(_filePath, linha);
        }

        /// <summary>
        /// Devolve a lista de todos os Hóspedes existentes no ficheiro.
        /// </summary>
        public List<Guest> GetAll()
        {
            var guests = new List<Guest>();
            var linhas = FileStorageHelper.ReadAllLines(_filePath);

            foreach (var linha in linhas)
            {
                var dados = linha.Split(';');

                if (dados.Length == 5 &&
                    int.TryParse(dados[0], out int id))
                {
                    guests.Add(new Guest
                    {
                        GuestId = id,
                        Name = dados[1],
                        Contact = dados[2],
                        Email = dados[3],
                        IdentificationDocument = dados[4]
                    });
                }
            }

            return guests;
        }

        /// <summary>
        /// Atualiza os dados de um hóspede existente, identificado pelo seu ID.
        /// </summary>
        public void Update(Guest updatedGuest)
        {
            List<Guest> allGuests = GetAll();

            for (int i = 0; i < allGuests.Count; i++)
            {
                if (allGuests[i].GuestId == updatedGuest.GuestId)
                {
                    allGuests[i] = updatedGuest;
                    break;
                }
            }

            SaveAll(allGuests);
        }

        /// <summary>
        /// Tenta remover um hóspede, apenas se não tiver reservas ativas.
        /// </summary>
        /// <param name="guestId">ID do hóspede a remover.</param>
        /// <returns>True se for removido; False se tiver reservas ativas.</returns>
        public bool Delete(int guestId)
        {
            if (!CanDelete(guestId))
                return false;

            List<Guest> allGuests = GetAll();

            List<Guest> updatedList = new List<Guest>();

            foreach (Guest guest in allGuests)
            {
                if (guest.GuestId != guestId)
                {
                    updatedList.Add(guest);
                }
            }

            SaveAll(updatedList);
            return true;
        }

        /// <summary>
        /// Verifica se o hóspede pode ser removido, isto é, se não tem reservas ativas.
        /// </summary>
        public bool CanDelete(int guestId)
        {
            var bookings = new CrudBookings().GetAll();

            foreach (var booking in bookings)
            {
                bool isSameGuest = booking.GuestId == guestId;
                bool isActive = booking.Status == BookingStatus.Reserved || booking.Status == BookingStatus.CheckedIn;
                bool isFutureOrOngoing = booking.CheckOutDate >= DateTime.Today;

                if (isSameGuest && isActive && isFutureOrOngoing)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Substitui todo o conteúdo do ficheiro com a lista de hóspedes fornecida.
        /// </summary>
        private void SaveAll(List<Guest> guests)
        {
            var linhas = guests.Select(g =>
                $"{g.GuestId};{g.Name};{g.Contact};{g.Email};{g.IdentificationDocument}");
            FileStorageHelper.WriteAllLines(_filePath, linhas);
        }

        /// <summary>
        /// Procura um hóspede com base no seu número de documento de identificação.
        /// </summary>
        /// <param name="document">Número do documento de identificação</param>
        /// <returns>Um objeto correspondente ao documento fornecido,
        /// ou null se nenhum hóspede for encontrado.</returns>
        public Guest GetByIdentificationDocument(string document)
        {
            List<Guest> allGuests = GetAll();

            foreach (Guest guest in allGuests)
            {
                if (guest.IdentificationDocument == document)
                {
                    return guest;
                }
            }

            return null;
        }
    }
}
