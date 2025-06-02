using HotelManagement.Core;

namespace HotelManagementSystem.Logic
{
    /// <summary>
    /// Classe auxiliar para operações relacionadas com o histórico de estadias e reservas de hóspedes.
    /// </summary>
    public static class GuestHistoryHelper
    {
        /// <summary>
        /// Obtém a lista de hóspedes que têm reservas ativas.
        /// Uma reserva é considerada ativa se a data de check-out for posterior à data atual
        /// e o estado da reserva não for Cancelado.
        /// </summary>
        /// <param name="allBookings">Lista de todas as reservas.</param>
        /// <param name="allGuests">Lista de todos os hóspedes.</param>
        /// <returns>Lista de hóspedes com reservas ativas.</returns>
        public static List<Guest> GetGuestsWithActiveBookings(List<Booking> allBookings, List<Guest> allGuests)
        {
            var activeGuestIds = allBookings
                .Where(b => b.CheckOutDate >= DateTime.Now && b.Status != BookingStatus.Cancelled)
                .Select(b => b.GuestId)
                .Distinct()
                .ToList();

            return allGuests
                .Where(g => activeGuestIds.Contains(g.GuestId))
                .ToList();
        }

        /// <summary>
        /// Obtém o histórico de estadias de um hóspede específico.
        /// São consideradas apenas as reservas cujo check-out já ocorreu.
        /// </summary>
        /// <param name="guestId">Identificador do hóspede.</param>
        /// <param name="allBookings">Lista de todas as reservas.</param>
        /// <returns>Lista de reservas do histórico do hóspede, ordenadas por data de check-in decrescente.</returns>
        public static List<Booking> GetBookingHistoryForGuest(int guestId, List<Booking> allBookings)
        {
            return allBookings
                .Where(b => b.GuestId == guestId && b.CheckOutDate < DateTime.Now)
                .OrderByDescending(b => b.CheckInDate)
                .ToList();
        }

        /// <summary>
        /// Obtém a lista de hóspedes que têm pelo menos uma estadia no histórico (já concluída).
        /// </summary>
        /// <param name="allBookings">Lista de todas as reservas.</param>
        /// <param name="allGuests">Lista de todos os hóspedes.</param>
        /// <returns>Lista de hóspedes com estadias passadas.</returns>
        public static List<Guest> GetGuestsWithStayHistory(List<Booking> allBookings, List<Guest> allGuests)
        {
            var guestIdsWithHistory = allBookings
                .Where(b => b.CheckOutDate < DateTime.Now)
                .Select(b => b.GuestId)
                .Distinct()
                .ToList();

            return allGuests
                .Where(g => guestIdsWithHistory.Contains(g.GuestId))
                .ToList();
        }
    }
}

