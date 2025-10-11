using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.Services
{
    public partial class NotificationService
    {
        partial void InitializePlatform()
        {
            System.Diagnostics.Debug.WriteLine("⚠️ Windows Notifications not implemented");
        }

        public partial async Task<bool> RequestPermissionsAsync()
        {
            System.Diagnostics.Debug.WriteLine("⚠️ Windows Notifications not implemented");
            return await Task.FromResult(true); // Simular permissão concedida
        }

        public partial async Task ShowNotificationAsync(string title, string message, int notificationId)
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ [Windows Stub] Notification: {title} - {message}");
            await Task.CompletedTask;
        }

        public partial async Task CancelNotificationAsync(int notificationId)
        {
            System.Diagnostics.Debug.WriteLine($"⚠️ [Windows Stub] Cancel Notification ID: {notificationId}");
            await Task.CompletedTask;
        }
    }
}