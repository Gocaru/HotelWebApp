using HotelManagement.Core;
using HotelManagementSystem.Logic;
using System.Data;


namespace HotelManagementApp.UserControls
{
    /// <summary>
    /// UserControl responsável pela gestão de reservas: adicionar, editar, remover e listar.
    /// </summary>
    public partial class UCBookings : UserControl
    {
        private CrudBookings crud = new CrudBookings();
        private int selectedBookingId = -1;
        public UCBookings()
        {
            InitializeComponent();
            LoadBookings();
            AttachEvents();
        }

        /// <summary>
        /// Carrega todas as reservas para a grelha.
        /// </summary>
        private void LoadBookings()
        {
            dgvBookings.Rows.Clear();
            dgvBookings.Columns.Clear();

            dgvBookings.Columns.Add("BookingId", "Booking ID");
            dgvBookings.Columns.Add("GuestName", "Guest Name");
            dgvBookings.Columns.Add("GuestDocument", "ID Document");
            dgvBookings.Columns.Add("RoomNumber", "Room Number");
            dgvBookings.Columns.Add("ReservationDate", "Reservation Date");
            dgvBookings.Columns.Add("CheckInDate", "Check-In Date");
            dgvBookings.Columns.Add("CheckOutDate", "Check-Out Date");
            dgvBookings.Columns.Add("Guests", "Occupants");

            List<Booking> bookings = crud.GetAll();
            List<Guest> allGuests = new CrudGuests().GetAll();
            List<Room> allRooms = new CrudRooms().GetAll();

            foreach (var booking in bookings)
            {
                var guest = allGuests.FirstOrDefault(g => g.GuestId == booking.GuestId);
                var room = allRooms.FirstOrDefault(r => r.RoomId == booking.RoomId);

                string guestName = guest?.Name ?? "Unknown";
                string guestDoc = guest?.IdentificationDocument ?? "N/A";
                int roomNumber = room?.Number ?? 0;

                dgvBookings.Rows.Add(
                    booking.BookingId,
                    guestName,
                    guestDoc,
                    roomNumber,
                    booking.ReservationDate.ToShortDateString(),
                    booking.CheckInDate.ToShortDateString(),
                    booking.CheckOutDate.ToShortDateString(),
                    booking.NumberOfGuests
                );
            }

            pnlAddBooking.Visible = false;
        }

        /// <summary>
        /// Associa os eventos aos botões e à grelha.
        /// </summary>
        private void AttachEvents()
        {
            btnAddBooking.Click += btnAddBooking_Click;
            btnEditBooking.Click += btnEditBooking_Click;
            btnDeleteBooking.Click += btnDeleteBooking_Click;
            btnSaveBooking.Click += btnSaveBooking_Click;
            btnCancelBooking.Click += btnCancelBooking_Click;
            btnCancelNoShow.Click += BtnCancelNoShow_Click;
            btnCheckIn.Click += BtnCheckIn_Click;
            btnCheckOut.Click += BtnCheckOut_Click;

            dgvBookings.SelectionChanged += dgvBookings_SelectionChanged;
            cmbRoomType.SelectedIndexChanged += cmbRoomType_SelectedIndexChanged;
            nudGuests.ValueChanged += nudGuests_ValueChanged;
        }

        private void BtnCheckOut_Click(object? sender, EventArgs e)
        {
            PerformCheckOut();
        }

        private void BtnCancelNoShow_Click(object? sender, EventArgs e)
        {
            CancelExpiredCheckIns();
        }

        private void BtnCheckIn_Click(object? sender, EventArgs e)
        {
            PerformCheckIn();
        }

        /// <summary>
        /// Carrega a lista de hóspedes existentes na ComboBox cmbGuest.
        /// </summary>
        private void LoadGuestComboBox()
        {
            List<Guest> guests = new CrudGuests().GetAll();

            cmbGuest.Items.Clear();

            foreach (var guest in guests)
            {
                cmbGuest.Items.Add($"{guest.IdentificationDocument} - {guest.Name}"); //Para precaver a existência de dois nomes de guests iguais
            }

            // Deixa sem seleção inicial — o utilizador deve escolher
            cmbGuest.SelectedIndex = -1;
        }

