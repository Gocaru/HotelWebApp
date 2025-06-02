using HotelManagement.Core;
using HotelManagementSystem.Logic;

namespace HotelManagementApp.UserControls
{
    /// <summary>
    /// UserControl responsável pela gestão de faturas: criação, visualização e ações básicas.
    /// </summary>
    public partial class UCInvoices : UserControl
    {
        private CrudInvoices crud = new CrudInvoices();
        private CrudBookings crudBookings = new CrudBookings();
        private CrudGuests crudGuests = new CrudGuests();
        private CrudExtras crudExtras = new CrudExtras();
        private CrudRooms crudRooms = new CrudRooms();

        private bool isEditing = false;
        private int editingInvoiceId;

        public UCInvoices()
        {
            InitializeComponent();
            LoadInvoices();
            LoadBookings();
            LoadPaymentMethods();
            AttachEvents();
        }

        /// <summary>
        /// Carrega todas as faturas na grelha.
        /// </summary>
        private void LoadInvoices()
        {
            dgvInvoices.Rows.Clear();
            dgvInvoices.Columns.Clear();

            dgvInvoices.Columns.Add("GuestId", "Invoice ID");
            dgvInvoices.Columns.Add("BookingId", "Booking ID");
            dgvInvoices.Columns.Add("GuestName", "Guest");
            dgvInvoices.Columns.Add("StayTotal", "Stay (€)");
            dgvInvoices.Columns.Add("ExtrasTotal", "Extras (€)");
            dgvInvoices.Columns.Add("Total", "Total (€)");
            dgvInvoices.Columns.Add("IssueDate", "Date");
            dgvInvoices.Columns.Add("PaymentMethod", "Payment");

            List<Invoice> invoices = crud.GetAll();

            foreach (Invoice invoice in invoices)
            {
                dgvInvoices.Rows.Add(
                    invoice.InvoiceId,
                    invoice.BookingId,
                    invoice.GuestName,
                    invoice.StayTotal.ToString("F2"),
                    invoice.ExtrasTotal.ToString("F2"),
                    invoice.Total.ToString("F2"),
                    invoice.IssueDate.ToShortDateString(),
                    invoice.PaymentMethod.ToString()
                );
            }
        }

        /// <summary>
        /// Carrega todas as reservas disponíveis no ComboBox.
        /// </summary>
        private void LoadBookings()
        {
            var bookings = crudBookings.GetAll();
            var guests = new CrudGuests().GetAll();
            var rooms = new CrudRooms().GetAll();

            var items = bookings.Select(b =>
            {
                var guest = guests.FirstOrDefault(g => g.GuestId == b.GuestId);
                var room = rooms.FirstOrDefault(r => r.RoomId == b.RoomId);

                string guestName = guest?.Name ?? "Unknown";
                string roomNumber = room?.Number.ToString() ?? "N/A";

                return new
                {
                    Id = b.BookingId, // Valor real (para a fatura)
                    Display = $"Booking {b.BookingId} - {guestName} - Room {roomNumber} ({b.CheckInDate:dd/MM} - {b.CheckOutDate:dd/MM})"
                };
            }).ToList();

            cmbBooking.DataSource = items;
            cmbBooking.DisplayMember = "Display";
            cmbBooking.ValueMember = "Id";
        }

        /// <summary>
        /// Carrega todos os métodos de pagamento no ComboBox.
        /// </summary>
        private void LoadPaymentMethods()
        {
            cmbPaymentMethod.Items.Clear();
            foreach (PaymentMethod method in Enum.GetValues(typeof(PaymentMethod)))
            {
                cmbPaymentMethod.Items.Add(method);
            }
            cmbPaymentMethod.SelectedIndex = 0;
        }

