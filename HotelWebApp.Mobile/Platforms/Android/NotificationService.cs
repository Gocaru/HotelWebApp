using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using HotelWebApp.Mobile.Services;

namespace HotelWebApp.Mobile.Services
{
    public partial class NotificationService
    {
        private NotificationManager? _notificationManager;
        private int _notificationId = 1000;

        partial void InitializePlatform()
        {
            CreateNotificationChannel();
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var context = Android.App.Application.Context;
                _notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService)!;

                var channel = new NotificationChannel(
                    CHANNEL_ID,
                    CHANNEL_NAME,
                    NotificationImportance.High)
                {
                    Description = "Notifications for hotel reservations, check-ins, and activities"
                };

                channel.EnableVibration(true);
                channel.SetVibrationPattern(new long[] { 0, 500, 250, 500 });
                channel.EnableLights(true);

                _notificationManager.CreateNotificationChannel(channel);
                System.Diagnostics.Debug.WriteLine("✅ Android Notification Channel Created");
            }
        }

        public partial async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu) // Android 13+
                {
                    var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();

                    if (status != PermissionStatus.Granted)
                    {
                        status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                    }

                    var granted = status == PermissionStatus.Granted;
                    System.Diagnostics.Debug.WriteLine($"Notification Permission: {(granted ? "✅ Granted" : "❌ Denied")}");
                    return granted;
                }

                // Android 12 e inferior não precisa de permissão runtime
                return true;
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

                var context = Android.App.Application.Context;
                _notificationManager ??= (NotificationManager)context.GetSystemService(Context.NotificationService)!;

                // Intent para abrir a app quando clicar na notificação
                var intent = context.PackageManager?.GetLaunchIntentForPackage(context.PackageName!);
                if (intent != null)
                {
                    intent.SetFlags(ActivityFlags.SingleTop | ActivityFlags.ClearTop);
                }

                var pendingIntent = PendingIntent.GetActivity(
                    context,
                    notificationId,
                    intent,
                    PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

                // Construir notificação
                var builder = new NotificationCompat.Builder(context, CHANNEL_ID)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetSmallIcon(HotelWebApp.Mobile.Resource.Drawable.notification_icon)
                    .SetAutoCancel(true)
                    .SetPriority(NotificationCompat.PriorityHigh)
                    .SetContentIntent(pendingIntent)
                    .SetVibrate(new long[] { 0, 500, 250, 500 });

                // Se mensagem for longa, usar BigTextStyle
                if (message.Length > 40)
                {
                    builder.SetStyle(new NotificationCompat.BigTextStyle().BigText(message));
                }

                _notificationManager.Notify(notificationId, builder.Build());

                System.Diagnostics.Debug.WriteLine($"Android Notification Sent: {title} (ID: {notificationId})");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error showing notification: {ex.Message}");
            }
        }

        public partial async Task CancelNotificationAsync(int notificationId)
        {
            try
            {
                var context = Android.App.Application.Context;
                _notificationManager ??= (NotificationManager)context.GetSystemService(Context.NotificationService)!;

                _notificationManager.Cancel(notificationId);

                System.Diagnostics.Debug.WriteLine($"✅ Android Notification Cancelled (ID: {notificationId})");

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error cancelling notification: {ex.Message}");
            }
        }
    }
}