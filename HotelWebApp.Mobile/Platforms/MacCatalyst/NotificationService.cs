using Foundation;
using UserNotifications;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.Services
{
    public partial class NotificationService
    {
        private UNUserNotificationCenter? _notificationCenter;
        private int _notificationId = 1000;

        partial void InitializePlatform()
        {
            _notificationCenter = UNUserNotificationCenter.Current;
            System.Diagnostics.Debug.WriteLine("✅ MacCatalyst Notification Center Initialized");
        }

        public partial async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                _notificationCenter ??= UNUserNotificationCenter.Current;

                var (granted, error) = await _notificationCenter.RequestAuthorizationAsync(
                    UNAuthorizationOptions.Alert |
                    UNAuthorizationOptions.Badge |
                    UNAuthorizationOptions.Sound);

                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ MacCatalyst Notification Permission Error: {error.LocalizedDescription}");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine($"MacCatalyst Notification Permission: {(granted ? "✅ Granted" : "❌ Denied")}");
                return granted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error requesting notification permissions: {ex.Message}");
                return false;
            }
        }

        public partial async Task ShowNotificationAsync(string title, string message, int notificationId)
        {
            try
            {
                if (notificationId == 0)
                {
                    notificationId = _notificationId++;
                }

                _notificationCenter ??= UNUserNotificationCenter.Current;

                // Criar conteúdo da notificação
                var content = new UNMutableNotificationContent
                {
                    Title = title,
                    Body = message,
                    Sound = UNNotificationSound.Default,
                    Badge = 1
                };

                // Trigger imediato (0.1 segundos)
                var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);

                // Criar request
                var request = UNNotificationRequest.FromIdentifier(
                    notificationId.ToString(),
                    content,
                    trigger);

                // Adicionar notificação
                await _notificationCenter.AddNotificationRequestAsync(request);

                System.Diagnostics.Debug.WriteLine($"✅ MacCatalyst Notification Sent: {title} (ID: {notificationId})");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error showing MacCatalyst notification: {ex.Message}");
            }
        }

        public partial async Task CancelNotificationAsync(int notificationId)
        {
            try
            {
                _notificationCenter ??= UNUserNotificationCenter.Current;

                var identifiers = new string[] { notificationId.ToString() };
                _notificationCenter.RemovePendingNotificationRequests(identifiers);
                _notificationCenter.RemoveDeliveredNotifications(identifiers);

                System.Diagnostics.Debug.WriteLine($"✅ MacCatalyst Notification Cancelled (ID: {notificationId})");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error cancelling MacCatalyst notification: {ex.Message}");
            }
        }
    }
}