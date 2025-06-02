using HotelManagement.Core;
using HotelManagementSystem.Logic;
using System.Data;

namespace HotelManagementApp.UserControls
{
    /// <summary>
    /// UserControl responsável pela gestão de quartos: adicionar, editar, remover, listar e filtrar.
    /// </summary>
    public partial class UCRooms : UserControl
    {
        private CrudRooms crud = new CrudRooms();
        private bool isEditing = false;
        private int editingRoomId = -1;

        public UCRooms()
        {
            InitializeComponent();
            LoadRooms();
            LoadAddRoomComboBoxes();
            LoadFilterComboBoxes();
            AttachEvents();
            AttachFilterEvents();
        }

        /// <summary>
        /// Carrega todos os quartos na DataGridView.
        /// </summary>
        public void LoadRooms()
        {
            dgvRooms.Columns.Clear();
            dgvRooms.Rows.Clear();

            dgvRooms.Columns.Add("Number", "Number");
            dgvRooms.Columns.Add("Type", "Type");
            dgvRooms.Columns.Add("Capacity", "Capacity");
            dgvRooms.Columns.Add("PricePerNight", "Price");
            dgvRooms.Columns.Add("Status", "Status");

            // Coluna invisível para RoomId
            dgvRooms.Columns.Add("RoomId", "Room ID");
            dgvRooms.Columns["RoomId"].Visible = false;

            var rooms = crud.GetAll();

            foreach (var room in rooms)
            {
                dgvRooms.Rows.Add(
                    room.Number,
                    room.Type,
                    room.Capacity,
                    room.PricePerNight.ToString("F2"),
                    room.Status,
                    room.RoomId // valor técnico interno
                );
            }
        }

        /// <summary>
        /// Carrega as ComboBoxes de adição com os valores dos enums.
        /// </summary>
        private void LoadAddRoomComboBoxes()
        {
            PopulateComboBoxWithEnum<RoomType>(cmbAddRoomType);
            PopulateComboBoxWithEnum<RoomStatus>(cmbAddRoomStatus);
        }

        /// <summary>
        /// Carrega as ComboBoxes de filtro com os valores dos enums e a opção "All".
        /// </summary>
        private void LoadFilterComboBoxes()
        {
            PopulateComboBoxWithEnum<RoomType>(cmbFilterRoomType, includeAll: true);
            PopulateComboBoxWithEnum<RoomStatus>(cmbFilterRoomStatus, includeAll: true);
        }

        /// <summary>
        /// Preenche uma ComboBox com os valores de uma enumeração.
        /// </summary>
        /// <typeparam name="TEnum">Tipo de enumeração.</typeparam>
        /// <param name="comboBox">ComboBox a preencher.</param>
        /// <param name="includeAll">Se deve incluir a opção "All".</param>
        private void PopulateComboBoxWithEnum<TEnum>(ComboBox comboBox, bool includeAll = false) where TEnum : Enum
        {
            comboBox.Items.Clear();
            if (includeAll)
                comboBox.Items.Add("All");
            foreach (var item in Enum.GetValues(typeof(TEnum)))
                comboBox.Items.Add(item);
            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Aplica os filtros de tipo, estado e preço máximo aos quartos e atualiza a grelha.
        /// </summary>
        private void ApplyFilters()
        {
            // Obter o tipo de quarto selecionado
            bool filtrarTipo = cmbFilterRoomType.SelectedItem.ToString() != "All";
            RoomType selectedType = RoomType.Standard; // Valor por defeito
            if (filtrarTipo)
            {
                selectedType = (RoomType)Enum.Parse(typeof(RoomType), cmbFilterRoomType.SelectedItem.ToString());
            }

            // Obter o estado do quarto selecionado
            bool filtrarEstado = cmbFilterRoomStatus.SelectedItem.ToString() != "All";
            RoomStatus selectedStatus = RoomStatus.Available; // Valor por defeito
            if (filtrarEstado)
            {
                selectedStatus = (RoomStatus)Enum.Parse(typeof(RoomStatus), cmbFilterRoomStatus.SelectedItem.ToString());
            }

            // Obter o valor máximo de preço
            bool filtrarPreco = nudFilterMaxPrice.Value > 0;
            decimal maxPrice = nudFilterMaxPrice.Value;

            // Obter todos os quartos
            var todosOsQuartos = crud.GetAll();

            // Aplicar filtros manualmente
            List<Room> quartosFiltrados = new List<Room>();
            foreach (var room in todosOsQuartos)
            {
                if (filtrarTipo && room.Type != selectedType)
                    continue;

                if (filtrarEstado && room.Status != selectedStatus)
                    continue;

                if (filtrarPreco && room.PricePerNight > maxPrice)
                    continue;

                quartosFiltrados.Add(room);
            }

            // Atualizar a grelha
            dgvRooms.Rows.Clear();
            foreach (var room in quartosFiltrados)
            {
                dgvRooms.Rows.Add(
                    room.Number,
                    room.Type,
                    room.Capacity,
                    room.PricePerNight.ToString("F2"),
                    room.Status);
            }
        }

        /// <summary>
        /// Liga os botões da interface aos respetivos eventos.
        /// </summary>
        private void AttachEvents()
        {
            btnAddRoom.Click += btnAddRoom_Click;
            btnEditRoom.Click += btnEditRoom_Click;
            btnDeleteRoom.Click += btnDeleteRoom_Click;
            btnSaveAddRoom.Click += btnSaveAddRoom_Click;
            btnCancelAddRoom.Click += btnCancelAddRoom_Click;
        }

        /// <summary>
        /// Liga os eventos de alteração dos filtros aos métodos respetivos.
        /// </summary>
        private void AttachFilterEvents()
        {
            cmbFilterRoomType.SelectedIndexChanged += cmbFilterRoomType_SelectedIndexChanged;
            cmbFilterRoomStatus.SelectedIndexChanged += cmbFilterRoomStatus_SelectedIndexChanged;
            nudFilterMaxPrice.ValueChanged += nudFilterMaxPrice_ValueChanged;
        }

        /// <summary>
        /// Evento acionado quando o filtro de tipo de quarto é alterado.
        /// </summary>
        private void cmbFilterRoomType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Evento acionado quando o filtro de estado do quarto é alterado.
        /// </summary>
        private void cmbFilterRoomStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Evento acionado quando o filtro de preço máximo é alterado.
        /// </summary>
        private void nudFilterMaxPrice_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        /// <summary>
        /// Prepara a interface para adicionar um novo quarto.
        /// </summary>
        private void btnAddRoom_Click(object sender, EventArgs e)
        {
            lblAddOrEdit.Text = "Add New Room";
            ClearFields();
            pnlAddRoom.Visible = true;
            txtAddRoomNumber.Enabled = true;
            isEditing = false;
        }

        /// <summary>
        /// Preenche os campos com os dados do quarto selecionado para edição.
        /// </summary>
        private void btnEditRoom_Click(object sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a room to edit.");
                return;
            }

            var row = dgvRooms.SelectedRows[0];

            // Captura o RoomId da linha selecionada
            editingRoomId = Convert.ToInt32(row.Cells["RoomId"].Value);

            txtAddRoomNumber.Text = row.Cells["Number"].Value.ToString();
            txtAddRoomCapacity.Text = row.Cells["Capacity"].Value.ToString();
            txtAddRoomPrice.Text = row.Cells["PricePerNight"].Value.ToString();
            cmbAddRoomType.SelectedItem = Enum.Parse(typeof(RoomType), row.Cells["Type"].Value.ToString());
            cmbAddRoomStatus.SelectedItem = Enum.Parse(typeof(RoomStatus), row.Cells["Status"].Value.ToString());

            txtAddRoomNumber.Enabled = false;
            pnlAddRoom.Visible = true;
            isEditing = true;
            lblAddOrEdit.Text = "Edit Room";
        }

