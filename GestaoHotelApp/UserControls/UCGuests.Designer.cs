namespace HotelManagementApp.UserControls
{
    partial class UCGuests
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
            pnlUCGuests = new Panel();
            btnViewHistory = new Button();
            btnAddGuest = new Button();
            btnEditGuest = new Button();
            btnDeleteGuest = new Button();
            label1 = new Label();
            pnlAddGuest = new Panel();
            label3 = new Label();
            label2 = new Label();
            lblAddOrEdit = new Label();
            btnSaveGuest = new Button();
            btnCancelGuest = new Button();
            txtGuestEmail = new TextBox();
            txtGuestContact = new TextBox();
            label7 = new Label();
            label9 = new Label();
            txtGuestName = new TextBox();
            txtGuestDocument = new TextBox();
            dgvGuests = new DataGridView();
            cmbGuestFilter = new ComboBox();
            label4 = new Label();
            pnlUCGuests.SuspendLayout();
            pnlAddGuest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGuests).BeginInit();
            SuspendLayout();
            // 
            // pnlUUCCredits
            // 
            pnlUCGuests.BackColor = Color.FromArgb(156, 149, 131);
            pnlUCGuests.Controls.Add(btnViewHistory);
            pnlUCGuests.Controls.Add(btnAddGuest);
            pnlUCGuests.Controls.Add(btnEditGuest);
            pnlUCGuests.Controls.Add(btnDeleteGuest);
            pnlUCGuests.Controls.Add(label1);
            pnlUCGuests.Dock = DockStyle.Top;
            pnlUCGuests.Location = new Point(0, 0);
            pnlUCGuests.Margin = new Padding(4);
            pnlUCGuests.Name = "pnlUUCCredits";
            pnlUCGuests.Size = new Size(1286, 155);
            pnlUCGuests.TabIndex = 0;
            // 
            // btnViewHistory
            // 
            btnViewHistory.Anchor = AnchorStyles.None;
            btnViewHistory.BackColor = Color.FromArgb(226, 212, 183);
            btnViewHistory.FlatAppearance.BorderSize = 2;
            btnViewHistory.FlatStyle = FlatStyle.Flat;
            btnViewHistory.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnViewHistory.ForeColor = Color.White;
            btnViewHistory.Location = new Point(1170, 51);
            btnViewHistory.Name = "btnViewHistory";
            btnViewHistory.Size = new Size(102, 48);
            btnViewHistory.TabIndex = 15;
            btnViewHistory.Text = "History";
            btnViewHistory.UseVisualStyleBackColor = false;
            // 
            // btnAddGuest
            // 
            btnAddGuest.Anchor = AnchorStyles.None;
            btnAddGuest.BackColor = Color.FromArgb(226, 212, 183);
            btnAddGuest.FlatAppearance.BorderSize = 2;
            btnAddGuest.FlatStyle = FlatStyle.Flat;
            btnAddGuest.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddGuest.ForeColor = Color.White;
            btnAddGuest.Location = new Point(816, 51);
            btnAddGuest.Name = "btnAddGuest";
            btnAddGuest.Size = new Size(93, 48);
            btnAddGuest.TabIndex = 10;
            btnAddGuest.Text = "Add";
            btnAddGuest.UseVisualStyleBackColor = false;
            // 
            // btnEditGuest
            // 
            btnEditGuest.Anchor = AnchorStyles.None;
            btnEditGuest.BackColor = Color.FromArgb(226, 212, 183);
            btnEditGuest.FlatAppearance.BorderSize = 2;
            btnEditGuest.FlatStyle = FlatStyle.Flat;
            btnEditGuest.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEditGuest.ForeColor = Color.White;
            btnEditGuest.Location = new Point(934, 51);
            btnEditGuest.Name = "btnEditGuest";
            btnEditGuest.Size = new Size(93, 48);
            btnEditGuest.TabIndex = 11;
            btnEditGuest.Text = "Edit";
            btnEditGuest.UseVisualStyleBackColor = false;
            // 
            // btnDeleteGuest
            // 
            btnDeleteGuest.Anchor = AnchorStyles.None;
            btnDeleteGuest.BackColor = Color.FromArgb(226, 212, 183);
            btnDeleteGuest.FlatAppearance.BorderSize = 2;
            btnDeleteGuest.FlatStyle = FlatStyle.Flat;
            btnDeleteGuest.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteGuest.ForeColor = Color.White;
            btnDeleteGuest.Location = new Point(1052, 51);
            btnDeleteGuest.Name = "btnDeleteGuest";
            btnDeleteGuest.Size = new Size(93, 48);
            btnDeleteGuest.TabIndex = 14;
            btnDeleteGuest.Text = "Delete";
            btnDeleteGuest.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(78, 60);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(271, 46);
            label1.TabIndex = 0;
            label1.Text = "Guests Overview";
            // 
            // pnlAddGuest
            // 
            pnlAddGuest.Controls.Add(label3);
            pnlAddGuest.Controls.Add(label2);
            pnlAddGuest.Controls.Add(lblAddOrEdit);
            pnlAddGuest.Controls.Add(btnSaveGuest);
            pnlAddGuest.Controls.Add(btnCancelGuest);
            pnlAddGuest.Controls.Add(txtGuestEmail);
            pnlAddGuest.Controls.Add(txtGuestContact);
            pnlAddGuest.Controls.Add(label7);
            pnlAddGuest.Controls.Add(label9);
            pnlAddGuest.Controls.Add(txtGuestName);
            pnlAddGuest.Controls.Add(txtGuestDocument);
            pnlAddGuest.Location = new Point(0, 153);
            pnlAddGuest.Name = "pnlAddGuest";
            pnlAddGuest.Size = new Size(1286, 648);
            pnlAddGuest.TabIndex = 21;
            pnlAddGuest.Visible = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(382, 350);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(120, 39);
            label3.TabIndex = 44;
            label3.Text = "Contact:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(406, 211);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(95, 39);
            label2.TabIndex = 43;
            label2.Text = "Name:";
            // 
            // lblAddOrEdit
            // 
            lblAddOrEdit.AutoSize = true;
            lblAddOrEdit.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAddOrEdit.Location = new Point(657, 121);
            lblAddOrEdit.Margin = new Padding(4, 0, 4, 0);
            lblAddOrEdit.Name = "lblAddOrEdit";
            lblAddOrEdit.Size = new Size(252, 46);
            lblAddOrEdit.TabIndex = 42;
            lblAddOrEdit.Text = "Add New Guest";
            // 
            // btnSaveGuest
            // 
            btnSaveGuest.Anchor = AnchorStyles.None;
            btnSaveGuest.BackColor = Color.FromArgb(226, 212, 183);
            btnSaveGuest.FlatAppearance.BorderSize = 2;
            btnSaveGuest.FlatStyle = FlatStyle.Flat;
            btnSaveGuest.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveGuest.ForeColor = Color.White;
            btnSaveGuest.Location = new Point(910, 478);
            btnSaveGuest.Name = "btnSaveGuest";
            btnSaveGuest.Size = new Size(93, 48);
            btnSaveGuest.TabIndex = 41;
            btnSaveGuest.Text = "Save";
            btnSaveGuest.UseVisualStyleBackColor = false;
            // 
            // btnCancelGuest
            // 
            btnCancelGuest.Anchor = AnchorStyles.None;
            btnCancelGuest.BackColor = Color.FromArgb(226, 212, 183);
            btnCancelGuest.FlatAppearance.BorderSize = 2;
            btnCancelGuest.FlatStyle = FlatStyle.Flat;
            btnCancelGuest.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelGuest.ForeColor = Color.White;
            btnCancelGuest.Location = new Point(792, 478);
            btnCancelGuest.Name = "btnCancelGuest";
            btnCancelGuest.Size = new Size(93, 48);
            btnCancelGuest.TabIndex = 40;
            btnCancelGuest.Text = "Cancel";
            btnCancelGuest.UseVisualStyleBackColor = false;
            // 
            // txtGuestEmail
            // 
            txtGuestEmail.Location = new Point(509, 429);
            txtGuestEmail.Name = "txtGuestEmail";
            txtGuestEmail.Size = new Size(185, 29);
            txtGuestEmail.TabIndex = 37;
            // 
            // txtGuestContact
            // 
            txtGuestContact.Location = new Point(509, 360);
            txtGuestContact.Name = "txtGuestContact";
            txtGuestContact.Size = new Size(185, 29);
            txtGuestContact.TabIndex = 36;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.Location = new Point(406, 419);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(96, 39);
            label7.TabIndex = 35;
            label7.Text = "Email:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(177, 280);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(325, 39);
            label9.TabIndex = 33;
            label9.Text = "Identification Document:";
            // 
            // txtGuestName
            // 
            txtGuestName.Location = new Point(509, 219);
            txtGuestName.Name = "txtGuestName";
            txtGuestName.Size = new Size(185, 29);
            txtGuestName.TabIndex = 32;
            // 
            // txtGuestDocument
            // 
            txtGuestDocument.Location = new Point(509, 290);
            txtGuestDocument.Name = "txtGuestDocument";
            txtGuestDocument.Size = new Size(185, 29);
            txtGuestDocument.TabIndex = 34;
            // 
            // dgvGuests
            // 
            dgvGuests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGuests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvGuests.Location = new Point(78, 364);
            dgvGuests.Name = "dgvGuests";
            dgvGuests.ReadOnly = true;
            dgvGuests.Size = new Size(1067, 335);
            dgvGuests.TabIndex = 18;
            // 
            // cmbGuestFilter
            // 
            cmbGuestFilter.FormattingEnabled = true;
            cmbGuestFilter.Location = new Point(291, 254);
            cmbGuestFilter.Name = "cmbGuestFilter";
            cmbGuestFilter.Size = new Size(216, 30);
            cmbGuestFilter.TabIndex = 23;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(78, 245);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(206, 39);
            label4.TabIndex = 22;
            label4.Text = "Filter by Guest:";
            // 
            // UCGuests
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(202, 219, 200);
            Controls.Add(pnlUCGuests);
            Controls.Add(pnlAddGuest);
            Controls.Add(cmbGuestFilter);
            Controls.Add(label4);
            Controls.Add(dgvGuests);
            Font = new Font("Sylfaen", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "UCGuests";
            Size = new Size(1286, 801);
            pnlUCGuests.ResumeLayout(false);
            pnlUCGuests.PerformLayout();
            pnlAddGuest.ResumeLayout(false);
            pnlAddGuest.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvGuests).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlUCGuests;
        private Label label1;
        private Button btnEditGuest;
        private Button btnAddGuest;
        private Button btnDeleteGuest;
        private DataGridView dgvGuests;
        private Panel pnlAddGuest;
        private Label lblAddOrEdit;
        private Button btnSaveGuest;
        private Button btnCancelGuest;
        private TextBox txtGuestEmail;
        private TextBox txtGuestContact;
        private Label label7;
        private Label label9;
        private TextBox txtGuestName;
        private TextBox txtGuestDocument;
        private Label label2;
        private Label label3;
        private ComboBox cmbGuestFilter;
        private Label label4;
        private Button btnViewHistory;
    }
}
