using HotelManagement.Core;
using HotelManagementSystem.Logic;
using System.Data;

namespace HotelManagementApp.UserControls
{
    /// <summary>
    /// UserControl responsável pela gestão de serviços extra associados a reservas.
    /// </summary>
    public partial class UCExtras : UserControl
    {
        private bool isEditing = false;
        private int editingServiceId = -1;
        private CrudExtras crudExtras = new CrudExtras();

        public UCExtras()
        {
            InitializeComponent();
            LoadBookings();
            LoadServiceTypes();
            LoadAllExtras();
            AttachEvents();
        }

        /// <summary>
        /// Carrega as reservas disponíveis na ComboBox.
        /// </summary>
        private void LoadBookings()
        {
            var crudBookings = new CrudBookings();
            var crudGuests = new CrudGuests();
            var crudRooms = new CrudRooms();

            var bookings = crudBookings.GetAll();
            var guests = crudGuests.GetAll();
            var rooms = crudRooms.GetAll();

            var items = bookings.Select(b =>
            {
                var guest = guests.FirstOrDefault(g => g.GuestId == b.GuestId);
                var room = rooms.FirstOrDefault(r => r.RoomId == b.RoomId);

                string guestName = guest?.Name ?? "Unknown Guest";
                string roomNumber = room?.Number.ToString() ?? "N/A";

                return new
                {
                    Id = b.BookingId,
                    Display = $"ID {b.BookingId} - {guestName} - Room {roomNumber} ({b.CheckInDate:dd/MM} - {b.CheckOutDate:dd/MM})"
                };
            }).ToList();

            cmbBooking.DataSource = items;
            cmbBooking.DisplayMember = "Display";
            cmbBooking.ValueMember = "Id";
        }

        /// <summary>
        /// Carrega os tipos de serviços extra (enum) na ComboBox.
        /// </summary>
        private void LoadServiceTypes()
        {
            cmbServiceType.DataSource = Enum.GetValues(typeof(ExtraServiceType));
        }

        /// <summary>
        /// Liga os eventos aos respetivos botões.
        /// </summary>
        private void AttachEvents()
        {
            btnAddService.Click += btnAddService_Click;
            btnEditService.Click += btnEditService_Click;
            btnDeleteService.Click += btnDeleteService_Click;
            btnSaveService.Click += btnSaveService_Click;
            btnCancel.Click += btnCancel_Click;
            cmbBooking.SelectedIndexChanged += cmbBooking_SelectedIndexChanged;
        }

        /// <summary>
        /// Abre o painel para adicionar um novo serviço.
        /// </summary>
        private void btnAddService_Click(object sender, EventArgs e)
        {
            lblAddOrEdit.Text = "Add Extra Service";
            ClearFields();
            pnlAddExtraService.Visible = true;
            isEditing = false;
        }

        /// <summary>
        /// Abre o painel para editar o serviço selecionado.
        /// </summary>
        private void btnEditService_Click(object sender, EventArgs e)
        {
            if (dgvExtras.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a service to edit.");
                return;
            }

            var row = dgvExtras.SelectedRows[0];
            editingServiceId = int.Parse(row.Cells["ServiceId"].Value.ToString());
            cmbServiceType.SelectedItem = Enum.Parse(typeof(ExtraServiceType), row.Cells["ServiceType"].Value.ToString());
            txtPrice.Text = row.Cells["Price"].Value.ToString();

            lblAddOrEdit.Text = "Edit Extra Service";
            pnlAddExtraService.Visible = true;
            isEditing = true;
        }

        /// <summary>
        /// Elimina o serviço extra selecionado.
        /// </summary>
        private void btnDeleteService_Click(object sender, EventArgs e)
        {
            if (dgvExtras.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a service to delete.");
                return;
            }

            int id = int.Parse(dgvExtras.SelectedRows[0].Cells["ServiceId"].Value.ToString());

            var confirm = MessageBox.Show("Are you sure you want to delete this service?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                crudExtras.Delete(id);
                LoadAllExtras();
            }
        }

        /// <summary>
        /// Cancela a adição ou edição do serviço.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlAddExtraService.Visible = false;
            ClearFields();
        }

        /// <summary>
        /// Guarda um novo serviço ou atualiza um existente.
        /// </summary>
        private void btnSaveService_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(cmbBooking.SelectedValue?.ToString(), out int bookingId) ||
                !(cmbServiceType.SelectedItem is ExtraServiceType selectedServiceType) ||
                !decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Please fill in all fields correctly.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var servico = new ExtraService
            {
                ExtraServiceId = isEditing ? editingServiceId : crudExtras.GenerateNextId(),
                BookingId = bookingId,
                ServiceType = selectedServiceType,
                Price = price
            };

            if (!ValidationHelper.ValidateExtraService(servico, out string erro))
            {
                MessageBox.Show("Validation error: " + erro);
                return;
            }

            if (isEditing)
            {
                crudExtras.Update(servico);
                MessageBox.Show("Service updated successfully.");
            }
            else
            {
                crudExtras.Add(servico);
                MessageBox.Show("Service added successfully.");
            }

            pnlAddExtraService.Visible = false;
            LoadAllExtras();
            ClearFields();
            isEditing = false;
        }

        /// <summary>
        /// Quando o utilizador muda a reserva, carrega os serviços extra associados.
        /// </summary>
        private void cmbBooking_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAllExtras();
        }

        /// <summary>
        /// Carrega os serviços extra da reserva selecionada na grelha.
        /// </summary>
        private void LoadAllExtras()
        {
            var extras = crudExtras.GetAll();

            dgvExtras.Columns.Clear();
            dgvExtras.Rows.Clear();

            dgvExtras.Columns.Add("ServiceId", "Service ID");
            dgvExtras.Columns.Add("BookingId", "Booking ID");
            dgvExtras.Columns.Add("GuestName", "Guest");
            dgvExtras.Columns.Add("ServiceType", "Service Type");
            dgvExtras.Columns.Add("Price", "Price (€)");

            CrudBookings gestorReservas = new CrudBookings();
            CrudGuests gestorHospedes = new CrudGuests();

            List<Booking> todasAsReservas = gestorReservas.GetAll();
            List<Guest> todosOsHospedes = gestorHospedes.GetAll();

            foreach (ExtraService extra in extras)
            {
                string nomeHospede = "Unknown";

                foreach (Booking reserva in todasAsReservas)
                {
                    if (reserva.BookingId == extra.BookingId)
                    {
                        foreach (Guest hospede in todosOsHospedes)
                        {
                            if (hospede.GuestId == reserva.GuestId)
                            {
                                nomeHospede = hospede.Name;
                                break;
                            }
                        }
                        break;
                    }
                }

                dgvExtras.Rows.Add(extra.ExtraServiceId, extra.BookingId, nomeHospede, extra.ServiceType.ToString(), extra.Price.ToString("F2"));
            }
        }

        /// <summary>
        /// Limpa os campos do painel.
        /// </summary>
        private void ClearFields()
        {
            cmbServiceType.SelectedIndex = -1;
            txtPrice.Clear();
        }
    }
}
