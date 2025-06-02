namespace HotelManagementApp.UserControls
{
    partial class UCRooms
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
            pnlUCRooms = new Panel();
            panel1 = new Panel();
            btnDeleteRoom = new Button();
            btnEditRoom = new Button();
            btnAddRoom = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            cmbFilterRoomType = new ComboBox();
            cmbFilterRoomStatus = new ComboBox();
            dgvRooms = new DataGridView();
            label4 = new Label();
            pnlAddRoom = new Panel();
            lblAddOrEdit = new Label();
            btnSaveAddRoom = new Button();
            btnCancelAddRoom = new Button();
            cmbAddRoomStatus = new ComboBox();
            label5 = new Label();
            txtAddRoomPrice = new TextBox();
            txtAddRoomCapacity = new TextBox();
            label7 = new Label();
            label8 = new Label();
            label9 = new Label();
            txtAddRoomNumber = new TextBox();
            cmbAddRoomType = new ComboBox();
            label10 = new Label();
            nudFilterMaxPrice = new NumericUpDown();
            pnlUCRooms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).BeginInit();
            pnlAddRoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudFilterMaxPrice).BeginInit();
            SuspendLayout();
            // 
            // pnlUCRooms
            // 
            pnlUCRooms.BackColor = Color.FromArgb(156, 149, 131);
            pnlUCRooms.Controls.Add(panel1);
            pnlUCRooms.Controls.Add(btnDeleteRoom);
            pnlUCRooms.Controls.Add(btnEditRoom);
            pnlUCRooms.Controls.Add(btnAddRoom);
            pnlUCRooms.Controls.Add(label1);
            pnlUCRooms.Dock = DockStyle.Top;
            pnlUCRooms.Location = new Point(0, 0);
            pnlUCRooms.Margin = new Padding(4);
            pnlUCRooms.Name = "pnlUCRooms";
            pnlUCRooms.Size = new Size(1286, 155);
            pnlUCRooms.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Location = new Point(0, 153);
            panel1.Name = "panel1";
            panel1.Size = new Size(1283, 645);
            panel1.TabIndex = 21;
            // 
            // btnDeleteRoom
            // 
            btnDeleteRoom.Anchor = AnchorStyles.None;
            btnDeleteRoom.BackColor = Color.FromArgb(226, 212, 183);
            btnDeleteRoom.FlatAppearance.BorderSize = 2;
            btnDeleteRoom.FlatStyle = FlatStyle.Flat;
            btnDeleteRoom.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDeleteRoom.ForeColor = Color.White;
            btnDeleteRoom.Location = new Point(1052, 51);
            btnDeleteRoom.Name = "btnDeleteRoom";
            btnDeleteRoom.Size = new Size(93, 48);
            btnDeleteRoom.TabIndex = 14;
            btnDeleteRoom.Text = "Delete";
            btnDeleteRoom.UseVisualStyleBackColor = false;
            // 
            // btnEditRoom
            // 
            btnEditRoom.Anchor = AnchorStyles.None;
            btnEditRoom.BackColor = Color.FromArgb(226, 212, 183);
            btnEditRoom.FlatAppearance.BorderSize = 2;
            btnEditRoom.FlatStyle = FlatStyle.Flat;
            btnEditRoom.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEditRoom.ForeColor = Color.White;
            btnEditRoom.Location = new Point(934, 51);
            btnEditRoom.Name = "btnEditRoom";
            btnEditRoom.Size = new Size(93, 48);
            btnEditRoom.TabIndex = 11;
            btnEditRoom.Text = "Edit";
            btnEditRoom.UseVisualStyleBackColor = false;
            // 
            // btnAddRoom
            // 
            btnAddRoom.Anchor = AnchorStyles.None;
            btnAddRoom.BackColor = Color.FromArgb(226, 212, 183);
            btnAddRoom.FlatAppearance.BorderSize = 2;
            btnAddRoom.FlatStyle = FlatStyle.Flat;
            btnAddRoom.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAddRoom.ForeColor = Color.White;
            btnAddRoom.Location = new Point(816, 51);
            btnAddRoom.Name = "btnAddRoom";
            btnAddRoom.Size = new Size(93, 48);
            btnAddRoom.TabIndex = 10;
            btnAddRoom.Text = "Add";
            btnAddRoom.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(78, 60);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(274, 46);
            label1.TabIndex = 0;
            label1.Text = "Rooms Overview";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(78, 245);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(196, 39);
            label2.TabIndex = 14;
            label2.Text = "Filter by Type:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.Location = new Point(464, 245);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(97, 39);
            label3.TabIndex = 15;
            label3.Text = "Status:";
            // 
            // cmbFilterRoomType
            // 
            cmbFilterRoomType.FormattingEnabled = true;
            cmbFilterRoomType.Location = new Point(281, 254);
            cmbFilterRoomType.Name = "cmbFilterRoomType";
            cmbFilterRoomType.Size = new Size(161, 30);
            cmbFilterRoomType.TabIndex = 16;
            // 
            // cmbFilterRoomStatus
            // 
            cmbFilterRoomStatus.FormattingEnabled = true;
            cmbFilterRoomStatus.Location = new Point(568, 255);
            cmbFilterRoomStatus.Name = "cmbFilterRoomStatus";
            cmbFilterRoomStatus.Size = new Size(161, 30);
            cmbFilterRoomStatus.TabIndex = 17;
            // 
            // dgvRooms
            // 
            dgvRooms.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRooms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRooms.Location = new Point(78, 364);
            dgvRooms.Name = "dgvRooms";
            dgvRooms.ReadOnly = true;
            dgvRooms.Size = new Size(1067, 335);
            dgvRooms.TabIndex = 18;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(772, 244);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(87, 39);
            label4.TabIndex = 19;
            label4.Text = "Price:";
            // 
            // pnlAddRoom
            // 
            pnlAddRoom.Controls.Add(lblAddOrEdit);
            pnlAddRoom.Controls.Add(btnSaveAddRoom);
            pnlAddRoom.Controls.Add(btnCancelAddRoom);
            pnlAddRoom.Controls.Add(cmbAddRoomStatus);
            pnlAddRoom.Controls.Add(label5);
            pnlAddRoom.Controls.Add(txtAddRoomPrice);
            pnlAddRoom.Controls.Add(txtAddRoomCapacity);
            pnlAddRoom.Controls.Add(label7);
            pnlAddRoom.Controls.Add(label8);
            pnlAddRoom.Controls.Add(label9);
            pnlAddRoom.Controls.Add(txtAddRoomNumber);
            pnlAddRoom.Controls.Add(cmbAddRoomType);
            pnlAddRoom.Controls.Add(label10);
            pnlAddRoom.Location = new Point(0, 153);
            pnlAddRoom.Name = "pnlAddRoom";
            pnlAddRoom.Size = new Size(1286, 648);
            pnlAddRoom.TabIndex = 21;
            pnlAddRoom.Visible = false;
            // 
            // lblAddOrEdit
            // 
            lblAddOrEdit.AutoSize = true;
            lblAddOrEdit.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAddOrEdit.Location = new Point(657, 121);
            lblAddOrEdit.Margin = new Padding(4, 0, 4, 0);
            lblAddOrEdit.Name = "lblAddOrEdit";
            lblAddOrEdit.Size = new Size(255, 46);
            lblAddOrEdit.TabIndex = 42;
            lblAddOrEdit.Text = "Add New Room";
            // 
            // btnSaveAddRoom
            // 
            btnSaveAddRoom.Anchor = AnchorStyles.None;
            btnSaveAddRoom.BackColor = Color.FromArgb(226, 212, 183);
            btnSaveAddRoom.FlatAppearance.BorderSize = 2;
            btnSaveAddRoom.FlatStyle = FlatStyle.Flat;
            btnSaveAddRoom.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveAddRoom.ForeColor = Color.White;
            btnSaveAddRoom.Location = new Point(910, 478);
            btnSaveAddRoom.Name = "btnSaveAddRoom";
            btnSaveAddRoom.Size = new Size(93, 48);
            btnSaveAddRoom.TabIndex = 41;
            btnSaveAddRoom.Text = "Save";
            btnSaveAddRoom.UseVisualStyleBackColor = false;
            // 
            // btnCancelAddRoom
            // 
            btnCancelAddRoom.Anchor = AnchorStyles.None;
            btnCancelAddRoom.BackColor = Color.FromArgb(226, 212, 183);
            btnCancelAddRoom.FlatAppearance.BorderSize = 2;
            btnCancelAddRoom.FlatStyle = FlatStyle.Flat;
            btnCancelAddRoom.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelAddRoom.ForeColor = Color.White;
            btnCancelAddRoom.Location = new Point(792, 478);
            btnCancelAddRoom.Name = "btnCancelAddRoom";
            btnCancelAddRoom.Size = new Size(93, 48);
            btnCancelAddRoom.TabIndex = 40;
            btnCancelAddRoom.Text = "Cancel";
            btnCancelAddRoom.UseVisualStyleBackColor = false;
            // 
            // cmbAddRoomStatus
            // 
            cmbAddRoomStatus.FormattingEnabled = true;
            cmbAddRoomStatus.Location = new Point(509, 497);
            cmbAddRoomStatus.Name = "cmbAddRoomStatus";
            cmbAddRoomStatus.Size = new Size(185, 30);
            cmbAddRoomStatus.TabIndex = 39;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(397, 487);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(97, 39);
            label5.TabIndex = 38;
            label5.Text = "Status:";
            // 
            // txtAddRoomPrice
            // 
            txtAddRoomPrice.Location = new Point(509, 429);
            txtAddRoomPrice.Name = "txtAddRoomPrice";
            txtAddRoomPrice.Size = new Size(185, 29);
            txtAddRoomPrice.TabIndex = 37;
            // 
            // txtAddRoomCapacity
            // 
            txtAddRoomCapacity.Location = new Point(509, 360);
            txtAddRoomCapacity.Name = "txtAddRoomCapacity";
            txtAddRoomCapacity.Size = new Size(185, 29);
            txtAddRoomCapacity.TabIndex = 36;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label7.Location = new Point(284, 419);
            label7.Margin = new Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new Size(210, 39);
            label7.TabIndex = 35;
            label7.Text = "Price per Night:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label8.Location = new Point(363, 350);
            label8.Margin = new Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new Size(131, 39);
            label8.TabIndex = 34;
            label8.Text = "Capacity:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label9.Location = new Point(410, 280);
            label9.Margin = new Padding(4, 0, 4, 0);
            label9.Name = "label9";
            label9.Size = new Size(84, 39);
            label9.TabIndex = 33;
            label9.Text = "Type:";
            // 
            // txtAddRoomNumber
            // 
            txtAddRoomNumber.Location = new Point(509, 219);
            txtAddRoomNumber.Name = "txtAddRoomNumber";
            txtAddRoomNumber.Size = new Size(185, 29);
            txtAddRoomNumber.TabIndex = 32;
            // 
            // cmbAddRoomType
            // 
            cmbAddRoomType.FormattingEnabled = true;
            cmbAddRoomType.Location = new Point(509, 290);
            cmbAddRoomType.Name = "cmbAddRoomType";
            cmbAddRoomType.Size = new Size(185, 30);
            cmbAddRoomType.TabIndex = 31;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new Font("Sylfaen", 21.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label10.Location = new Point(370, 209);
            label10.Margin = new Padding(4, 0, 4, 0);
            label10.Name = "label10";
            label10.Size = new Size(124, 39);
            label10.TabIndex = 30;
            label10.Text = "Number:";
            // 
            // nudFilterMaxPrice
            // 
            nudFilterMaxPrice.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            nudFilterMaxPrice.Location = new Point(870, 254);
            nudFilterMaxPrice.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            nudFilterMaxPrice.Name = "nudFilterMaxPrice";
            nudFilterMaxPrice.Size = new Size(161, 29);
            nudFilterMaxPrice.TabIndex = 24;
            // 
            // UCRooms
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(202, 219, 200);
            Controls.Add(pnlUCRooms);
            Controls.Add(pnlAddRoom);
            Controls.Add(label4);
            Controls.Add(nudFilterMaxPrice);
            Controls.Add(dgvRooms);
            Controls.Add(cmbFilterRoomStatus);
            Controls.Add(cmbFilterRoomType);
            Controls.Add(label3);
            Controls.Add(label2);
            Font = new Font("Sylfaen", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "UCRooms";
            Size = new Size(1286, 801);
            pnlUCRooms.ResumeLayout(false);
            pnlUCRooms.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRooms).EndInit();
            pnlAddRoom.ResumeLayout(false);
            pnlAddRoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudFilterMaxPrice).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlUCRooms;
        private Label label1;
        private Button btnEditRoom;
        private Button btnAddRoom;
        private Label label2;
        private Label label3;
        private Button btnDeleteRoom;
        private ComboBox cmbFilterRoomType;
        private ComboBox cmbFilterRoomStatus;
        private DataGridView dgvRooms;
        private Label label4;
        private Panel panel1;
        private Panel pnlAddRoom;
        private Label lblAddOrEdit;
        private Button btnSaveAddRoom;
        private Button btnCancelAddRoom;
        private ComboBox cmbAddRoomStatus;
        private Label label5;
        private TextBox txtAddRoomPrice;
        private TextBox txtAddRoomCapacity;
        private Label label7;
        private Label label8;
        private Label label9;
        private TextBox txtAddRoomNumber;
        private ComboBox cmbAddRoomType;
        private Label label10;
        private NumericUpDown nudFilterMaxPrice;
    }
}
