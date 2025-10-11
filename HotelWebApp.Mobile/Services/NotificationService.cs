using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Services
{
    public partial class NotificationService : INotificationService
    {
        private const string CHANNEL_ID = "hotel_notifications";
        private const string CHANNEL_NAME = "Hotel Notifications";

        public NotificationService()
        {
            InitializePlatform();
        }

        // Método que cada plataforma implementa
        partial void InitializePlatform();

        // Implementações comuns (com lógica de negócio)
        public async Task ShowReservationConfirmedAsync(int reservationId, string roomNumber, DateTime checkInDate)
        {
            var title = "Reservation Confirmed";
            var message = $"Your reservation for Room {roomNumber} is confirmed!\nCheck-in: {checkInDate:dd MMM yyyy}";
            var notificationId = 1000 + reservationId;

            await ShowNotificationAsync(title, message, notificationId);
            System.Diagnostics.Debug.WriteLine($"Notification sent: Reservation confirmed #{reservationId}");
        }

        public async Task ShowCheckInSuccessAsync(string roomNumber)
        {
            var title = "Check-in Successful";
            var message = $"Welcome! You're checked in to Room {roomNumber}. Enjoy your stay!";
            var notificationId = new Random().Next(10000, 99999);

            await ShowNotificationAsync(title, message, notificationId);
            System.Diagnostics.Debug.WriteLine($"Notification sent: Check-in success");
        }

        public async Task ShowPaymentSuccessAsync(decimal amount, string transactionId)
        {
            var title = "Payment Successful";
            var message = $"Payment of €{amount:N2} processed successfully.\nTransaction: {transactionId}";
            var notificationId = new Random().Next(10000, 99999);

            await ShowNotificationAsync(title, message, notificationId);
            System.Diagnostics.Debug.WriteLine($"Notification sent: Payment success");
        }

        public async Task ShowActivityBookedAsync(string activityName, DateTime scheduledDate)
        {
            var title = "Activity Booked";
            var message = $"{activityName} booked for {scheduledDate:dd MMM yyyy HH:mm}";
            var notificationId = new Random().Next(10000, 99999);

            await ShowNotificationAsync(title, message, notificationId);
            System.Diagnostics.Debug.WriteLine($"Notification sent: Activity booked");
        }

        public async Task ScheduleCheckInReminderAsync(int reservationId, string roomNumber, DateTime checkInDate)
        {
            var reminderDate = checkInDate.Date.AddDays(-1).AddHours(10);

            if (reminderDate <= DateTime.Now)
            {
                System.Diagnostics.Debug.WriteLine($"Check-in reminder not scheduled (date in past): {reminderDate}");
                return;
            }

            var title = "⏰ Check-in Reminder";
            var message = $"Your check-in at Room {roomNumber} is tomorrow at {checkInDate:HH:mm}. Don't forget!";
            var notificationId = 2000 + reservationId;

            // Por agora, mostra imediatamente (scheduling virá depois)
            await ShowNotificationAsync(title, message, notificationId);
            System.Diagnostics.Debug.WriteLine($"Check-in reminder scheduled for {reminderDate:dd MMM yyyy HH:mm}");
        }

        // Métodos que cada plataforma implementa (declarados como partial)
        public partial Task<bool> RequestPermissionsAsync();
        public partial Task ShowNotificationAsync(string title, string message, int notificationId);
        public partial Task CancelNotificationAsync(int notificationId);
    }
}
