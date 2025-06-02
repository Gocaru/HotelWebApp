using HotelDB_WPF_Framework.Helpers;
using HotelDB_WPF_Framework.Models;
using HotelDB_WPF_Framework.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HotelDB_WPF_Framework.UserControls
{
    /// <summary>
    /// Interaction logic for UCGuests.xaml
    /// </summary>
    public partial class UCGuests : UserControl
    {
        private readonly NetworkService networkService = new NetworkService("http://hoteldbapi.somee.com/api/guests");

        private readonly ApiService apiService = new ApiService("http://hoteldbapi.somee.com/api");

        private Guest hospedeSelecionado;

        public UCGuests()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarHospedesAsync();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddGuest.Visibility = Visibility.Visible;
            pnlGuestList.Visibility = Visibility.Collapsed;

            txtAddName.Clear();
            txtAddContact.Clear();
            txtAddEmail.Clear();
            txtAddDoc.Clear();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            hospedeSelecionado = dgGuests.SelectedItem as Guest;

            if (hospedeSelecionado == null)
            {
                MessageBox.Show("Please select a guest.", "Edit Guest",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Alternar visibilidade
            pnlEditGuest.Visibility = Visibility.Visible;
            pnlGuestList.Visibility = Visibility.Collapsed;

            // Preencher os campos
            txtEditName.Text = hospedeSelecionado.Name;
            txtEditContact.Text = hospedeSelecionado.Contact;
            txtEditEmail.Text = hospedeSelecionado.Email;
            txtEditDoc.Text = hospedeSelecionado.IdentificationDocument;
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedGuest = dgGuests.SelectedItem as Guest;

            if (selectedGuest == null)
            {
                MessageBox.Show("Please select a guest to delete.",
                    "Delete Guest",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete {selectedGuest.Name}?",
                                          "Confirm Deletion",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                bool success = await apiService.DeleteAsync("guests", selectedGuest.GuestId);

                if (success)
                {
                    MessageBox.Show("Guest deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await CarregarHospedesAsync();
                    btnEditGuest.IsEnabled = false;
                    btnDeleteGuest.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Failed to delete guest.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the API:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void BtnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Cria o objeto com os dados introduzidos
            var newGuest = new Guest
            {
                Name = txtAddName.Text.Trim(),
                Contact = txtAddContact.Text.Trim(),
                Email = txtAddEmail.Text.Trim(),
                IdentificationDocument = txtAddDoc.Text.Trim()
            };

            // Valida o hóspede
            if (!ValidationHelper.ValidateGuest(newGuest, out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await apiService.PostAsync("guests", newGuest);

                if (success)
                {
                    MessageBox.Show("Guest added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlAddGuest.Visibility = Visibility.Collapsed;
                    pnlGuestList.Visibility = Visibility.Visible;
                    await CarregarHospedesAsync();
                }
                else
                {
                    MessageBox.Show("Failed to add guest.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddGuest.Visibility = Visibility.Collapsed;
            pnlGuestList.Visibility = Visibility.Visible;

            hospedeSelecionado = null;
            dgGuests.UnselectAll();
        }


        private async void BtnConfirmEdit_Click(object sender, RoutedEventArgs e)
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (hospedeSelecionado == null)
            {
                MessageBox.Show("No guest selected.", "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Atualiza os dados do hóspede com os valores dos campos
            hospedeSelecionado.Name = txtEditName.Text.Trim();
            hospedeSelecionado.Contact = txtEditContact.Text.Trim();
            hospedeSelecionado.Email = txtEditEmail.Text.Trim();
            hospedeSelecionado.IdentificationDocument = txtEditDoc.Text.Trim();

            // Valida antes de enviar
            if (!ValidationHelper.ValidateGuest(hospedeSelecionado, out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await apiService.PutAsync($"guests/{hospedeSelecionado.GuestId}", hospedeSelecionado);

                if (success)
                {
                    MessageBox.Show("Guest updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlEditGuest.Visibility = Visibility.Collapsed;
                    pnlGuestList.Visibility = Visibility.Visible;
                    await CarregarHospedesAsync();
                }
                else
                {
                    MessageBox.Show("Error updating guest.", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the API:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            pnlEditGuest.Visibility = Visibility.Collapsed;
            pnlGuestList.Visibility = Visibility.Visible;

            hospedeSelecionado = null;
            dgGuests.UnselectAll();
        }

        /// <summary>
        /// Carrega a lista de hóspedes da API e preenche a grelha.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarHospedesAsync()
        {
            if (!networkService.IsInternetAvailable())
            {
                MessageBox.Show("No internet connection.", "Network Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!await networkService.IsApiAvailableAsync())
            {
                MessageBox.Show("The API is currently unavailable.", "API Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                List<Guest> guests = await apiService.GetAllAsync<Guest>("guests");

                if (guests != null && guests.Count > 0)
                {
                    dgGuests.ItemsSource = guests;
                }
                else
                {
                    dgGuests.ItemsSource = null;
                    MessageBox.Show("No guests found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading guests:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        /// <summary>
        /// Torna os botões edit e delete selecionáveis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgGuests_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgGuests.SelectedItem != null)
            {
                btnEditGuest.IsEnabled = true;
                btnDeleteGuest.IsEnabled = true;
            }
            else
            {
                btnEditGuest.IsEnabled = false;
                btnDeleteGuest.IsEnabled = false;
            }
        }
    }
}

