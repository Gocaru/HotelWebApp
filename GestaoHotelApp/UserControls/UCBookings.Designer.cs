using HotelManagementSystem.Logic;

namespace HotelManagementApp.UserControls
{
    partial class UCBookings
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
            pnlUCBookings = new Panel();
            label1 = new Label();
            btnDeleteBooking = new Button();
            btnEditBooking = new Button();
            btnAddBooking = new Button();
            pnlAddBooking = new Panel();
            label6 = new Label();
            cmbRoom = new ComboBox();
            label5 = new Label();
            cmbGuest = new ComboBox();
            lblReservationDate = new Label();
            btnSaveBooking = new Button();
            btnCancelBooking = new Button();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            lblAddOrEdit = new Label();
            dtpCheckOut = new DateTimePicker();
            dtpCheckIn = new DateTimePicker();
            nudGuests = new NumericUpDown();
            cmbRoomType = new ComboBox();
            label7 = new Label();
            label9 = new Label();
            dgvBookings = new DataGridView();
            btnCheckIn = new Button();
            btnCheckOut = new Button();
            btnCancelNoShow = new Button();
            pnlUCBookings.SuspendLayout();
            pnlAddBooking.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudGuests).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvBookings).BeginInit();
            SuspendLayout();
            // 
            // pnlUCBookings
            // 
            pnlUCBookings.BackColor = Color.FromArgb(156, 149, 131);
            pnlUCBookings.Controls.Add(label1);
            pnlUCBookings.Controls.Add(btnDeleteBooking);
            pnlUCBookings.Controls.Add(btnEditBooking);
            pnlUCBookings.Controls.Add(btnAddBooking);
            pnlUCBookings.Dock = DockStyle.Top;
            pnlUCBookings.Location = new Point(0, 0);
            pnlUCBookings.Name = "pnlUCBookings";
            pnlUCBookings.Size = new Size(1286, 155);
            pnlUCBookings.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 26.25F);
            label1.Location = new Point(78, 60);
            label1.Name = "label1";
            label1.Size = new Size(309, 46);
            label1.TabIndex = 0;
            label1.Text = "Bookings Overview";
            // 
            // btnDeleteBooking
            // 
            btnDeleteBooking.Anchor = AnchorStyles.None;
            btnDeleteBooking.BackColor = Color.FromArgb(226, 212, 183);
            btnDeleteBooking.FlatAppearance.BorderSize = 2;
            btnDeleteBooking.FlatStyle = FlatStyle.Flat;
            btnDeleteBooking.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold);
            btnDeleteBooking.ForeColor = Color.White;
            btnDeleteBooking.Location = new Point(1052, 51);
            btnDeleteBooking.Name = "btnDeleteBooking";
            btnDeleteBooking.Size = new Size(93, 48);
            btnDeleteBooking.TabIndex = 14;
            btnDeleteBooking.Text = "Delete";
            btnDeleteBooking.UseVisualStyleBackColor = false;
            // 
            // btnEditBooking
            // 
            btnEditBooking.Anchor = AnchorStyles.None;
            btnEditBooking.BackColor = Color.FromArgb(226, 212, 183);
            btnEditBooking.FlatAppearance.BorderSize = 2;
            btnEditBooking.FlatStyle = FlatStyle.Flat;
            btnEditBooking.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold);
            btnEditBooking.ForeColor = Color.White;
            btnEditBooking.Location = new Point(934, 51);
            btnEditBooking.Name = "btnEditBooking";
            btnEditBooking.Size = new Size(93, 48);
            btnEditBooking.TabIndex = 11;
            btnEditBooking.Text = "Edit";
            btnEditBooking.UseVisualStyleBackColor = false;
            // 
            // btnAddBooking
            // 
            btnAddBooking.Anchor = AnchorStyles.None;
            btnAddBooking.BackColor = Color.FromArgb(226, 212, 183);
            btnAddBooking.FlatAppearance.BorderSize = 2;
            btnAddBooking.FlatStyle = FlatStyle.Flat;
            btnAddBooking.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold);
            btnAddBooking.ForeColor = Color.White;
            btnAddBooking.Location = new Point(816, 51);
            btnAddBooking.Name = "btnAddBooking";
            btnAddBooking.Size = new Size(93, 48);
            btnAddBooking.TabIndex = 10;
            btnAddBooking.Text = "Add";
            btnAddBooking.UseVisualStyleBackColor = false;
            // 
            // pnlAddBooking
            // 
            pnlAddBooking.BackColor = Color.FromArgb(202, 219, 200);
            pnlAddBooking.Controls.Add(label6);
            pnlAddBooking.Controls.Add(cmbRoom);
            pnlAddBooking.Controls.Add(label5);
            pnlAddBooking.Controls.Add(cmbGuest);
            pnlAddBooking.Controls.Add(lblReservationDate);
            pnlAddBooking.Controls.Add(btnSaveBooking);
            pnlAddBooking.Controls.Add(btnCancelBooking);
            pnlAddBooking.Controls.Add(label4);
            pnlAddBooking.Controls.Add(label3);
            pnlAddBooking.Controls.Add(label2);
            pnlAddBooking.Controls.Add(lblAddOrEdit);
            pnlAddBooking.Controls.Add(dtpCheckOut);
            pnlAddBooking.Controls.Add(dtpCheckIn);
            pnlAddBooking.Controls.Add(nudGuests);
            pnlAddBooking.Controls.Add(cmbRoomType);
            pnlAddBooking.Controls.Add(label7);
            pnlAddBooking.Controls.Add(label9);
            pnlAddBooking.Location = new Point(0, 153);
            pnlAddBooking.Name = "pnlAddBooking";
            pnlAddBooking.Size = new Size(1286, 648);
            pnlAddBooking.TabIndex = 22;
            pnlAddBooking.Visible = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(270, 536);
            label6.Name = "label6";
            label6.Size = new Size(232, 39);
            label6.TabIndex = 50;
            label6.Text = "Reservation Date:";
            // 
            // cmbRoom
            // 
            cmbRoom.Location = new Point(509, 379);
            cmbRoom.Name = "cmbRoom";
            cmbRoom.Size = new Size(185, 30);
            cmbRoom.TabIndex = 48;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(310, 369);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(192, 39);
            label5.TabIndex = 49;
            label5.Text = "Choose Room:";
            // 
            // cmbGuest
            // 
            cmbGuest.FormattingEnabled = true;
            cmbGuest.Location = new Point(509, 209);
            cmbGuest.Name = "cmbGuest";
            cmbGuest.Size = new Size(185, 30);
            cmbGuest.TabIndex = 47;
            // 
            // lblReservationDate
            // 
            lblReservationDate.AutoSize = true;
            lblReservationDate.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblReservationDate.Location = new Point(509, 536);
            lblReservationDate.Name = "lblReservationDate";
            lblReservationDate.Size = new Size(91, 39);
            lblReservationDate.TabIndex = 46;
            lblReservationDate.Text = "label5";
            // 
            // btnSaveBooking
            // 
            btnSaveBooking.Anchor = AnchorStyles.None;
            btnSaveBooking.BackColor = Color.FromArgb(226, 212, 183);
            btnSaveBooking.FlatAppearance.BorderSize = 2;
            btnSaveBooking.FlatStyle = FlatStyle.Flat;
            btnSaveBooking.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveBooking.ForeColor = Color.White;
            btnSaveBooking.Location = new Point(922, 497);
            btnSaveBooking.Name = "btnSaveBooking";
            btnSaveBooking.Size = new Size(93, 48);
            btnSaveBooking.TabIndex = 43;
            btnSaveBooking.Text = "Save";
            btnSaveBooking.UseVisualStyleBackColor = false;
            // 
            // btnCancelBooking
            // 
            btnCancelBooking.Anchor = AnchorStyles.None;
            btnCancelBooking.BackColor = Color.FromArgb(226, 212, 183);
            btnCancelBooking.FlatAppearance.BorderSize = 2;
            btnCancelBooking.FlatStyle = FlatStyle.Flat;
            btnCancelBooking.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelBooking.ForeColor = Color.White;
            btnCancelBooking.Location = new Point(804, 497);
            btnCancelBooking.Name = "btnCancelBooking";
            btnCancelBooking.Size = new Size(93, 48);
            btnCancelBooking.TabIndex = 42;
            btnCancelBooking.Text = "Cancel";
            btnCancelBooking.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(338, 476);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(159, 39);
            label4.TabIndex = 45;
            label4.Text = "Check-Out:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(212, 321);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(290, 39);
            label3.TabIndex = 44;
            label3.Text = "Number of Occupants:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(313, 200);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(189, 39);
            label2.TabIndex = 43;
            label2.Text = "Choose Guest:";
            // 
            // lblAddOrEdit
            // 
            lblAddOrEdit.AutoSize = true;
            lblAddOrEdit.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAddOrEdit.Location = new Point(657, 121);
            lblAddOrEdit.Margin = new Padding(4, 0, 4, 0);
            lblAddOrEdit.Name = "lblAddOrEdit";
            lblAddOrEdit.Size = new Size(290, 46);
            lblAddOrEdit.TabIndex = 42;
            lblAddOrEdit.Text = "Add New Booking";
            // 
            // dtpCheckOut
            // 
            dtpCheckOut.Location = new Point(509, 484);
            dtpCheckOut.Name = "dtpCheckOut";
            dtpCheckOut.Size = new Size(200, 29);
            dtpCheckOut.TabIndex = 14;
            // 
            // dtpCheckIn
            // 
            dtpCheckIn.Location = new Point(509, 427);
            dtpCheckIn.Name = "dtpCheckIn";
            dtpCheckIn.Size = new Size(200, 29);
            dtpCheckIn.TabIndex = 12;
            // 
            // nudGuests
            // 
            nudGuests.Location = new Point(509, 332);
            nudGuests.Name = "nudGuests";
            nudGuests.Size = new Size(185, 29);
            nudGuests.TabIndex = 15;
            // 
            // cmbRoomType
            // 
            cmbRoomType.Location = new Point(509, 272);
            cmbRoomType.Name = "cmbRoomType";
            cmbRoomType.Size = new Size(185, 30);
            cmbRoomType.TabIndex = 16;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.Location = new Point(357, 419);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(138, 39);
            label7.TabIndex = 35;
            label7.Text = "Check-In:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(338, 262);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(164, 39);
            label9.TabIndex = 33;
            label9.Text = "Room Type:";
            // 
            // dgvBookings
            // 
            dgvBookings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBookings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBookings.Location = new Point(78, 364);
            dgvBookings.Name = "dgvBookings";
            dgvBookings.ReadOnly = true;
            dgvBookings.Size = new Size(1067, 335);
            dgvBookings.TabIndex = 23;
            // 
            // btnCheckIn
            // 
            btnCheckIn.Anchor = AnchorStyles.None;
            btnCheckIn.BackColor = Color.FromArgb(226, 212, 183);
            btnCheckIn.FlatAppearance.BorderSize = 2;
            btnCheckIn.FlatStyle = FlatStyle.Flat;
            btnCheckIn.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold);
            btnCheckIn.ForeColor = Color.White;
            btnCheckIn.Location = new Point(78, 246);
            btnCheckIn.Name = "btnCheckIn";
            btnCheckIn.Size = new Size(126, 48);
            btnCheckIn.TabIndex = 15;
            btnCheckIn.Text = "CheckIn";
            btnCheckIn.UseVisualStyleBackColor = false;
            // 
            // btnCheckOut
            // 
            btnCheckOut.Anchor = AnchorStyles.None;
            btnCheckOut.BackColor = Color.FromArgb(226, 212, 183);
            btnCheckOut.FlatAppearance.BorderSize = 2;
            btnCheckOut.FlatStyle = FlatStyle.Flat;
            btnCheckOut.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold);
            btnCheckOut.ForeColor = Color.White;
            btnCheckOut.Location = new Point(226, 246);
            btnCheckOut.Name = "btnCheckOut";
            btnCheckOut.Size = new Size(126, 48);
            btnCheckOut.TabIndex = 24;
            btnCheckOut.Text = "CheckOut";
            btnCheckOut.UseVisualStyleBackColor = false;
            // 
            // btnCancelNoShow
            // 
            btnCancelNoShow.Anchor = AnchorStyles.None;
            btnCancelNoShow.BackColor = Color.FromArgb(226, 212, 183);
            btnCancelNoShow.FlatAppearance.BorderSize = 2;
            btnCancelNoShow.FlatStyle = FlatStyle.Flat;
            btnCancelNoShow.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold);
            btnCancelNoShow.ForeColor = Color.White;
            btnCancelNoShow.Location = new Point(374, 246);
            btnCancelNoShow.Name = "btnCancelNoShow";
            btnCancelNoShow.Size = new Size(234, 48);
            btnCancelNoShow.TabIndex = 25;
            btnCancelNoShow.Text = "Cancel for No-Show";
            btnCancelNoShow.UseVisualStyleBackColor = false;
            // 
            // UCBookings
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(202, 219, 200);
            Controls.Add(pnlUCBookings);
            Controls.Add(pnlAddBooking);
            Controls.Add(btnCancelNoShow);
            Controls.Add(btnCheckOut);
            Controls.Add(btnCheckIn);
            Controls.Add(dgvBookings);
            Font = new Font("Sylfaen", 12F);
            Margin = new Padding(4);
            Name = "UCBookings";
            Size = new Size(1286, 900);
            pnlUCBookings.ResumeLayout(false);
            pnlUCBookings.PerformLayout();
            pnlAddBooking.ResumeLayout(false);
            pnlAddBooking.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudGuests).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvBookings).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlUCBookings;
        private Button btnDeleteBooking;
        private Button btnEditBooking;
        private Button btnAddBooking;
        private Label label1;
        private DateTimePicker dtpCheckIn;
        private DateTimePicker dtpCheckOut;
        private NumericUpDown nudGuests;
        private ComboBox cmbRoomType;
        private Panel pnlAddBooking;
        private Label label3;
        private Label label2;
        private Label lblAddOrEdit;
        private Label label7;
        private Label label9;
        private Label label4;
        private Button btnSaveBooking;
        private Button btnCancelBooking;
        private DataGridView dgvBookings;
        private Label lblReservationDate;
        private ComboBox cmbGuest;
        private ComboBox cmbRoom;
        private Label label5;
        private Label label6;
        private Button btnCheckIn;
        private Button btnCheckOut;
        private Button btnCancelNoShow;
    }
}
