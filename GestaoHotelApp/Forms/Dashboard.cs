using HotelManagementApp.UserControls;
using System.Globalization;

namespace HotelManagementApp.Forms
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            timerTime.Start();
        }

        private void timerTime_Tick(object sender, EventArgs e)
        {
            DateTime agora = DateTime.Now;
            lblTimerTime.Text = agora.ToString("dddd, d 'de' MMMM 'de' yyyy - HH:mm:ss", new CultureInfo("pt-PT"));
        }

        private void btnRooms_Click_1(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            var uc = new UCRooms();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);

        }

        private void btnGuests_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            var uc = new UCGuests();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);

        }

        private void btnBookings_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            var uc = new UCBookings();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);

        }

        private void btnPayInvo_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            var uc = new UCInvoices();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);

        }

        private void btnExtras_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            var uc = new UCExtras();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);

        }


        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCredits_Click(object sender, EventArgs e)
        {
            panelMain.Controls.Clear();
            var uc = new UCCredits();
            uc.Dock = DockStyle.Fill;
            panelMain.Controls.Add(uc);
        }
    }
}
