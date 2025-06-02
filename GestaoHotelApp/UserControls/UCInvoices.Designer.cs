namespace HotelManagementApp.UserControls
{
    partial class UCInvoices
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
                
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlUCInvoices = new Panel();
            btnDeleteInvoice = new Button();
            btnEditInvoice = new Button();
            btnAddInvoice = new Button();
            label1 = new Label();
            dgvInvoices = new DataGridView();
            pnlAddInvoice = new Panel();
            label6 = new Label();
            btnSaveInvoice = new Button();
            btnCancel = new Button();
            label5 = new Label();
            cmbBooking = new ComboBox();
            label10 = new Label();
            txtGsName = new TextBox();
            label4 = new Label();
            txtStayTotal = new TextBox();
            label8 = new Label();
            txtExtrasTotal = new TextBox();
            label11 = new Label();
            txtTotal = new TextBox();
            label12 = new Label();
            cmbPaymentMethod = new ComboBox();
            pnlUCInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInvoices).BeginInit();
            pnlAddInvoice.SuspendLayout();
            SuspendLayout();
            // 
            // pnlUCInvoices
            // 
            pnlUCInvoices.BackColor = Color.FromArgb(156, 149, 131);
            pnlUCInvoices.Controls.Add(btnDeleteInvoice);
            pnlUCInvoices.Controls.Add(btnEditInvoice);
            pnlUCInvoices.Controls.Add(btnAddInvoice);
            pnlUCInvoices.Controls.Add(label1);
            pnlUCInvoices.Dock = DockStyle.Top;
            pnlUCInvoices.Location = new Point(0, 0);
            pnlUCInvoices.Margin = new Padding(4);
            pnlUCInvoices.Name = "pnlUCInvoices";
            pnlUCInvoices.Size = new Size(1286, 155);
            pnlUCInvoices.TabIndex = 1;
            // 
            // btnDeleteInvoice
            // 
            btnDeleteInvoice.Anchor = AnchorStyles.None;
            btnDeleteInvoice.BackColor = Color.FromArgb(226, 212, 183);
            btnDeleteInvoice.FlatAppearance.BorderSize = 2;
            btnDeleteInvoice.FlatStyle = FlatStyle.Flat;
            btnDeleteInvoice.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteInvoice.ForeColor = Color.White;
            btnDeleteInvoice.Location = new Point(1052, 51);
            btnDeleteInvoice.Name = "btnDeleteInvoice";
            btnDeleteInvoice.Size = new Size(93, 48);
            btnDeleteInvoice.TabIndex = 24;
            btnDeleteInvoice.Text = "Delete";
            btnDeleteInvoice.UseVisualStyleBackColor = false;
            // 
            // btnEditInvoice
            // 
            btnEditInvoice.Anchor = AnchorStyles.None;
            btnEditInvoice.BackColor = Color.FromArgb(226, 212, 183);
            btnEditInvoice.FlatAppearance.BorderSize = 2;
            btnEditInvoice.FlatStyle = FlatStyle.Flat;
            btnEditInvoice.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEditInvoice.ForeColor = Color.White;
            btnEditInvoice.Location = new Point(934, 51);
            btnEditInvoice.Name = "btnEditInvoice";
            btnEditInvoice.Size = new Size(93, 48);
            btnEditInvoice.TabIndex = 23;
            btnEditInvoice.Text = "Edit";
            btnEditInvoice.UseVisualStyleBackColor = false;
            // 
            // btnAddInvoice
            // 
            btnAddInvoice.Anchor = AnchorStyles.None;
            btnAddInvoice.BackColor = Color.FromArgb(226, 212, 183);
            btnAddInvoice.FlatAppearance.BorderSize = 2;
            btnAddInvoice.FlatStyle = FlatStyle.Flat;
            btnAddInvoice.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddInvoice.ForeColor = Color.White;
            btnAddInvoice.Location = new Point(816, 51);
            btnAddInvoice.Name = "btnAddInvoice";
            btnAddInvoice.Size = new Size(93, 48);
            btnAddInvoice.TabIndex = 22;
            btnAddInvoice.Text = "Add";
            btnAddInvoice.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(78, 60);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(296, 46);
            label1.TabIndex = 0;
            label1.Text = "Invoices Overview";
            // 
            // dgvInvoices
            // 
            dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInvoices.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvInvoices.Location = new Point(78, 364);
            dgvInvoices.Name = "dgvInvoices";
            dgvInvoices.ReadOnly = true;
            dgvInvoices.Size = new Size(1067, 335);
            dgvInvoices.TabIndex = 25;
            // 
            // pnlAddInvoice
            // 
            pnlAddInvoice.Controls.Add(label6);
            pnlAddInvoice.Controls.Add(btnSaveInvoice);
            pnlAddInvoice.Controls.Add(btnCancel);
            pnlAddInvoice.Controls.Add(label5);
            pnlAddInvoice.Controls.Add(cmbBooking);
            pnlAddInvoice.Controls.Add(label10);
            pnlAddInvoice.Controls.Add(txtGsName);
            pnlAddInvoice.Controls.Add(label4);
            pnlAddInvoice.Controls.Add(txtStayTotal);
            pnlAddInvoice.Controls.Add(label8);
            pnlAddInvoice.Controls.Add(txtExtrasTotal);
            pnlAddInvoice.Controls.Add(label11);
            pnlAddInvoice.Controls.Add(txtTotal);
            pnlAddInvoice.Controls.Add(label12);
            pnlAddInvoice.Controls.Add(cmbPaymentMethod);
            pnlAddInvoice.Location = new Point(0, 156);
            pnlAddInvoice.Name = "pnlAddInvoice";
            pnlAddInvoice.Size = new Size(1286, 645);
            pnlAddInvoice.TabIndex = 26;
            pnlAddInvoice.Visible = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(657, 121);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(277, 46);
            label6.TabIndex = 42;
            label6.Text = "Add New Invoice";
            // 
            // btnSaveInvoice
            // 
            btnSaveInvoice.Anchor = AnchorStyles.None;
            btnSaveInvoice.BackColor = Color.FromArgb(226, 212, 183);
            btnSaveInvoice.FlatAppearance.BorderSize = 2;
            btnSaveInvoice.FlatStyle = FlatStyle.Flat;
            btnSaveInvoice.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveInvoice.ForeColor = Color.White;
            btnSaveInvoice.Location = new Point(902, 516);
            btnSaveInvoice.Name = "btnSaveInvoice";
            btnSaveInvoice.Size = new Size(93, 48);
            btnSaveInvoice.TabIndex = 53;
            btnSaveInvoice.Text = "Save";
            btnSaveInvoice.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.None;
            btnCancel.BackColor = Color.FromArgb(226, 212, 183);
            btnCancel.FlatAppearance.BorderSize = 2;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(792, 516);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(93, 48);
            btnCancel.TabIndex = 47;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(343, 209);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(158, 39);
            label5.TabIndex = 43;
            label5.Text = "Booking GuestId:";
            // 
            // cmbBooking
            // 
            cmbBooking.FormattingEnabled = true;
            cmbBooking.Location = new Point(509, 215);
            cmbBooking.Name = "cmbBooking";
            cmbBooking.Size = new Size(185, 30);
            cmbBooking.TabIndex = 45;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(330, 280);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(172, 39);
            label10.TabIndex = 33;
            label10.Text = "Guest Name:";
            // 
            // txtGsName
            // 
            txtGsName.Location = new Point(509, 291);
            txtGsName.Name = "txtGsName";
            txtGsName.ReadOnly = true;
            txtGsName.Size = new Size(185, 29);
            txtGsName.TabIndex = 50;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(355, 350);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(147, 39);
            label4.TabIndex = 44;
            label4.Text = "Stay Total:";
            // 
            // txtStayTotal
            // 
            txtStayTotal.Location = new Point(509, 360);
            txtStayTotal.Name = "txtStayTotal";
            txtStayTotal.ReadOnly = true;
            txtStayTotal.Size = new Size(185, 29);
            txtStayTotal.TabIndex = 36;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(332, 419);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(170, 39);
            label8.TabIndex = 35;
            label8.Text = "Extras Total:";
            // 
            // txtExtrasTotal
            // 
            txtExtrasTotal.Location = new Point(509, 429);
            txtExtrasTotal.Name = "txtExtrasTotal";
            txtExtrasTotal.ReadOnly = true;
            txtExtrasTotal.Size = new Size(185, 29);
            txtExtrasTotal.TabIndex = 37;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label11.Location = new Point(330, 478);
            label11.Margin = new Padding(4, 0, 4, 0);
            label11.Name = "label11";
            label11.Size = new Size(171, 39);
            label11.TabIndex = 49;
            label11.Text = "Grand Total:";
            // 
            // txtTotal
            // 
            txtTotal.Location = new Point(509, 488);
            txtTotal.Name = "txtTotal";
            txtTotal.ReadOnly = true;
            txtTotal.Size = new Size(185, 29);
            txtTotal.TabIndex = 51;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label12.Location = new Point(268, 535);
            label12.Margin = new Padding(4, 0, 4, 0);
            label12.Name = "label12";
            label12.Size = new Size(234, 39);
            label12.TabIndex = 48;
            label12.Text = "Payment Method:";
            // 
            // cmbPaymentMethod
            // 
            cmbPaymentMethod.FormattingEnabled = true;
            cmbPaymentMethod.Location = new Point(509, 544);
            cmbPaymentMethod.Name = "cmbPaymentMethod";
            cmbPaymentMethod.Size = new Size(185, 30);
            cmbPaymentMethod.TabIndex = 52;
            // 
            // UCInvoices
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(202, 219, 200);
            Controls.Add(pnlUCInvoices);
            Controls.Add(pnlAddInvoice);
            Controls.Add(dgvInvoices);
            Font = new Font("Sylfaen", 12F);
            Margin = new Padding(4);
            Name = "UCInvoices";
            Size = new Size(1286, 801);
            pnlUCInvoices.ResumeLayout(false);
            pnlUCInvoices.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvInvoices).EndInit();
            pnlAddInvoice.ResumeLayout(false);
            pnlAddInvoice.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlUCInvoices;
        private Label label1;
        private Button btnDeleteInvoice;
        private Button btnEditInvoice;
        private Button btnAddInvoice;
        private DataGridView dgvInvoices;
        private Panel pnlAddInvoice;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox txtExtrasTotal;
        private TextBox txtStayTotal;
        private Label label8;
        private Label label10;
        private ComboBox cmbBooking;
        private Button btnCancel;
        private Label label11;
        private Label label12;
        private TextBox txtGsName;
        private Button btnSaveInvoice;
        private ComboBox cmbPaymentMethod;
        private TextBox txtTotal;
    }
}
