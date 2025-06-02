namespace HotelManagementApp.UserControls
{
    public partial class UCCredits : UserControl
    {
        public UCCredits()
        {
            InitializeComponent();
            LoadCreditsInfo();
        }

        private void LoadCreditsInfo()
        {
            rtbCredits.Text =
            @"

Project developed by: Gonçalo Russo
Version: 1.0.1
Release date: April 2025

About the system:
This software was developed within the scope of the curricular unit UF5413 – Object-Oriented Programming, as part of the Advanced Technician Course in Information Systems Technologies and Programming (CET.TPSI.D.L.97).

The purpose of the project was to simulate a fully functional hotel management system (Property Management System – PMS) and apply object-oriented principles in a practical and structured way.

The system provides features for managing guests, room bookings, room rates, additional services, invoicing, and other essential hotel operations.

About StaySafe Hotels:  
StaySafe Hotels is a fictional company created for academic purposes. It specialises in the development and maintenance of software solutions for the hospitality industry, with a strong focus on innovation, simplicity, and data reliability. The StaySafe PMS was designed to streamline the daily management of hotels, ensuring a secure, efficient, and well-organised experience. In this example, the system is applied to the hotel Lisboa HI Suites & Spa.";
        }
    }
}
