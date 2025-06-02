using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using HotelDB_API_Framework.Services;

namespace HotelDB_API_Framework.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão das reservas.
    /// Expõe endpoints da API para consultar, criar, atualizar e eliminar reservas,
    /// delegando as regras de negócio ao <see cref="BookingService"/>.
    /// </summary>
    public class BookingsController : ApiController
    {
        private HotelDBDataClassesDataContext dc;

        private BookingService service;

        /// <summary>
        /// Construtor do controlador de reservas.
        /// Inicializa a ligação à base de dados e o serviço de reservas responsável pela aplicação das regras de negócio.
        /// </summary>
        public BookingsController()
        {
            dc = new HotelDBDataClassesDataContext(
             ConfigurationManager.ConnectionStrings["HotelDBConnectionString"].ConnectionString);

            service = new BookingService(dc);
        }

        /// <summary>
        /// Obtém a lista de todas as reservas.
        /// </summary>
        /// <returns>Lista de reservas ordenada por data de check-in.</returns>
        public List<Booking> Get()
        {
            var lista = from b in dc.Bookings orderby b.CheckInDate select b;
            return lista.ToList();
        }

        /// <summary>
        /// Obtém uma reserva pelo seu ID.
        /// </summary>
        /// <param name="id">ID da reserva.</param>
        /// <returns>Reserva correspondente ou mensagem de erro.</returns>
        public IHttpActionResult Get(int id)
        {
            var booking = dc.Bookings.SingleOrDefault(b => b.BookingId == id);
            if (booking != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, booking));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Booking not found"));
        }

        /// <summary>
        /// Adiciona uma nova reserva.
        /// </summary>
        /// <param name="newBooking">Objeto reserva a adicionar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Post([FromBody] Booking newBooking)
        {
            if (newBooking.BookingId != 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Booking ID must not be set by client"));
            }

            var guest = dc.Guests.SingleOrDefault(g => g.GuestId == newBooking.GuestId);
            if (guest == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found"));
            }

            var room = dc.Rooms.SingleOrDefault(r => r.RoomId == newBooking.RoomId);
            if (room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Room not found"));
            }

            if (service.HasOverlappingBooking(newBooking))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict,
                    "This room is already booked for the selected dates"));
            }

            if (newBooking.CheckInDate >= newBooking.CheckOutDate)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Check-out must be after check-in"));
            }

            newBooking.ReservationDate = DateTime.Now;

            dc.Bookings.InsertOnSubmit(newBooking);

            try
            {
                dc.SubmitChanges();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }
        }

        /// <summary>
        /// Atualiza uma reserva existente.
        /// </summary>
        /// <param name="updatedBooking">Objeto reserva com dados atualizados.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Put([FromBody] Booking updatedBooking)
        {
            var booking = dc.Bookings.SingleOrDefault(b => b.BookingId == updatedBooking.BookingId);
            if (booking == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Booking not found"));
            }

            if (!service.IsValidGuest(updatedBooking.GuestId))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found"));

            if (!service.IsValidRoom(updatedBooking.RoomId))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Room not found"));

            if (!updatedBooking.CheckInDate.HasValue || !updatedBooking.CheckOutDate.HasValue)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Check-in and check-out dates are required"));
            }

            if (!service.IsDateRangeValid(updatedBooking.CheckInDate.Value, updatedBooking.CheckOutDate.Value))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Check-out must be after check-in"));
            }

            if (service.HasOverlappingBooking(updatedBooking))
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Room is already booked in this period"));

            booking.GuestId = updatedBooking.GuestId;
            booking.RoomId = updatedBooking.RoomId;
            booking.CheckInDate = updatedBooking.CheckInDate;
            booking.CheckOutDate = updatedBooking.CheckOutDate;
            booking.NumberOfGuests = updatedBooking.NumberOfGuests;
            booking.Status = updatedBooking.Status;

            try
            {
                dc.SubmitChanges();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }
        }

        /// <summary>
        /// Elimina uma reserva pelo seu ID.
        /// </summary>
        /// <param name="id">ID da reserva a eliminar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Delete(int id)
        {
            var booking = dc.Bookings.SingleOrDefault(b => b.BookingId == id);
            if (booking == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Booking not found"));
            }

            dc.Bookings.DeleteOnSubmit(booking);

            try
            {
                dc.SubmitChanges();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }
        }
    }
}
