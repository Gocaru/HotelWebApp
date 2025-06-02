using HotelDB_WPF_Framework.Enums;
using HotelDB_WPF_Framework.Helpers.InputBuilders;
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
    /// Interaction logic for UCRooms.xaml
    /// </summary>
    public partial class UCRooms : UserControl
    {
        private readonly NetworkService networkService = new NetworkService("http://hoteldbapi.somee.com/api/rooms");

        private readonly ApiService apiService = new ApiService("http://hoteldbapi.somee.com/api");

        private Room quartoSelecionado;

        public UCRooms()
        {
            InitializeComponent();

            // Carregar os enums nos ComboBoxes do painel de adição
            cbAddType.ItemsSource = Enum.GetValues(typeof(RoomType));
            cbAddStatus.ItemsSource = Enum.GetValues(typeof(RoomStatus));

            // Carregar os enums nos ComboBoxes do painel de edição
            cbEditType.ItemsSource = Enum.GetValues(typeof(RoomType));
            cbEditStatus.ItemsSource = Enum.GetValues(typeof(RoomStatus));
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await CarregarQuartosAsync();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddRoom.Visibility = Visibility.Visible;
            pnlRoomList.Visibility = Visibility.Collapsed;

            // Limpar campos
            txtAddNumber.Clear();
            txtAddCapacity.Clear();
            txtAddPrice.Clear();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            quartoSelecionado = dgRooms.SelectedItem as Room;

            if (quartoSelecionado == null)
            {
                MessageBox.Show("Please select a room.", "Edit Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            pnlEditRoom.Visibility = Visibility.Visible;
            pnlRoomList.Visibility = Visibility.Collapsed;

            txtEditNumber.Text = quartoSelecionado.Number.ToString();
            txtEditCapacity.Text = quartoSelecionado.Capacity.ToString();
            cbEditType.SelectedItem = quartoSelecionado.Type;
            txtEditPrice.Text = quartoSelecionado.PricePerNight.ToString();
            cbEditStatus.SelectedItem = quartoSelecionado.Status;
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

            var selectedRoom = dgRooms.SelectedItem as Room;

            if (selectedRoom == null)
            {
                MessageBox.Show("Please select a room to delete.", "Delete Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show($"Are you sure you want to delete room #{selectedRoom.Number}?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                bool success = await apiService.DeleteAsync("rooms", selectedRoom.RoomId);

                if (success)
                {
                    MessageBox.Show("Room deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    await CarregarQuartosAsync();
                }
                else
                {
                    MessageBox.Show("Failed to delete room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (!RoomInputBuilder.TryBuildRoom(
                    txtAddNumber.Text,
                    txtAddCapacity.Text,
                    txtAddPrice.Text,
                    cbAddType.SelectedItem,
                    cbAddStatus.SelectedItem,
                    out Room newRoom,
                    out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                bool success = await apiService.PostAsync("rooms", newRoom);

                if (success)
                {
                    MessageBox.Show("Room added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlAddRoom.Visibility = Visibility.Collapsed;
                    pnlRoomList.Visibility = Visibility.Visible;
                    await CarregarQuartosAsync();
                }
                else
                {
                    MessageBox.Show("Failed to add room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void BtnCancelAdd_Click(object sender, RoutedEventArgs e)
        {
            pnlAddRoom.Visibility = Visibility.Collapsed;
            pnlRoomList.Visibility = Visibility.Visible;
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

            if (quartoSelecionado == null)
            {
                MessageBox.Show("No room selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!RoomInputBuilder.TryBuildRoom(
                    txtEditNumber.Text,
                    txtEditCapacity.Text,
                    txtEditPrice.Text,
                    cbEditType.SelectedItem,
                    cbEditStatus.SelectedItem,
                    out Room editedRoom,
                    out string error))
            {
                MessageBox.Show(error, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Atualizar os dados do objeto selecionado com os novos valores
            quartoSelecionado.Number = editedRoom.Number;
            quartoSelecionado.Capacity = editedRoom.Capacity;
            quartoSelecionado.Type = editedRoom.Type;
            quartoSelecionado.PricePerNight = editedRoom.PricePerNight;
            quartoSelecionado.Status = editedRoom.Status;

            try
            {
                bool success = await apiService.PutAsync($"rooms/{quartoSelecionado.RoomId}", quartoSelecionado);

                if (success)
                {
                    MessageBox.Show("Room updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    pnlEditRoom.Visibility = Visibility.Collapsed;
                    pnlRoomList.Visibility = Visibility.Visible;
                    await CarregarQuartosAsync();
                }
                else
                {
                    MessageBox.Show("Error updating room.", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the API:\n" + ex.Message, "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            pnlEditRoom.Visibility = Visibility.Collapsed;
            pnlRoomList.Visibility = Visibility.Visible;
        }

        private async Task CarregarQuartosAsync()
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
                List<Room> rooms = await apiService.GetAllAsync<Room>("rooms");

                if (rooms != null && rooms.Count > 0)
                {
                    dgRooms.ItemsSource = rooms;
                }
                else
                {
                    dgRooms.ItemsSource = null;
                    MessageBox.Show("No rooms found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading rooms:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditRoom.IsEnabled = dgRooms.SelectedItem != null;
            btnDeleteRoom.IsEnabled = dgRooms.SelectedItem != null;
        }
    }
}

