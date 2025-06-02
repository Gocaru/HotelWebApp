using HotelDB_API_Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelDB_API_Framework.Services
{
    /// <summary>
    /// Serviço responsável por aplicar regras de negócio relacionadas com os quartos.
    /// </summary>
    public class RoomService
    {
        private HotelDBDataClassesDataContext _dc;

        /// <summary>
        /// Construtor do serviço de quartos.
        /// </summary>
        /// <param name="dc">Instância do DataContext usada para acesso à base de dados.</param>
        public RoomService(HotelDBDataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Verifica se um quarto pode ser eliminado.
        /// Só é possível eliminar quartos disponíveis e sem reservas associadas.
        /// </summary>
        /// <param name="room">Objeto quarto a validar.</param>
        /// <returns>Verdadeiro se puder ser eliminado, falso caso contrário.</returns>
        public bool CanRoomBeDeleted(Room room)
        {
            if ((RoomStatus)room.Status != RoomStatus.Available)
                return false;

            bool hasBookings = _dc.Bookings.Any(b => b.RoomId == room.RoomId);
            return !hasBookings;
        }

        /// <summary>
        /// Verifica se o número do quarto é único, excluindo opcionalmente um ID específico.
        /// </summary>
        /// <param name="number">Número do quarto a verificar.</param>
        /// <param name="excludeRoomId">ID do quarto a excluir da verificação (para edição).</param>
        /// <returns>Verdadeiro se o número for único, falso caso contrário.</returns>
        public bool IsRoomNumberUnique(int number, int? excludeRoomId = null)
        {
            return !_dc.Rooms.Any(r =>
                r.Number == number &&
                r.RoomId != excludeRoomId.GetValueOrDefault());
        }

        /// <summary>
        /// Verifica se o valor fornecido corresponde a um tipo de quarto válido.
        /// </summary>
        /// <param name="type">Valor numérico do tipo de quarto.</param>
        /// <returns>Verdadeiro se o tipo for válido, falso caso contrário.</returns>
        public bool IsValidRoomType(int type)
        {
            return Enum.IsDefined(typeof(RoomType), type);
        }

        /// <summary>
        /// Verifica se o quarto está atualmente ocupado (reserva com status CheckedIn).
        /// </summary>
        /// <param name="roomId">ID do quarto a verificar.</param>
        /// <returns>Verdadeiro se estiver ocupado, falso caso contrário.</returns>
        public bool IsRoomCurrentlyOccupied(int roomId)
        {
            return _dc.Bookings.Any(b =>
                b.RoomId == roomId &&
                (BookingStatus)b.Status == BookingStatus.CheckedIn);
        }

        /// <summary>
        /// Obtém o preço base sugerido para o tipo de quarto fornecido.
        /// (Regra opcional de apoio à definição de preços).
        /// </summary>
        /// <param name="type">Tipo de quarto.</param>
        /// <returns>Preço base sugerido.</returns>
        public decimal GetBasePriceForRoomType(RoomType type)
        {
            switch (type)
            {
                case RoomType.Standard: return 60;
                case RoomType.Suite: return 100;
                case RoomType.Luxury: return 150;
                default: return 0;
            }
        }

    }
}