        /// <summary>
        /// Quando uma reserva é selecionada, os campos são preenchidos automaticamente.
        /// </summary>
        private void cmbBooking_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBooking.SelectedItem is Booking booking)
            {
                Guest guest = crudGuests.GetAll().FirstOrDefault(g => g.GuestId == booking.GuestId);
                if (guest != null)
                {
                    txtGsName.Text = guest.Name;
                }

                int noites = (booking.CheckOutDate - booking.CheckInDate).Days;

                Room room = crudRooms.GetAll().FirstOrDefault(r => r.RoomId == booking.RoomId); // ✅ Correto agora
                decimal precoBase = room != null ? room.PricePerNight : 0;
                decimal totalEstadia = noites * precoBase;

                decimal totalExtras = crudExtras.GetTotalByBooking(booking.BookingId);
                decimal totalGeral = totalEstadia + totalExtras;

                txtStayTotal.Text = totalEstadia.ToString("F2");
                txtExtrasTotal.Text = totalExtras.ToString("F2");
                txtTotal.Text = totalGeral.ToString("F2");
            }
        }

        /// <summary>
        /// Liga os eventos aos controlos.
        /// </summary>
        private void AttachEvents()
        {
            cmbBooking.SelectedIndexChanged += cmbBooking_SelectedIndexChanged;
            btnAddInvoice.Click += btnAddInvoice_Click;
            btnSaveInvoice.Click += btnSaveInvoice_Click;
            btnCancel.Click += btnCancel_Click;
            btnEditInvoice.Click += btnEditInvoice_Click;
            btnDeleteInvoice.Click += btnDeleteInvoice_Click;
        }

        private void btnAddInvoice_Click(object sender, EventArgs e)
        {
            label6.Text = "Add New Invoice";
            ClearFields();
            pnlAddInvoice.Visible = true;
            isEditing = false;
        }

        private void btnEditInvoice_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvInvoices.SelectedRows[0];

            int invoiceId = Convert.ToInt32(selectedRow.Cells["GuestId"].Value);
            Invoice invoice = crud.GetById(invoiceId);

            if (invoice == null)
            {
                MessageBox.Show("Invoice not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Preencher os campos com os dados da fatura
            cmbBooking.SelectedValue = invoice.BookingId;

            Booking booking = crudBookings.GetAll().FirstOrDefault(b => b.BookingId == invoice.BookingId);
            if (booking != null)
            {
                Guest guest = crudGuests.GetAll().FirstOrDefault(g => g.GuestId == booking.GuestId);
                if (guest != null)
                {
                    txtGsName.Text = guest.Name;
                }
            }

            txtStayTotal.Text = invoice.StayTotal.ToString("F2");
            txtExtrasTotal.Text = invoice.ExtrasTotal.ToString("F2");
            txtTotal.Text = invoice.Total.ToString("F2");
            cmbPaymentMethod.SelectedItem = invoice.PaymentMethod;

            // Atualizar estado
            isEditing = true;
            editingInvoiceId = invoice.InvoiceId;
            label6.Text = "Edit Invoice";
            pnlAddInvoice.Visible = true;
        }

        private void btnDeleteInvoice_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an invoice to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvInvoices.SelectedRows[0];
            int invoiceId = Convert.ToInt32(selectedRow.Cells["GuestId"].Value);

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this invoice?",
                                                   "Confirm Deletion",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                List<Invoice> allInvoices = crud.GetAll();
                Invoice invoiceToRemove = allInvoices.FirstOrDefault(i => i.InvoiceId == invoiceId);

                if (invoiceToRemove != null)
                {
                    allInvoices.Remove(invoiceToRemove);
                    crud.SaveAll(allInvoices);

                    MessageBox.Show("Invoice deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInvoices(); // método que atualiza a grelha
                }
                else
                {
                    MessageBox.Show("Invoice not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            pnlAddInvoice.Visible = false;
            isEditing = false;
            ClearFields();
        }

        /// <summary>
        /// Limpa os campos do formulário.
        /// </summary>
        private void ClearFields()
        {
            cmbBooking.SelectedIndex = -1;
            txtGsName.Clear();
            txtStayTotal.Clear();
            txtExtrasTotal.Clear();
            txtTotal.Clear();
            cmbPaymentMethod.SelectedIndex = -1;
        }

        /// <summary>
        /// Guarda uma nova fatura ou atualiza uma existente.
        /// </summary>
        private void btnSaveInvoice_Click(object sender, EventArgs e)
        {
            if (!(cmbBooking.SelectedItem is Booking booking))
            {
                MessageBox.Show("Please select a booking.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Invoice invoice = new Invoice
            {
                InvoiceId = isEditing ? editingInvoiceId : crud.GenerateNextId(),
                BookingId = booking.BookingId,
                GuestName = txtGsName.Text,
                StayTotal = decimal.TryParse(txtStayTotal.Text, out decimal stay) ? stay : 0,
                ExtrasTotal = decimal.TryParse(txtExtrasTotal.Text, out decimal extras) ? extras : 0,
                IssueDate = DateTime.Now,
                PaymentMethod = (PaymentMethod)cmbPaymentMethod.SelectedItem
            };

            if (!ValidationHelper.ValidateInvoice(invoice, out string erro))
            {
                MessageBox.Show("Validation error: " + erro, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isEditing)
            {
                crud.Update(invoice);
                MessageBox.Show("Invoice updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                crud.Add(invoice);
                MessageBox.Show("Invoice created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadInvoices();
            pnlAddInvoice.Visible = false;
            isEditing = false;
            ClearFields();
        }
    }
}