        /// <summary>
        /// Remove o quarto selecionado, se possível.
        /// </summary>
        private void btnDeleteRoom_Click(object sender, EventArgs e)
        {
            if (dgvRooms.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a room to delete.");
                return;
            }

            var row = dgvRooms.SelectedRows[0];

            int roomId = Convert.ToInt32(row.Cells["RoomId"].Value);
            int roomNumber = Convert.ToInt32(row.Cells["Number"].Value);

            if (!crud.CanDelete(roomId))
            {
                MessageBox.Show("This room cannot be deleted because it has future reservations.",
                    "Deletion Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                $"Are you sure you want to delete room {roomNumber}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm == DialogResult.Yes)
            {
                crud.Delete(roomId); // ✅ Agora usa o identificador correto
                LoadRooms();
                ClearFields();
                pnlAddRoom.Visible = false;
                txtAddRoomNumber.Enabled = true;
                isEditing = false;
            }
        }

        /// <summary>
        /// Cancela a adição ou edição de um quarto.
        /// </summary>
        private void btnCancelAddRoom_Click(object sender, EventArgs e)
        {
            pnlAddRoom.Visible = false;
            ClearFields();
            txtAddRoomNumber.Enabled = true;
            isEditing = false;
        }

        /// <summary>
        /// Valida e guarda um novo quarto ou atualiza um existente.
        /// </summary>
        private void btnSaveAddRoom_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAddRoomNumber.Text, out int number) ||
                !int.TryParse(txtAddRoomCapacity.Text, out int capacity) ||
                !decimal.TryParse(txtAddRoomPrice.Text, out decimal price))
            {
                MessageBox.Show("Please enter valid numeric values.");
                return;
            }

            Room room = new Room
            {
                Number = number,
                Capacity = capacity,
                PricePerNight = price,
                Type = (RoomType)cmbAddRoomType.SelectedItem,
                Status = (RoomStatus)cmbAddRoomStatus.SelectedItem
            };

            if (!ValidationHelper.ValidateRoom(room, out string erro))
            {
                MessageBox.Show("Validation error: " + erro);
                return;
            }

            if (isEditing)
            {
                room.RoomId = editingRoomId;
                crud.Update(room);
                MessageBox.Show("Room updated successfully.");
            }
            else
            {
                var existingRooms = crud.GetAll();
                if (existingRooms.Any(r => r.Number == number))
                {
                    MessageBox.Show($"Room number {number} already exists. Please choose a different number.", "Duplicate Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                crud.Add(room);
                MessageBox.Show("Room added successfully.");
            }

            pnlAddRoom.Visible = false;
            LoadRooms();
            ClearFields();
            txtAddRoomNumber.Enabled = true;
            isEditing = false;
        }

        /// <summary>
        /// Limpa todos os campos do formulário de quartos.
        /// </summary>
        private void ClearFields()
        {
            txtAddRoomNumber.Clear();
            txtAddRoomCapacity.Clear();
            txtAddRoomPrice.Clear();
            cmbAddRoomType.SelectedIndex = 0;
            cmbAddRoomStatus.SelectedIndex = 0;
        }
    }
}


