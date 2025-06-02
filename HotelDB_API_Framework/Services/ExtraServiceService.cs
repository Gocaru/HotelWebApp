using HotelDB_API_Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelDB_API_Framework.Services
{
    /// <summary>
    /// Serviço responsável por aplicar regras de negócio relacionadas com serviços extra.
    /// </summary>
    public class ExtraServiceService
    {
        private HotelDBDataClassesDataContext _dc;

        /// <summary>
        /// Construtor do serviço de serviços extra.
        /// </summary>
        /// <param name="dc">Instância do DataContext usada para acesso à base de dados.</param>
        public ExtraServiceService(HotelDBDataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Verifica se o tipo de serviço fornecido é válido segundo o enum <see cref="ExtraServiceType"/>.
        /// </summary>
        /// <param name="type">Valor inteiro a validar.</param>
        /// <returns>Verdadeiro se corresponder a um valor definido no enum, falso caso contrário.</returns>
        public bool IsValidServiceType(int type)
        {
            return Enum.IsDefined(typeof(ExtraServiceType), type);
        }

        /// <summary>
        /// Verifica se a reserva associada ao serviço extra existe.
        /// </summary>
        /// <param name="bookingId">ID da reserva a verificar.</param>
        /// <returns>Verdadeiro se a reserva existir, falso caso contrário.</returns>
        public bool IsValidBooking(int bookingId)
        {
            return _dc.Bookings.Any(b => b.BookingId == bookingId);
        }

        /// <summary>
        /// Verifica se já existe um serviço extra com o ID fornecido.
        /// </summary>
        /// <param name="extraServiceId">ID do serviço extra a verificar.</param>
        /// <returns>Verdadeiro se já existir, falso caso contrário.</returns>
        public bool ExistsById(int extraServiceId)
        {
            return _dc.ExtraServices.Any(es => es.ExtraServiceId == extraServiceId);
        }

        /// <summary>
        /// Obtém o total dos serviços extra associados a uma reserva.
        /// </summary>
        /// <param name="bookingId">ID da reserva.</param>
        /// <returns>Total em euros dos serviços extra.</returns>
        public decimal GetTotalForBooking(int bookingId)
        {
            return _dc.ExtraServices
                .Where(es => es.BookingId == bookingId)
                .Sum(es => es.Price);
        }
    }
}