using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;
using HotelWebApp.Mobile.Views;
using System.Collections.ObjectModel;
using System.Linq;

namespace HotelWebApp.Mobile.ViewModels
{
    public partial class PromotionsViewModel : ObservableObject
    {
        private readonly PromotionService _promotionService;

        [ObservableProperty]
        private ObservableCollection<PromotionDto> promotions = new();

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool hasPromotions;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public PromotionsViewModel(PromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public async Task InitializeAsync()
        {
            await LoadPromotionsAsync();
        }

        [RelayCommand]
        private async Task LoadPromotionsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            IsRefreshing = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _promotionService.GetPromotionsAsync();

                if (result.Success && result.Data != null)
                {
                    System.Diagnostics.Debug.WriteLine($"🔵 Processing {result.Data.Count} promotions");
                    System.Diagnostics.Debug.WriteLine($"📅 Today: {DateTime.Today:yyyy-MM-dd}");

                    Promotions.Clear();

                    foreach (var promotion in result.Data.OrderBy(p => p.EndDate))
                    {
                        System.Diagnostics.Debug.WriteLine($"\n--- {promotion.Title} ---");
                        System.Diagnostics.Debug.WriteLine($"  StartDate: {promotion.StartDate:yyyy-MM-dd}");
                        System.Diagnostics.Debug.WriteLine($"  EndDate: {promotion.EndDate:yyyy-MM-dd}");
                        System.Diagnostics.Debug.WriteLine($"  IsActive (from API): {promotion.IsActive}");
                        System.Diagnostics.Debug.WriteLine($"  IsExpired: {promotion.IsExpired}");
                        System.Diagnostics.Debug.WriteLine($"  IsUpcoming: {promotion.IsUpcoming}");
                        System.Diagnostics.Debug.WriteLine($"  IsCurrentlyActive: {promotion.IsCurrentlyActive}");
                        System.Diagnostics.Debug.WriteLine($"  StatusText: {promotion.StatusText}");

                        Promotions.Add(promotion);
                    }

                    HasPromotions = Promotions.Any();

                    System.Diagnostics.Debug.WriteLine($"\n✅ HasPromotions: {HasPromotions}, Count: {Promotions.Count}");
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load promotions";
                    HasPromotions = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                HasPromotions = false;
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

    }
}
