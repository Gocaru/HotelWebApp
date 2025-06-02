namespace HotelManagementApp.UserControls
{
    partial class UCExtras
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
            label1 = new Label();
            pnlUCExtras = new Panel();
            btnDeleteService = new Button();
            btnEditService = new Button();
            btnAddService = new Button();
            pnlAddExtraService = new Panel();
            cmbBooking = new ComboBox();
            btnSaveService = new Button();
            btnCancel = new Button();
            lblAddOrEdit = new Label();
            txtPrice = new TextBox();
            label8 = new Label();
            label9 = new Label();
            cmbServiceType = new ComboBox();
            label10 = new Label();
            dgvExtras = new DataGridView();
            pnlUCExtras.SuspendLayout();
            pnlAddExtraService.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvExtras).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(78, 60);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(379, 46);
            label1.TabIndex = 0;
            label1.Text = "Extra Services Overview";
            // 
            // pnlUCExtras
            // 
            pnlUCExtras.BackColor = Color.FromArgb(156, 149, 131);
            pnlUCExtras.Controls.Add(btnDeleteService);
            pnlUCExtras.Controls.Add(btnEditService);
            pnlUCExtras.Controls.Add(btnAddService);
            pnlUCExtras.Controls.Add(label1);
            pnlUCExtras.Dock = DockStyle.Top;
            pnlUCExtras.Location = new Point(0, 0);
            pnlUCExtras.Margin = new Padding(4);
            pnlUCExtras.Name = "pnlUCExtras";
            pnlUCExtras.Size = new Size(1286, 155);
            pnlUCExtras.TabIndex = 1;
            // 
            // btnDeleteService
            // 
            btnDeleteService.Anchor = AnchorStyles.None;
            btnDeleteService.BackColor = Color.FromArgb(226, 212, 183);
            btnDeleteService.FlatAppearance.BorderSize = 2;
            btnDeleteService.FlatStyle = FlatStyle.Flat;
            btnDeleteService.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteService.ForeColor = Color.White;
            btnDeleteService.Location = new Point(1052, 51);
            btnDeleteService.Name = "btnDeleteService";
            btnDeleteService.Size = new Size(93, 48);
            btnDeleteService.TabIndex = 24;
            btnDeleteService.Text = "Delete";
            btnDeleteService.UseVisualStyleBackColor = false;
            // 
            // btnEditService
            // 
            btnEditService.Anchor = AnchorStyles.None;
            btnEditService.BackColor = Color.FromArgb(226, 212, 183);
            btnEditService.FlatAppearance.BorderSize = 2;
            btnEditService.FlatStyle = FlatStyle.Flat;
            btnEditService.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEditService.ForeColor = Color.White;
            btnEditService.Location = new Point(934, 51);
            btnEditService.Name = "btnEditService";
            btnEditService.Size = new Size(93, 48);
            btnEditService.TabIndex = 23;
            btnEditService.Text = "Edit";
            btnEditService.UseVisualStyleBackColor = false;
            // 
            // btnAddService
            // 
            btnAddService.Anchor = AnchorStyles.None;
            btnAddService.BackColor = Color.FromArgb(226, 212, 183);
            btnAddService.FlatAppearance.BorderSize = 2;
            btnAddService.FlatStyle = FlatStyle.Flat;
            btnAddService.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddService.ForeColor = Color.White;
            btnAddService.Location = new Point(816, 51);
            btnAddService.Name = "btnAddService";
            btnAddService.Size = new Size(93, 48);
            btnAddService.TabIndex = 22;
            btnAddService.Text = "Add";
            btnAddService.UseVisualStyleBackColor = false;
            // 
            // pnlAddExtraService
            // 
            pnlAddExtraService.Controls.Add(cmbBooking);
            pnlAddExtraService.Controls.Add(btnSaveService);
            pnlAddExtraService.Controls.Add(btnCancel);
            pnlAddExtraService.Controls.Add(lblAddOrEdit);
            pnlAddExtraService.Controls.Add(txtPrice);
            pnlAddExtraService.Controls.Add(label8);
            pnlAddExtraService.Controls.Add(label9);
            pnlAddExtraService.Controls.Add(cmbServiceType);
            pnlAddExtraService.Controls.Add(label10);
            pnlAddExtraService.Location = new Point(0, 153);
            pnlAddExtraService.Name = "pnlAddExtraService";
            pnlAddExtraService.Size = new Size(1286, 648);
            pnlAddExtraService.TabIndex = 25;
            pnlAddExtraService.Visible = false;
            // 
            // cmbBooking
            // 
            cmbBooking.FormattingEnabled = true;
            cmbBooking.Location = new Point(509, 218);
            cmbBooking.Name = "cmbBooking";
            cmbBooking.Size = new Size(236, 30);
            cmbBooking.TabIndex = 45;
            // 
            // btnSaveService
            // 
            btnSaveService.Anchor = AnchorStyles.None;
            btnSaveService.BackColor = Color.FromArgb(226, 212, 183);
            btnSaveService.FlatAppearance.BorderSize = 2;
            btnSaveService.FlatStyle = FlatStyle.Flat;
            btnSaveService.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveService.ForeColor = Color.White;
            btnSaveService.Location = new Point(955, 403);
            btnSaveService.Name = "btnSaveService";
            btnSaveService.Size = new Size(93, 48);
            btnSaveService.TabIndex = 44;
            btnSaveService.Text = "Save";
            btnSaveService.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.None;
            btnCancel.BackColor = Color.FromArgb(226, 212, 183);
            btnCancel.FlatAppearance.BorderSize = 2;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(837, 403);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(93, 48);
            btnCancel.TabIndex = 43;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // lblAddOrEdit
            // 
            lblAddOrEdit.AutoSize = true;
            lblAddOrEdit.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAddOrEdit.Location = new Point(657, 121);
            lblAddOrEdit.Margin = new Padding(4, 0, 4, 0);
            lblAddOrEdit.Name = "lblAddOrEdit";
            lblAddOrEdit.Size = new Size(273, 46);
            lblAddOrEdit.TabIndex = 42;
            lblAddOrEdit.Text = "Add New Service";
            // 
            // txtPrice
            // 
            txtPrice.Location = new Point(509, 360);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(236, 29);
            txtPrice.TabIndex = 36;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(415, 350);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(87, 39);
            label8.TabIndex = 34;
            label8.Text = "Price:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(323, 281);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(179, 39);
            label9.TabIndex = 33;
            label9.Text = "Service Type:";
            // 
            // cmbServiceType
            // 
            cmbServiceType.FormattingEnabled = true;
            cmbServiceType.Location = new Point(509, 290);
            cmbServiceType.Name = "cmbServiceType";
            cmbServiceType.Size = new Size(236, 30);
            cmbServiceType.TabIndex = 31;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(377, 209);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(125, 39);
            label10.TabIndex = 30;
            label10.Text = "Booking:";
            // 
            // dgvExtras
            // 
            dgvExtras.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvExtras.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExtras.Location = new Point(78, 364);
            dgvExtras.Name = "dgvExtras";
            dgvExtras.ReadOnly = true;
            dgvExtras.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExtras.Size = new Size(1067, 335);
            dgvExtras.TabIndex = 45;
            // 
            // UCExtras
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(202, 219, 200);
            Controls.Add(pnlUCExtras);
            Controls.Add(pnlAddExtraService);
            Controls.Add(dgvExtras);
            Font = new Font("Sylfaen", 12F);
            Margin = new Padding(4);
            Name = "UCExtras";
            Size = new Size(1286, 801);
            pnlUCExtras.ResumeLayout(false);
            pnlUCExtras.PerformLayout();
            pnlAddExtraService.ResumeLayout(false);
            pnlAddExtraService.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvExtras).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Panel pnlUCExtras;
        private Button btnDeleteService;
        private Button btnEditService;
        private Button btnAddService;
        private Panel pnlAddExtraService;
        private DataGridView dgvExtras;
        private Button btnSaveService;
        private Button btnCancel;
        private Label lblAddOrEdit;
        private Label label8;
        private Label label9;
        private TextBox txtPrice;
        private ComboBox cmbServiceType;
        private Label label10;
        private ComboBox cmbBooking;
    }
}
