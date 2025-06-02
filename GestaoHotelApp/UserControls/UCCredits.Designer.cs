namespace HotelManagementApp.UserControls
{
    partial class UCCredits
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
            pnlUUCCredits = new Panel();
            label1 = new Label();
            rtbCredits = new RichTextBox();
            pnlUUCCredits.SuspendLayout();
            SuspendLayout();
            // 
            // pnlUUCCredits
            // 
            pnlUUCCredits.BackColor = Color.FromArgb(156, 149, 131);
            pnlUUCCredits.Controls.Add(label1);
            pnlUUCCredits.Dock = DockStyle.Top;
            pnlUUCCredits.Location = new Point(0, 0);
            pnlUUCCredits.Margin = new Padding(4);
            pnlUUCCredits.Name = "pnlUUCCredits";
            pnlUUCCredits.Size = new Size(1286, 155);
            pnlUUCCredits.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Sylfaen", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(78, 60);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(278, 46);
            label1.TabIndex = 0;
            label1.Text = "Credits Overview";
            // 
            // rtbCredits
            // 
            rtbCredits.BackColor = Color.FromArgb(202, 219, 200);
            rtbCredits.BorderStyle = BorderStyle.None;
            rtbCredits.Location = new Point(204, 279);
            rtbCredits.Name = "rtbCredits";
            rtbCredits.ReadOnly = true;
            rtbCredits.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbCredits.Size = new Size(992, 420);
            rtbCredits.TabIndex = 41;
            rtbCredits.Text = "";
            // 
            // UCCredits
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(202, 219, 200);
            Controls.Add(pnlUUCCredits);
            Controls.Add(rtbCredits);
            Font = new Font("Sylfaen", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "UCCredits";
            Size = new Size(1286, 801);
            pnlUUCCredits.ResumeLayout(false);
            pnlUUCCredits.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlUUCCredits;
        private Label label1;
        private Button btnEditGuest;
        private Button btnAddGuest;
        private Button btnDeleteGuest;
        private Label lblAddOrEdit;
        private Button btnSaveGuest;
        private TextBox txtGuestEmail;
        private TextBox txtGuestContact;
        private Label label7;
        private Label label9;
        private TextBox txtGuestName;
        private TextBox txtGuestDocument;
        private Label label2;
        private Label label3;
        private RichTextBox rtbCredits;
        private Button btnViewHistory;
    }
}
