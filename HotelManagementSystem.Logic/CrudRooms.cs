using HotelManagement.Core;


namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe responsável pelas operações CRUD sobre os quartos do hotel.
    /// </summary>
    public class CrudRooms
    {
        private string _filePath = @"Data\rooms.txt"; // Caminho onde os dados são guardados

        public CrudRooms()
        {
            FileStorageHelper.EnsureFileExists(_filePath);
        }

        /// <summary>
        /// Adiciona um novo quarto ao ficheiro, gerando automaticamente o RoomId.
        /// </summary>
        public void Add(Room room)
        {
            room.RoomId = GetNextRoomId();
            string linha = $"{room.RoomId};{room.Number};{room.Type};{room.Capacity};{room.PricePerNight};{room.Status}";
            FileStorageHelper.AppendLine(_filePath, linha);
        }

        /// <summary>
        /// Lê todos os quartos a partir do ficheiro e devolve-os como uma lista de objetos Room.
        /// </summary>
        public List<Room> GetAll()
        {
            var rooms = new List<Room>();
            var linhas = FileStorageHelper.ReadAllLines(_filePath);

            foreach (var linha in linhas)
            {
                var dados = linha.Split(';');

                if (dados.Length == 6 &&
                    int.TryParse(dados[0], out int id) &&
                    int.TryParse(dados[1], out int number) &&
                    Enum.TryParse(dados[2], out RoomType type) &&
                    int.TryParse(dados[3], out int capacity) &&
                    decimal.TryParse(dados[4], out decimal price) &&
                    Enum.TryParse(dados[5], out RoomStatus status))
                {
                    rooms.Add(new Room
                    {
                        RoomId = id,
                        Number = number,
                        Type = type,
                        Capacity = capacity,
                        PricePerNight = price,
                        Status = status
                    });
                }
            }
            return rooms;
        }

        /// <summary>
        /// Atualiza os dados de um quarto existente com base no RoomId.
        /// </summary>
        public void Update(Room updatedRoom)
        {
            var allRooms = GetAll();
            var index = allRooms.FindIndex(r => r.RoomId == updatedRoom.RoomId);

            if (index >= 0)
            {
                allRooms[index] = updatedRoom;
                SaveAll(allRooms);
            }
        }

        /// <summary>
        /// Tenta remover um quarto com base no RoomId, se não tiver reservas futuras.
        /// </summary>
        public bool Delete(int roomId)
        {
            if (!CanDelete(roomId))
                return false;

            var allRooms = GetAll();
            allRooms = allRooms.Where(r => r.RoomId != roomId).ToList();
            SaveAll(allRooms);
            return true;
        }

        /// <summary>
        /// Verifica se é possível remover um quarto com base no RoomId.
        /// </summary>
        public bool CanDelete(int roomId)
        {
            var bookings = new CrudBookings().GetAll();
            return !bookings.Any(b =>
                b.RoomId == roomId &&
                b.Status != BookingStatus.Cancelled &&
                b.CheckOutDate >= DateTime.Today);
        }

        /// <summary>
        /// Filtra os quartos de acordo com os critérios especificados.
        /// </summary>
        public List<Room> Filter(RoomType? type, RoomStatus? status, decimal? maxPrice)
        {
            var rooms = GetAll();

            if (type != null)
                rooms = rooms.Where(r => r.Type == type).ToList();

            if (status != null)
                rooms = rooms.Where(r => r.Status == status).ToList();

            if (maxPrice != null)
                rooms = rooms.Where(r => r.PricePerNight <= maxPrice).ToList();

            return rooms;
        }

        /// <summary>
        /// Substitui o conteúdo do ficheiro com a lista fornecida.
        /// </summary>
        private void SaveAll(List<Room> rooms)
        {
            var linhas = rooms.Select(r =>
                $"{r.RoomId};{r.Number};{r.Type};{r.Capacity};{r.PricePerNight};{r.Status}");
            FileStorageHelper.WriteAllLines(_filePath, linhas);
        }

        /// <summary>
        /// Procura um quarto com base no número (valor visível, não técnico).
        /// </summary>
        public Room GetByRoomNumber(int number)
        {
            return GetAll().FirstOrDefault(r => r.Number == number);
        }

        /// <summary>
        /// Procura um quarto com base no RoomId (chave técnica).
        /// </summary>
        public Room GetByRoomId(int id)
        {
            return GetAll().FirstOrDefault(r => r.RoomId == id);
        }

        /// <summary>
        /// Gera o próximo RoomId incremental.
        /// </summary>
        private int GetNextRoomId()
        {
            var all = GetAll();
            return all.Any() ? all.Max(r => r.RoomId) + 1 : 1;
        }
    }
}

