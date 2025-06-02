using HotelDB_WPF_Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelDB_WPF_Framework.Helpers
{
    /// <summary>
    /// Classe auxiliar com métodos para verificar disponibilidade de reservas.
    /// </summary>
    public static class BookingHelper
    {
        /// <summary>
        /// Verifica se um quarto está disponível entre duas datas, ignorando uma reserva específica.
        /// </summary>
        /// <param name="roomId">ID do quarto a verificar.</param>
        /// <param name="checkIn">Data de entrada desejada.</param>
        /// <param name="checkOut">Data de saída desejada.</param>
        /// <param name="reservasExistentes">Lista de todas as reservas existentes.</param>
        /// <param name="reservaIgnoradaId">ID da reserva a ignorar (útil na edição).</param>
        public static bool EstaDisponivel(int roomId, DateTime checkIn, DateTime checkOut, List<Booking> reservasExistentes, int? reservaIgnoradaId = null)
        {
            return !reservasExistentes.Any(r =>
                r.RoomId == roomId &&
                r.BookingId != reservaIgnoradaId &&
                r.Status != Enums.BookingStatus.Cancelled &&
                r.CheckInDate < checkOut &&
                r.CheckOutDate > checkIn);
        }
    }
}
