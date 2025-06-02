using HotelManagement.Core;
using HotelManagementSystem.Logic;

namespace HotelManagementApp.UserControls
{
    /// <summary>
    /// UserControl responsável pela gestão de hóspedes: adicionar, editar, remover e listar.
    /// </summary>
    public partial class UCGuests : UserControl
    {
        private CrudBookings crudBookings = new CrudBookings();
        private CrudGuests crud = new CrudGuests();
        private int selectedGuestId = -1; // -1 indica que nenhum hóspede está selecionado

        /// <summary>
        /// Construtor: inicializa o componente, carrega os hóspedes e associa os eventos.
        /// </summary>
        public UCGuests()
        {
            InitializeComponent();
            ConfigureFilterComboBox();
            LoadGuests();
            AttachEvents();
        }

        /// <summary>
        /// Cria e configura a ComboBox de filtros de hóspedes.
        /// </summary>
        private void ConfigureFilterComboBox()
        {
            cmbGuestFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGuestFilter.Items.Clear();
            cmbGuestFilter.Items.AddRange(new object[]
            {
                "All Guests",
                "With Active Bookings",
                "With Stay History"
            });
            cmbGuestFilter.SelectedIndex = 0;
            cmbGuestFilter.SelectedIndexChanged += cmbGuestFilter_SelectedIndexChanged;
        }

        /// <summary>
        /// Carrega todos os hóspedes da base de dados.
        /// </summary>
        private void LoadGuests()
        {
            var guests = crud.GetAll();
            LoadGuests(guests);
        }

        /// <summary>
        /// Carrega uma lista específica de hóspedes na grelha.
        /// </summary>
        /// <param name="guests">Lista de hóspedes a apresentar.</param>
        private void LoadGuests(List<Guest> guests)
        {
            dgvGuests.Columns.Clear();
            dgvGuests.Rows.Clear();

            dgvGuests.Columns.Add("GuestId", "Unique Identifier");
            dgvGuests.Columns.Add("Name", "Name");
            dgvGuests.Columns.Add("Contact", "Contact");
            dgvGuests.Columns.Add("Email", "Email");
            dgvGuests.Columns.Add("IdentificationDocument", "Identification Document");

            foreach (var guest in guests)
            {
                dgvGuests.Rows.Add(
                    guest.GuestId,
                    guest.Name,
                    guest.Contact,
                    guest.Email,
                    guest.IdentificationDocument);
            }
        }

        /// <summary>
        /// Liga os controlos da interface aos respetivos eventos.
        /// </summary>
        private void AttachEvents()
        {
            btnAddGuest.Click += btnAddGuest_Click;
            btnEditGuest.Click += btnEditGuest_Click;
            btnDeleteGuest.Click += btnDeleteGuest_Click;
            dgvGuests.SelectionChanged += dgvGuests_SelectionChanged;
            btnSaveGuest.Click += btnSaveGuest_Click;
            btnCancelGuest.Click += btnCancelGuest_Click;
            btnViewHistory.Click += btnViewHistory_Click;
        }

        /// <summary>
        /// Aplica o filtro selecionado na ComboBox aos hóspedes listados.
        /// </summary>
        private void ApplyGuestFilter()
        {
            var allGuests = crud.GetAll();
            var allBookings = crudBookings.GetAll();

            switch (cmbGuestFilter.SelectedIndex)
            {
                case 0: // Todos os hóspedes
                    LoadGuests(allGuests);
                    break;

                case 1: // Hóspedes com reservas ativas
                    var activeGuests = GuestHistoryHelper.GetGuestsWithActiveBookings(allBookings, allGuests);
                    LoadGuests(activeGuests);
                    break;

                case 2: // Hóspedes com histórico de estadias
                    var guestsWithHistory = GuestHistoryHelper.GetGuestsWithStayHistory(allBookings, allGuests);
                    LoadGuests(guestsWithHistory);
                    break;
            }
        }

        /// <summary>
        /// Evento acionado quando o filtro de hóspedes é alterado.
        /// </summary>
        private void cmbGuestFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyGuestFilter();
        }

        /// <summary>
        /// Prepara a interface para adicionar um novo hóspede.
        /// </summary>
        private void btnAddGuest_Click(object sender, EventArgs e)
        {
            ClearFields();
            lblAddOrEdit.Text = "Add New Guest";
            pnlAddGuest.Visible = true;
        }

