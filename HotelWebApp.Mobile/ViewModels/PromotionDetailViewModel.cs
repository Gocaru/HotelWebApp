using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelWebApp.Mobile.Models;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.ViewModels
{
    [QueryProperty(nameof(PromotionId), "PromotionId")]
    public partial class PromotionDetailViewModel : ObservableObject
    {
        private readonly PromotionService _promotionService;

        [ObservableProperty]
        private PromotionDto? promotion;

        [ObservableProperty]
        private int promotionId;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public PromotionDetailViewModel(PromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        public async Task InitializeAsync()
        {
            await LoadPromotionDetailsAsync();
        }

        partial void OnPromotionIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadPromotionDetailsAsync();
            }
        }

        [RelayCommand]
        private async Task LoadPromotionDetailsAsync()
        {
            if (IsBusy || PromotionId <= 0) return;

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var result = await _promotionService.GetPromotionByIdAsync(PromotionId);

                if (result.Success && result.Data != null)
                {
                    Promotion = result.Data;
                }
                else
                {
                    ErrorMessage = result.Message ?? "Failed to load promotion details";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
