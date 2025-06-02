using HotelManagementApp.Forms;

namespace HotelManagementApp
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void btnDataBase_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            using (Dashboard fd = new Dashboard())
            {
                fd.ShowDialog();
            }

        }

        private void btnClose_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