        /// <summary>
        /// Prepara a interface para editar o hóspede selecionado.
        /// </summary>
        private void btnEditGuest_Click(object sender, EventArgs e)
        {
            if (selectedGuestId == -1)
            {
                MessageBox.Show("Select a Guest to edit.");
                return;
            }

            lblAddOrEdit.Text = "Edit Guest";
            pnlAddGuest.Visible = true;
        }

        /// <summary>
        /// Remove o hóspede selecionado, caso não tenha reservas ativas.
        /// </summary>
        private void btnDeleteGuest_Click(object sender, EventArgs e)
        {
            if (selectedGuestId == -1)
            {
                MessageBox.Show("Select a Guest to remove.");
                return;
            }

            if (!crud.CanDelete(selectedGuestId))
            {
                MessageBox.Show("This Guest cannot be removed because they have active bookings..",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show("Are you sure you want to remove this guest?",
                "Confirmar remoção", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                crud.Delete(selectedGuestId);
                ApplyGuestFilter();
                ClearFields();
            }
        }

        /// <summary>
        /// Guarda um novo hóspede ou atualiza um hóspede existente.
        /// </summary>
        private void btnSaveGuest_Click(object sender, EventArgs e)
        {
            var guest = new Guest
            {
                GuestId = selectedGuestId == -1 ? GenerateGuestId() : selectedGuestId,
                Name = txtGuestName.Text.Trim(),
                Contact = txtGuestContact.Text.Trim(),
                Email = txtGuestEmail.Text.Trim(),
                IdentificationDocument = txtGuestDocument.Text.Trim()
            };

            if (!ValidationHelper.ValidateGuest(guest, out string erro))
            {
                MessageBox.Show(erro, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (selectedGuestId == -1)
                crud.Add(guest);
            else
                crud.Update(guest);

            ApplyGuestFilter();
            ClearFields();
            pnlAddGuest.Visible = false;
        }

        /// <summary>
        /// Cancela a operação de adição ou edição.
        /// </summary>
        private void btnCancelGuest_Click(object sender, EventArgs e)
        {
            ClearFields();
            pnlAddGuest.Visible = false;
        }

        /// <summary>
        /// Atualiza os campos de texto com os dados do hóspede selecionado.
        /// </summary>
        private void dgvGuests_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvGuests.SelectedRows.Count == 0)
                return;

            var row = dgvGuests.SelectedRows[0];

            // Garante que a linha não é a linha vazia da DataGridView
            if (row.IsNewRow)
                return;

            // Verifica se todas as células têm valores válidos
            for (int i = 0; i <= 4; i++)
            {
                if (row.Cells[i].Value == null)
                    return;
            }

            // Preenche os campos com os valores da linha
            selectedGuestId = Convert.ToInt32(row.Cells[0].Value);
            txtGuestName.Text = row.Cells[1].Value.ToString();
            txtGuestContact.Text = row.Cells[2].Value.ToString();
            txtGuestEmail.Text = row.Cells[3].Value.ToString();
            txtGuestDocument.Text = row.Cells[4].Value.ToString();
        }

        /// <summary>
        /// Limpa os campos da interface e o identificador selecionado.
        /// </summary>
        private void ClearFields()
        {
            txtGuestName.Clear();
            txtGuestContact.Clear();
            txtGuestEmail.Clear();
            txtGuestDocument.Clear();
            selectedGuestId = -1;
        }

        /// <summary>
        /// Gera um novo ID único para o hóspede.
        /// </summary>
        private int GenerateGuestId()
        {
            var allGuests = crud.GetAll();
            return allGuests.Count == 0 ? 1 : allGuests.Max(g => g.GuestId) + 1;
        }

        /// <summary>
        /// Mostra o histórico de estadias do hóspede selecionado.
        /// </summary>
        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (selectedGuestId == -1)
            {
                MessageBox.Show("Please select a guest first.", "No Guest Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var allBookings = crudBookings.GetAll();
            var history = GuestHistoryHelper.GetBookingHistoryForGuest(selectedGuestId, allBookings);

            if (history.Count == 0)
            {
                MessageBox.Show("This guest has no previous stays.", "No History", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string message = "Stay history:\n\n";

            foreach (var booking in history)
            {
                message += $"- From {booking.CheckInDate:dd/MM/yyyy} to {booking.CheckOutDate:dd/MM/yyyy}, Room {booking.RoomId}\n";
            }

            MessageBox.Show(message, "Guest Stay History", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}