        /// <summary>
        /// Carrega os valores disponíveis do enum RoomType na ComboBox de tipos de quarto.
        /// Remove previamente todos os itens existentes e adiciona cada valor do enum.
        /// </summary>
        private void LoadRoomTypesComboBox()
        {
            cmbRoomType.Items.Clear();
            foreach (var type in Enum.GetValues(typeof(RoomType)))
            {
                cmbRoomType.Items.Add(type);
            }
        }

        /// <summary>
        /// Carrega os quartos disponíveis com base no tipo de quarto selecionado,
        /// número de ocupantes indicado e datas da reserva. Exclui quartos ocupados, em manutenção ou reservados.
        /// </summary>
        private void LoadRoomComboBox()
        {
            cmbRoom.DataSource = null;
            cmbRoom.Items.Clear();

            if (cmbRoomType.SelectedItem == null)
                return;

            RoomType selectedType = (RoomType)cmbRoomType.SelectedItem;
            int guests = (int)nudGuests.Value;
            DateTime checkIn = dtpCheckIn.Value.Date;
            DateTime checkOut = dtpCheckOut.Value.Date;

            var allRooms = new CrudRooms().GetAll()
                .Where(r => r.Type == selectedType && r.Capacity >= guests)
                .ToList();

            var allBookings = new CrudBookings().GetAll();

            List<Room> availableRooms = new List<Room>();

            foreach (var room in allRooms)
            {
                bool isSameRoomBeingEdited = selectedBookingId != -1 &&
                                             allBookings.Any(b => b.BookingId == selectedBookingId && b.RoomId == room.RoomId);

                bool hasConflict = allBookings.Any(b =>
                    b.RoomId == room.RoomId &&
                    b.BookingId != selectedBookingId &&
                    b.Status != BookingStatus.Cancelled &&
                    checkIn < b.CheckOutDate &&
                    checkOut > b.CheckInDate);

                bool isRoomUsable = room.Status == RoomStatus.Available;
                bool canAddRoom = (isRoomUsable || isSameRoomBeingEdited) && !hasConflict;

                if (canAddRoom)
                {
                    availableRooms.Add(room);
                }
            }

            if (availableRooms.Count == 0)
            {
                cmbRoom.Items.Add("No rooms available");
                cmbRoom.SelectedIndex = 0;
                return;
            }

            cmbRoom.DataSource = availableRooms;
            cmbRoom.DisplayMember = "Number";
            cmbRoom.ValueMember = "RoomId";
            cmbRoom.SelectedIndex = -1;
        }


