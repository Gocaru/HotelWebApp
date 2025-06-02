using HotelManagement.Core;
using HotelManagementWpfApp_.Helpers;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace HotelManagementWpfApp_.UserControls
{
    /// <summary>
    /// Interaction logic for UCGuests.xaml
    /// </summary>
    public partial class UCGuests : UserControl
    {
        private Guest? hospedeSelecionado;

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
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7176/api/");
                    var response = await client.DeleteAsync($"guests/{selectedGuest.GuestId}");

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Guest deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        await CarregarHospedesAsync();
                        btnEditGuest.IsEnabled = false;
                        btnDeleteGuest.IsEnabled = false;
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete guest: {response.ReasonPhrase}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the API:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            // Cria o objeto com os dados introduzidos
            var newGuest = new Guest
            {
                Name = txtAddName.Text,
                Contact = txtAddContact.Text,
                Email = txtAddEmail.Text,
                IdentificationDocument = txtAddDoc.Text
            };

            // Valida o hóspede
            if (!ValidationHelper.ValidateGuest(newGuest, out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using HttpClient client = new();
                client.BaseAddress = new Uri("https://localhost:7176/api/");
                var response = await client.PostAsJsonAsync("guests", newGuest);

                if (response.IsSuccessStatusCode)
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
            if (hospedeSelecionado == null)
            {
                MessageBox.Show("No guest selected.", "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Atualiza os dados do hóspede com os valores dos campos
            hospedeSelecionado.Name = txtEditName.Text;
            hospedeSelecionado.Contact = txtEditContact.Text;
            hospedeSelecionado.Email = txtEditEmail.Text;
            hospedeSelecionado.IdentificationDocument = txtEditDoc.Text;

            // Valida antes de enviar
            if (!ValidationHelper.ValidateGuest(hospedeSelecionado, out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7176/api/");
                    HttpResponseMessage response = await client.PutAsJsonAsync($"guests/{hospedeSelecionado.GuestId}", hospedeSelecionado);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Guest updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        pnlAddGuest.Visibility = Visibility.Collapsed;
                        pnlGuestList.Visibility = Visibility.Visible;
                        await CarregarHospedesAsync();
                    }
                    else
                    {
                        MessageBox.Show($"Error updating guest: {response.ReasonPhrase}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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


        private async Task CarregarHospedesAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7176/api/");
                    var guests = await client.GetFromJsonAsync<List<Guest>>("guests");

                    if (guests != null)
                    {
                        dgGuests.ItemsSource = guests;
                    }
                    else
                    {
                        MessageBox.Show("Nenhum hóspede encontrado.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar hóspedes:\n" + ex.Message);
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
