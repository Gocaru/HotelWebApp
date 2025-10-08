using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly UserService _userService;
        private readonly ReservationService _reservationService;

        [ObservableProperty]
        private UserDto? profile;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isEditing;

        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private string? phoneNumber;

        [ObservableProperty]
        private string? identificationDocument;

        public ProfileViewModel(UserService userService, ReservationService reservationService)
        {
            _userService = userService;
            _reservationService = reservationService;
        }

        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Carrega perfil do utilizador
                var profileResponse = await _userService.GetProfileAsync();

                if (!profileResponse.Success || profileResponse.Data == null)
                {
                    await Shell.Current.DisplayAlert("Error", profileResponse.Message, "OK");
                    return;
                }

                Profile = profileResponse.Data;

                // Carrega sempre as reservas para calcular estatísticas
                var reservationsResponse = await _reservationService.GetMyReservationsAsync();

                if (reservationsResponse.Success && reservationsResponse.Data != null)
                {
                    var reservations = reservationsResponse.Data;

                    System.Diagnostics.Debug.WriteLine($"\n=== RESERVATION STATISTICS DEBUG ===");
                    System.Diagnostics.Debug.WriteLine($"Total Reservations: {reservations.Count}");

                    // Filtro as reservas válidas (excluo as canceladas)
                    var validReservations = reservations
                        .Where(r => !r.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                        .ToList();


                    // Calcula as estatísticas
                    Profile.TotalReservations = validReservations.Count;
                    System.Diagnostics.Debug.WriteLine($"Valid Reservations (excludes Cancelled): {Profile.TotalReservations}");

                    // Past Stays = Completed
                    var completedList = validReservations
                        .Where(r => r.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    Profile.CompletedStays = completedList.Count;
                    System.Diagnostics.Debug.WriteLine($"Completed: {Profile.CompletedStays}");

                    // Upcoming = Confirmed/CheckedIn com data futura ou hoje
                    var upcomingList = validReservations
                        .Where(r =>
                            (r.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) ||
                             r.Status.Equals("CheckedIn", StringComparison.OrdinalIgnoreCase)) &&
                            r.CheckInDate >= DateTime.Today)
                        .ToList();
                    Profile.UpcomingReservations = upcomingList.Count;
                    System.Diagnostics.Debug.WriteLine($"Upcoming: {Profile.UpcomingReservations}");

                    System.Diagnostics.Debug.WriteLine($"=== END DEBUG ===\n");

                    // Forçar atualização da UI
                    OnPropertyChanged(nameof(Profile));
                }
                else
                {
                    Profile.TotalReservations = 0;
                    Profile.CompletedStays = 0;
                    Profile.UpcomingReservations = 0;

                    System.Diagnostics.Debug.WriteLine(" Failed to load reservations for statistics");
                }

                // Preenche campos de edição
                FullName = Profile.FullName;
                PhoneNumber = Profile.PhoneNumber;
                IdentificationDocument = Profile.IdentificationDocument;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($" LoadProfile Exception: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void ToggleEdit()
        {
            IsEditing = !IsEditing;

            if (!IsEditing && Profile != null)
            {
                // Cancelar - restaurar valores originais
                FullName = Profile.FullName;
                PhoneNumber = Profile.PhoneNumber;
                IdentificationDocument = Profile.IdentificationDocument;
            }
        }

        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                await Shell.Current.DisplayAlert("Validation Error", "Full name is required", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                var request = new UpdateProfileRequest
                {
                    FullName = FullName.Trim(),
                    PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim(),
                    IdentificationDocument = string.IsNullOrWhiteSpace(IdentificationDocument) ? null : IdentificationDocument.Trim()
                };

                var response = await _userService.UpdateProfileAsync(request);

                if (response.Success && response.Data != null)
                {
                    // Atualiza os campos editáveis (preserva estatísticas)
                    if (Profile != null)
                    {
                        Profile.FullName = response.Data.FullName;
                        Profile.PhoneNumber = response.Data.PhoneNumber;
                        Profile.IdentificationDocument = response.Data.IdentificationDocument;
                        Profile.ProfilePictureUrl = response.Data.ProfilePictureUrl;

                        // Força a atualização da UI
                        OnPropertyChanged(nameof(Profile));
                    }

                    IsEditing = false;
                    await Shell.Current.DisplayAlert("Success", "Profile updated successfully", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", response.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to update profile: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ChangePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select profile photo"
                });

                if (result != null)
                {
                    IsBusy = true;

                    using var stream = await result.OpenReadAsync();
                    var uploadResponse = await _userService.UploadProfilePhotoAsync(stream, result.FileName);

                    if (uploadResponse.Success && uploadResponse.Data != null)
                    {
                        // Atualizar o ProfilePictureUrl diretamente
                        if (Profile != null)
                        {
                            // Extrair só o nome do ficheiro da URL retornada
                            var fileName = uploadResponse.Data.Replace("/images/profiles/", "").Split('?')[0];
                            Profile.ProfilePictureUrl = fileName;

                            // Forçar atualização da propriedade computada
                            OnPropertyChanged(nameof(Profile));
                        }

                        await Shell.Current.DisplayAlert("Success", "Photo updated successfully", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", uploadResponse.Message, "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to change photo: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task ViewReservationsAsync()
        {
            await Shell.Current.GoToAsync(nameof(ReservationsPage));
        }

        [RelayCommand]
        private async Task NavigateToChangePasswordAsync()
        {
            await Shell.Current.GoToAsync(nameof(ChangePasswordPage));
        }
    }
}