        /// <summary>
        /// Cancela automaticamente reservas que não fizeram check-in na data prevista.
        /// </summary>
        private void CancelExpiredCheckIns()
        {
            var bookings = crud.GetAll();
            var expired = new List<Booking>();

            foreach (var b in bookings)
            {
                if (b.Status == BookingStatus.Reserved && b.CheckInDate < DateTime.Today)
                {
                    expired.Add(b);
                }
            }

            if (expired.Count == 0)
            {
                MessageBox.Show("There are no bookings to cancel.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var crudRooms = new CrudRooms();
            var rooms = crudRooms.GetAll();

            foreach (var booking in expired)
            {
                booking.Status = BookingStatus.Cancelled;
                crud.Update(booking);

                var room = rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
                if (room != null)
                {
                    room.Status = RoomStatus.Available;
                    crudRooms.Update(room);
                }
            }

            MessageBox.Show($"{expired.Count} booking(s) cancelled due to no-show.", "Bookings Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }

        /// <summary>
        /// Realiza o check-in para a reserva selecionada, se for possível.
        /// </summary>
        private void PerformCheckIn()
        {
            if (selectedBookingId == -1)
            {
                MessageBox.Show("Please select a booking to check in.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bookings = crud.GetAll();
            Booking booking = null;
            foreach (var b in bookings)
            {
                if (b.BookingId == selectedBookingId)
                {
                    booking = b;
                    break;
                }
            }

            if (booking == null)
            {
                MessageBox.Show("Booking not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (booking.Status != BookingStatus.Reserved)
            {
                MessageBox.Show("Only bookings with status 'Reserved' can be checked in.", "Invalid Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (booking.CheckInDate.Date != DateTime.Today)
            {
                MessageBox.Show("Check-in is only allowed on the scheduled check-in date.", "Check-in Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            booking.Status = BookingStatus.CheckedIn;
            crud.Update(booking);

            MessageBox.Show("Check-in completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }

        /// <summary>
        /// Realiza o check-out para a reserva selecionada, se for possível.
        /// </summary>
        private void PerformCheckOut()
        {
            if (selectedBookingId == -1)
            {
                MessageBox.Show("Please select a booking to check out.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var bookings = crud.GetAll();
            Booking booking = bookings.FirstOrDefault(b => b.BookingId == selectedBookingId);

            if (booking == null)
            {
                MessageBox.Show("Booking not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (booking.Status != BookingStatus.CheckedIn)
            {
                MessageBox.Show("Only bookings with status 'CheckedIn' can be checked out.", "Invalid Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (DateTime.Today < booking.CheckInDate.Date || DateTime.Today > booking.CheckOutDate.Date)
            {
                MessageBox.Show("Check-out is only allowed between check-in and check-out dates.", "Check-out Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            booking.Status = BookingStatus.Completed;
            crud.Update(booking);

            var crudRooms = new CrudRooms();
            var rooms = crudRooms.GetAll();

            var room = rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            if (room != null)
            {
                room.Status = RoomStatus.Available;
                crudRooms.Update(room);
            }

            MessageBox.Show("Check-out completed and room made available.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }


        /// <summary>
        /// Carrega os dados da reserva selecionada para edição.
        /// </summary>
        private void btnEditBooking_Click(object sender, EventArgs e)
        {
            if (selectedBookingId == -1)
            {
                MessageBox.Show("Please select a booking to edit.");
                return;
            }

            LoadGuestComboBox();         // carrega os hóspedes
            LoadRoomTypesComboBox();     // carrega tipos de quarto

            LoadSelectedBookingData();   // carrega os dados da reserva

            lblAddOrEdit.Text = "Edit Booking";
            pnlAddBooking.Visible = true;
        }

        /// <summary>
        /// Remove a reserva selecionada da lista e do ficheiro.
        /// </summary>
        private void btnDeleteBooking_Click(object sender, EventArgs e)
        {
            // Verifica se foi selecionada uma reserva
            if (selectedBookingId == -1)
            {
                MessageBox.Show("Please select a booking to delete.",
                    "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirmação do utilizador
            var confirm = MessageBox.Show("Are you sure you want to delete this booking?",
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            // Obtém a reserva a eliminar
            Booking bookingToDelete = crud.GetAll()
                .FirstOrDefault(b => b.BookingId == selectedBookingId);

            if (bookingToDelete == null)
            {
                MessageBox.Show("Booking not found.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Atualiza o estado do quarto associado para "Available"
            CrudRooms crudRooms = new CrudRooms();
            Room roomToUpdate = crudRooms.GetAll()
                .FirstOrDefault(r => r.RoomId == bookingToDelete.RoomId); // ✅ Correto agora

            if (roomToUpdate != null)
            {
                roomToUpdate.Status = RoomStatus.Available;
                crudRooms.Update(roomToUpdate);
            }

            // Elimina a reserva
            crud.Delete(bookingToDelete.BookingId);

            // Atualiza a interface
            LoadBookings();
            ClearFields();
            pnlAddBooking.Visible = false;

            MessageBox.Show("Booking deleted and room status updated.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Adiciona ou atualiza a reserva no ficheiro.
        /// </summary>
        private void btnSaveBooking_Click(object sender, EventArgs e)
        {
            // Verificação inicial dos campos obrigatórios
            if (cmbGuest.SelectedItem == null || cmbRoom.SelectedItem == null)
            {
                MessageBox.Show("Please select a guest and a room.",
                    "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Extrai o documento do hóspede a partir da string do ComboBox
            string selectedGuestText = cmbGuest.SelectedItem.ToString();
            string guestDoc = selectedGuestText.Split('-')[0].Trim();

            Guest selectedGuest = new CrudGuests().GetAll()
                .FirstOrDefault(g => g.IdentificationDocument == guestDoc);

            if (selectedGuest == null)
            {
                MessageBox.Show("Selected guest could not be found.",
                    "Guest Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Obter o quarto selecionado a partir do ComboBox
            Room selectedRoom = cmbRoom.SelectedItem as Room;

            if (selectedRoom == null)
            {
                MessageBox.Show("Selected room could not be found.",
                    "Room Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cria a instância da reserva com os dados preenchidos
            Booking booking = new Booking
            {
                BookingId = selectedBookingId == -1 ? GenerateBookingId() : selectedBookingId,
                GuestId = selectedGuest.GuestId,
                RoomId = selectedRoom.RoomId, // ✅ agora usa o ID real
                ReservationDate = DateTime.Now,
                CheckInDate = dtpCheckIn.Value,
                CheckOutDate = dtpCheckOut.Value,
                NumberOfGuests = (int)nudGuests.Value
            };

            // Verifica conflitos de datas para o mesmo quarto
            if (HasBookingConflict(booking))
            {
                MessageBox.Show("This room is already booked during the selected period.",
                    "Booking Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validação geral da reserva
            if (!ValidationHelper.ValidateBooking(booking, out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Guarda ou atualiza consoante o modo atual
            if (selectedBookingId == -1)
                crud.Add(booking);
            else
                crud.Update(booking);

            // Atualiza o estado do quarto para reservado
            selectedRoom.Status = RoomStatus.Reserved;
            new CrudRooms().Update(selectedRoom);

            // Atualiza a interface
            LoadBookings();
            ClearFields();
            pnlAddBooking.Visible = false;
        }

        /// <summary>
        /// Limpa os campos e esconde o painel de edição.
        /// </summary>
        private void btnCancelBooking_Click(object sender, EventArgs e)
        {
            ClearFields();
            pnlAddBooking.Visible = false;
        }

        /// <summary>
        /// Carrega os dados da reserva selecionada nos campos da dataGridView quando a seleção da grelha muda
        /// </summary>
        private void dgvBookings_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvBookings.SelectedRows.Count == 0)
                return;

            var row = dgvBookings.SelectedRows[0];
            if (row.IsNewRow) return;

            // Obtem os dados da reserva
            selectedBookingId = Convert.ToInt32(row.Cells[0].Value);
            string guestName = row.Cells[1].Value.ToString();
            string guestDoc = row.Cells[2].Value.ToString();
            int roomId = Convert.ToInt32(row.Cells[3].Value); // ⚠️ Agora é RoomId, não Number
            int numGuests = Convert.ToInt32(row.Cells[7].Value);

            // Seleciona o hóspede no ComboBox
            var guest = new CrudGuests().GetAll()
                .FirstOrDefault(g => g.IdentificationDocument == guestDoc);

            if (guest != null)
            {
                for (int i = 0; i < cmbGuest.Items.Count; i++)
                {
                    if (cmbGuest.Items[i].ToString().StartsWith(guest.IdentificationDocument))
                    {
                        cmbGuest.SelectedIndex = i;
                        break;
                    }
                }
            }
            else
            {
                cmbGuest.SelectedIndex = -1;
            }

            // Atualiza número de hóspedes
            nudGuests.Value = numGuests;

            // Obtem o quarto com base no RoomId
            var room = new CrudRooms().GetAll().FirstOrDefault(r => r.RoomId == roomId);

            if (room != null)
            {
                // Seleciona o tipo e carrega os quartos disponíveis
                cmbRoomType.SelectedItem = room.Type;
                LoadRoomComboBox();

                // Seleciona o quarto no ComboBox pela RoomId
                cmbRoom.SelectedValue = room.RoomId;
            }
            else
            {
                cmbRoomType.SelectedIndex = -1;
                cmbRoom.Items.Clear();
            }

            // Preenche as datas
            dtpCheckIn.Value = DateTime.Parse(row.Cells[5].Value.ToString());
            dtpCheckOut.Value = DateTime.Parse(row.Cells[6].Value.ToString());

            // Mostra o painel de edição
            lblAddOrEdit.Text = "Edit Booking";

            LoadSelectedBookingData();
        }

        /// <summary>
        /// Carrega os dados da reserva selecionada da grelha para os controlos do formulário.
        /// </summary>
        private void LoadSelectedBookingData()
        {
            if (dgvBookings.SelectedRows.Count == 0)
                return;

            var row = dgvBookings.SelectedRows[0];
            if (row.IsNewRow || row.Cells[0].Value == null)
                return;

            // ID da reserva
            selectedBookingId = Convert.ToInt32(row.Cells[0].Value);

            // Documento do hóspede e RoomId (não RoomNumber)
            string guestDoc = row.Cells[2].Value.ToString();
            int roomId = Convert.ToInt32(row.Cells[3].Value);
            int numGuests = Convert.ToInt32(row.Cells[7].Value);

            // Datas
            dtpCheckIn.Value = DateTime.Parse(row.Cells[5].Value.ToString());
            dtpCheckOut.Value = DateTime.Parse(row.Cells[6].Value.ToString());

            // Atualiza a label da data de reserva
            lblReservationDate.Text = DateTime.Parse(row.Cells[4].Value.ToString()).ToString("dd/MM/yyyy HH:mm");

            // Seleciona o hóspede
            var guest = new CrudGuests().GetAll()
                .FirstOrDefault(g => g.IdentificationDocument == guestDoc);

            if (guest != null)
            {
                for (int i = 0; i < cmbGuest.Items.Count; i++)
                {
                    if (cmbGuest.Items[i].ToString().StartsWith(guest.IdentificationDocument))
                    {
                        cmbGuest.SelectedIndex = i;
                        break;
                    }
                }
            }

            // Ocupantes
            nudGuests.Value = numGuests;

            // Obtem o quarto com base no RoomId
            var room = new CrudRooms().GetAll().FirstOrDefault(r => r.RoomId == roomId);

            if (room != null)
            {
                cmbRoomType.SelectedItem = room.Type;

                // Só agora que o tipo de quarto e nº hóspedes estão definidos, carregamos os disponíveis
                LoadRoomComboBox();

                // Seleciona diretamente com base no ID
                cmbRoom.SelectedValue = room.RoomId;
            }
        }


        /// <summary>
        /// Limpa os campos do formulário de reserva e o ID selecionado.
        /// </summary>
        private void ClearFields()
        {
            cmbGuest.SelectedIndex = -1;
            cmbRoomType.SelectedIndex = -1;
            cmbRoom.Items.Clear();
            nudGuests.Value = 1;
            dtpCheckIn.Value = DateTime.Today;
            dtpCheckOut.Value = DateTime.Today.AddDays(1);
            selectedBookingId = -1;
            lblReservationDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        /// <summary>
        /// Gera um novo ID para a reserva com base no maior ID atual.
        /// </summary>
        /// <returns>Novo ID único.</returns>
        private int GenerateBookingId()
        {
            var bookings = crud.GetAll();
            return bookings.Count == 0 ? 1 : bookings.Max(b => b.BookingId) + 1;
        }

        /// <summary>
        /// Verifica se existe uma sobreposição de datas de reserva para o mesmo quarto
        /// </summary>
        /// <param name="booking">Verifica se existe uma sobreposição de datas</param>
        /// <returns>True se houver conflito de datas; caso contrário, False.</returns>
        private bool HasBookingConflict(Booking booking)
        {
            // Obtém todas as reservas que usam o mesmo quarto, exceto a atual (no caso de edição)
            var existingBookings = crud.GetAll()
                .Where(b => b.RoomId == booking.RoomId && b.BookingId != booking.BookingId)
                .ToList();

            // Verifica se existe alguma sobreposição de datas
            return existingBookings.Any(b =>
                booking.CheckInDate < b.CheckOutDate &&
                booking.CheckOutDate > b.CheckInDate);
        }

        /// <summary>
        /// Prepara o formulário para adicionar uma nova reserva.
        /// </summary>
        private void btnAddBooking_Click(object sender, EventArgs e)
        {

            ClearFields();
            LoadGuestComboBox();
            LoadRoomTypesComboBox();
            lblAddOrEdit.Text = "Add New Booking";
            lblReservationDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            pnlAddBooking.Visible = true;
        }

        /// <summary>
        /// Sempre que o tipo de quarto é alterado, reinicia o número de ocupantes,
        /// atualiza a lista de quartos disponíveis e limpa a seleção atual.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reinicia o número de ocupantes para 1
            nudGuests.Value = 1;

            // Recarrega os quartos do tipo selecionado com capacidade >= 1
            LoadRoomComboBox();

            // Nenhum quarto selecionado por defeito após a mudança de tipo
            cmbRoom.SelectedIndex = -1;

        }

        /// <summary>
        /// Quando o número de hóspedes é alterado, atualiza a lista de quartos
        /// disponíveis e limpa a seleção anterior.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nudGuests_ValueChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de quartos com base no novo número de ocupantes
            LoadRoomComboBox();

            // Limpa a seleção do quarto atual
            cmbRoom.SelectedIndex = -1;

        }
    }
}
