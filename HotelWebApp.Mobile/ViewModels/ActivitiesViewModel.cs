using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Collections.ObjectModel;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class ActivitiesViewModel : ObservableObject
    {
        private readonly ActivityService _activityService;

        [ObservableProperty]
        private ObservableCollection<ActivityDto> activities = new();

        [ObservableProperty]
        private bool isBusy;


        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool hasActivities;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public ActivitiesViewModel(ActivityService activityService)
        {
            _activityService = activityService;
        }

        public async Task InitializeAsync()
        {
            await LoadActivitiesAsync();
        }

        [RelayCommand]
        private async Task LoadActivitiesAsync()
        {
            System.Diagnostics.Debug.WriteLine("🔵 STARTING LoadActivitiesAsync");

            if (IsBusy)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ ALREADY BUSY");
                return;
            }

            IsBusy = true;
            IsRefreshing = true;
            ErrorMessage = string.Empty;
            System.Diagnostics.Debug.WriteLine("🔵 Making API call...");

            try
            {
                var result = await _activityService.GetActivitiesAsync();

                System.Diagnostics.Debug.WriteLine($"🔵 API Result - Success: {result.Success}");
                System.Diagnostics.Debug.WriteLine($"🔵 API Result - Data is null: {result.Data == null}");

                if (result.Success && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔵 Data count: {result.Data.Count}");

                    Activities.Clear();

                    // ✅ SEM FILTRO - API já retorna só ativas
                    foreach (var activity in result.Data)
                    {
                        System.Diagnostics.Debug.WriteLine($"🔵 Adding activity: {activity.Name} (IsActive={activity.IsActive})");
                        Activities.Add(activity);
                    }

                    HasActivities = Activities.Any();

                    System.Diagnostics.Debug.WriteLine($"✅ Final count: {Activities.Count}, HasActivities: {HasActivities}");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load activities";
                    HasActivities = false;

                    System.Diagnostics.Debug.WriteLine($"❌ Error: {ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                HasActivities = false;

                System.Diagnostics.Debug.WriteLine($"❌ Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"❌ StackTrace: {ex.StackTrace}");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
                System.Diagnostics.Debug.WriteLine("🔵 IsBusy = false");
            }
        }

        [RelayCommand]
        private async Task ViewActivityDetailsAsync(ActivityDto activity)
        {
            if (activity == null) return;

            await Shell.Current.GoToAsync(nameof(ActivityDetailPage), new Dictionary<string, object>
            {
                { "ActivityId", activity.ActivityId }
            });
        }
    }
}