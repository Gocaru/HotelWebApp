using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Services
{
    public interface INotificationService
    {
        Task<bool> RequestPermissionsAsync();
        Task ShowNotificationAsync(string title, string message, int notificationId = 0);
        Task ShowReservationConfirmedAsync(int reservationId, string roomNumber, DateTime checkInDate);
        Task ShowCheckInSuccessAsync(string roomNumber);
        Task ShowPaymentSuccessAsync(decimal amount, string transactionId);
        Task ShowActivityBookedAsync(string activityName, DateTime scheduledDate);
        Task ScheduleCheckInReminderAsync(int reservationId, string roomNumber, DateTime checkInDate);
        Task CancelNotificationAsync(int notificationId);
    }
}
