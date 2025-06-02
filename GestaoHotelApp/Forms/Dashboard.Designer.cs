namespace HotelManagementApp.Forms
{
    partial class Dashboard
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dashboard));
            panel1 = new Panel();
            btnCredits = new Button();
            btnExtras = new Button();
            btnPayInvo = new Button();
            btnBookings = new Button();
            btnGuests = new Button();
            btnRooms = new Button();
            panel2 = new Panel();
            label3 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            panel3 = new Panel();
            btnClose = new Button();
            panel4 = new Panel();
            lblTimerTime = new Label();
            lblTime = new Label();
            lblUserRole = new Label();
            label5 = new Label();
            lblUserName = new Label();
            label1 = new Label();
            timerTime = new System.Windows.Forms.Timer(components);
            panelMain = new Panel();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(176, 187, 191);
            panel1.Controls.Add(btnCredits);
            panel1.Controls.Add(btnExtras);
            panel1.Controls.Add(btnPayInvo);
            panel1.Controls.Add(btnBookings);
            panel1.Controls.Add(btnGuests);
            panel1.Controls.Add(btnRooms);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 720);
            panel1.TabIndex = 0;
            // 
            // btnCredits
            // 
            btnCredits.Anchor = AnchorStyles.None;
            btnCredits.BackColor = Color.FromArgb(226, 212, 183);
            btnCredits.FlatAppearance.BorderSize = 2;
            btnCredits.FlatStyle = FlatStyle.Flat;
            btnCredits.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCredits.ForeColor = Color.White;
            btnCredits.Location = new Point(42, 516);
            btnCredits.Name = "btnCredits";
            btnCredits.Size = new Size(158, 48);
            btnCredits.TabIndex = 16;
            btnCredits.Text = "Credits";
            btnCredits.UseVisualStyleBackColor = false;
            btnCredits.Click += btnCredits_Click;
            // 
            // btnExtras
            // 
            btnExtras.Anchor = AnchorStyles.None;
            btnExtras.BackColor = Color.FromArgb(226, 212, 183);
            btnExtras.FlatAppearance.BorderSize = 2;
            btnExtras.FlatStyle = FlatStyle.Flat;
            btnExtras.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExtras.ForeColor = Color.White;
            btnExtras.Location = new Point(42, 389);
            btnExtras.Name = "btnExtras";
            btnExtras.Size = new Size(158, 48);
            btnExtras.TabIndex = 15;
            btnExtras.Text = "Extras";
            btnExtras.UseVisualStyleBackColor = false;
            btnExtras.Click += btnExtras_Click;
            // 
            // btnPayInvo
            // 
            btnPayInvo.Anchor = AnchorStyles.None;
            btnPayInvo.BackColor = Color.FromArgb(226, 212, 183);
            btnPayInvo.FlatAppearance.BorderSize = 2;
            btnPayInvo.FlatStyle = FlatStyle.Flat;
            btnPayInvo.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPayInvo.ForeColor = Color.White;
            btnPayInvo.Location = new Point(42, 453);
            btnPayInvo.Name = "btnPayInvo";
            btnPayInvo.Size = new Size(158, 48);
            btnPayInvo.TabIndex = 14;
            btnPayInvo.Text = "Pay/Invoice";
            btnPayInvo.UseVisualStyleBackColor = false;
            btnPayInvo.Click += btnPayInvo_Click;
            // 
            // btnBookings
            // 
            btnBookings.Anchor = AnchorStyles.None;
            btnBookings.BackColor = Color.FromArgb(226, 212, 183);
            btnBookings.FlatAppearance.BorderSize = 2;
            btnBookings.FlatStyle = FlatStyle.Flat;
            btnBookings.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBookings.ForeColor = Color.White;
            btnBookings.Location = new Point(42, 325);
            btnBookings.Name = "btnBookings";
            btnBookings.Size = new Size(158, 48);
            btnBookings.TabIndex = 12;
            btnBookings.Text = "Bookings";
            btnBookings.UseVisualStyleBackColor = false;
            btnBookings.Click += btnBookings_Click;
            // 
            // btnGuests
            // 
            btnGuests.Anchor = AnchorStyles.None;
            btnGuests.BackColor = Color.FromArgb(226, 212, 183);
            btnGuests.FlatAppearance.BorderSize = 2;
            btnGuests.FlatStyle = FlatStyle.Flat;
            btnGuests.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGuests.ForeColor = Color.White;
            btnGuests.Location = new Point(42, 261);
            btnGuests.Name = "btnGuests";
            btnGuests.Size = new Size(158, 48);
            btnGuests.TabIndex = 11;
            btnGuests.Text = "Guests";
            btnGuests.UseVisualStyleBackColor = false;
            btnGuests.Click += btnGuests_Click;
            // 
            // btnRooms
            // 
            btnRooms.Anchor = AnchorStyles.None;
            btnRooms.BackColor = Color.FromArgb(226, 212, 183);
            btnRooms.FlatAppearance.BorderSize = 2;
            btnRooms.FlatStyle = FlatStyle.Flat;
            btnRooms.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRooms.ForeColor = Color.White;
            btnRooms.Location = new Point(42, 195);
            btnRooms.Name = "btnRooms";
            btnRooms.Size = new Size(158, 48);
            btnRooms.TabIndex = 10;
            btnRooms.Text = "Rooms";
            btnRooms.UseVisualStyleBackColor = false;
            btnRooms.Click += btnRooms_Click_1;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(176, 187, 191);
            panel2.Controls.Add(label3);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(pictureBox1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(200, 174);
            panel2.TabIndex = 1;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.Font = new Font("Sylfaen", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.White;
            label3.Location = new Point(3, 137);
            label3.Name = "label3";
            label3.Size = new Size(194, 25);
            label3.TabIndex = 5;
            label3.Text = "Lisboa HI Suites && Spa";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.None;
            label2.AutoSize = true;
            label2.Font = new Font("Sylfaen", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.White;
            label2.Location = new Point(30, 112);
            label2.Name = "label2";
            label2.Size = new Size(134, 25);
            label2.TabIndex = 4;
            label2.Text = "StaySafe Hotels";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(56, 27);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(89, 66);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // panel3
            // 
            panel3.Controls.Add(btnClose);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(200, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(1000, 63);
            panel3.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(226, 212, 183);
            btnClose.FlatAppearance.BorderSize = 2;
            btnClose.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.White;
            btnClose.Image = (Image)resources.GetObject("btnClose.Image");
            btnClose.Location = new Point(932, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(68, 63);
            btnClose.TabIndex = 12;
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click_1;
            // 
            // panel4
            // 
            panel4.BackColor = Color.FromArgb(176, 187, 191);
            panel4.Controls.Add(lblTimerTime);
            panel4.Controls.Add(lblTime);
            panel4.Controls.Add(lblUserRole);
            panel4.Controls.Add(label5);
            panel4.Controls.Add(lblUserName);
            panel4.Controls.Add(label1);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(200, 63);
            panel4.Name = "panel4";
            panel4.Size = new Size(1000, 111);
            panel4.TabIndex = 2;
            // 
            // lblTimerTime
            // 
            lblTimerTime.Anchor = AnchorStyles.None;
            lblTimerTime.AutoSize = true;
            lblTimerTime.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimerTime.ForeColor = Color.White;
            lblTimerTime.Location = new Point(832, 59);
            lblTimerTime.Name = "lblTimerTime";
            lblTimerTime.Size = new Size(112, 27);
            lblTimerTime.TabIndex = 8;
            lblTimerTime.Text = "HH:MM:SS";
            // 
            // lblTime
            // 
            lblTime.Anchor = AnchorStyles.None;
            lblTime.AutoSize = true;
            lblTime.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTime.ForeColor = Color.White;
            lblTime.Location = new Point(832, 59);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(0, 27);
            lblTime.TabIndex = 7;
            // 
            // lblUserRole
            // 
            lblUserRole.Anchor = AnchorStyles.None;
            lblUserRole.AutoSize = true;
            lblUserRole.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUserRole.ForeColor = Color.White;
            lblUserRole.Location = new Point(148, 59);
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(91, 27);
            lblUserRole.TabIndex = 6;
            lblUserRole.Text = "userRole";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.None;
            label5.AutoSize = true;
            label5.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.ForeColor = Color.White;
            label5.Location = new Point(84, 59);
            label5.Name = "label5";
            label5.Size = new Size(58, 27);
            label5.TabIndex = 5;
            label5.Text = "Role:";
            // 
            // lblUserName
            // 
            lblUserName.Anchor = AnchorStyles.None;
            lblUserName.AutoSize = true;
            lblUserName.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblUserName.ForeColor = Color.White;
            lblUserName.Location = new Point(148, 23);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(103, 27);
            lblUserName.TabIndex = 4;
            lblUserName.Text = "userName";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.White;
            label1.Location = new Point(39, 23);
            label1.Name = "label1";
            label1.Size = new Size(103, 27);
            label1.TabIndex = 3;
            label1.Text = "Welcome:";
            // 
            // timerTime
            // 
            timerTime.Tick += timerTime_Tick;
            // 
            // panelMain
            // 
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(200, 174);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1000, 546);
            panelMain.TabIndex = 3;
            // 
            // Dashboard
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(202, 219, 200);
            ClientSize = new Size(1200, 720);
            Controls.Add(panelMain);
            Controls.Add(panel4);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Font = new Font("Sylfaen", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Dashboard";
            Text = "Dashboard";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private PictureBox pictureBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btnPayInvo;
        private Button btnBookings;
        private Button btnGuests;
        private Button btnRooms;
        private Button btnExtras;
        private Label lblUserName;
        private Button btnClose;
        private Label lblTime;
        private Label lblUserRole;
        private Label label5;
        private System.Windows.Forms.Timer timerTime;
        private Label lblTimerTime;
        private Panel panelMain;
        private Button btnCredits;
    }
}