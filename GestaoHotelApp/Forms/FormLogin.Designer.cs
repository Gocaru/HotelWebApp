namespace HotelManagementApp
{
    partial class FormLogin
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            panel1 = new Panel();
            btnClose = new Button();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            label1 = new Label();
            pictureBox2 = new PictureBox();
            label3 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label4 = new Label();
            btnLogin = new Button();
            label5 = new Label();
            label6 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(176, 187, 191);
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1200, 77);
            panel1.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(226, 212, 183);
            btnClose.Dock = DockStyle.Right;
            btnClose.FlatAppearance.BorderSize = 2;
            btnClose.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.White;
            btnClose.Image = (Image)resources.GetObject("btnClose.Image");
            btnClose.Location = new Point(1099, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(101, 77);
            btnClose.TabIndex = 11;
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click_1;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(23, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(62, 50);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(100, 25);
            label2.Name = "label2";
            label2.Size = new Size(165, 27);
            label2.TabIndex = 2;
            label2.Text = "StaySafe Hotels";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.None;
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(456, 338);
            label1.Name = "label1";
            label1.Size = new Size(116, 27);
            label1.TabIndex = 1;
            label1.Text = "User Name:";
            // 
            // pictureBox2
            // 
            pictureBox2.Anchor = AnchorStyles.None;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(552, 180);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(96, 92);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.None;
            label3.AutoSize = true;
            label3.Font = new Font("Sylfaen", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(474, 275);
            label3.Name = "label3";
            label3.Size = new Size(271, 42);
            label3.TabIndex = 4;
            label3.Text = "Please Login First";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.None;
            textBox1.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox1.Location = new Point(456, 368);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(289, 35);
            textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.None;
            textBox2.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            textBox2.Location = new Point(456, 449);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(289, 35);
            textBox2.TabIndex = 7;
            textBox2.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.None;
            label4.AutoSize = true;
            label4.Font = new Font("Sylfaen", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(456, 419);
            label4.Name = "label4";
            label4.Size = new Size(101, 27);
            label4.TabIndex = 6;
            label4.Text = "Password:";
            // 
            // btnLogin
            // 
            btnLogin.Anchor = AnchorStyles.None;
            btnLogin.BackColor = Color.FromArgb(226, 212, 183);
            btnLogin.FlatAppearance.BorderSize = 2;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Sylfaen", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(456, 509);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(289, 48);
            btnLogin.TabIndex = 8;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += btnLogin_Click_1;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.None;
            label5.AutoSize = true;
            label5.Font = new Font("Sylfaen", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label5.Location = new Point(537, 573);
            label5.Name = "label5";
            label5.Size = new Size(126, 22);
            label5.TabIndex = 9;
            label5.Text = "Forgot Password?";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Font = new Font("Sylfaen", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label6.Location = new Point(23, 679);
            label6.Name = "label6";
            label6.Size = new Size(317, 18);
            label6.TabIndex = 10;
            label6.Text = "CopyRights © 2025. All Rights Reserved By StaySafe PMS";
            // 
            // FormLogin
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(202, 219, 200);
            ClientSize = new Size(1200, 720);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(btnLogin);
            Controls.Add(textBox2);
            Controls.Add(label4);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(pictureBox2);
            Controls.Add(label1);
            Controls.Add(panel1);
            Font = new Font("Sylfaen", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormLogin";
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label2;
        private Label label1;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Label label3;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label4;
        private Button btnLogin;
        private Label label5;
        private Label label6;
        private Button btnClose;
    }
}
