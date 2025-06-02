using HotelDB_API_Framework.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HotelDB_API_Framework.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos hospedes.
    /// Expõe endpoints da API para consultar, criar, atualizar e eliminar hospedes,
    /// delegando as regras de negócio ao <see cref="GuestService"/>.
    /// </summary>
    public class GuestsController : ApiController
    {
        private HotelDBDataClassesDataContext dc;
        private GuestService service;


        /// <summary>
        /// Construtor do controlador de hóspedes
        /// Inicializa a ligação à base de dados e o serviço de hóspedes responsável pela aplicação das regras de negócio.
        /// </summary>
        public GuestsController()
        {
            dc = new HotelDBDataClassesDataContext(ConfigurationManager.ConnectionStrings["HotelDBConnectionString"].ConnectionString);
            service = new GuestService(dc);
        }

        /// <summary>
        /// Obtém a lista de todos os hóspedes.
        /// </summary>
        /// <returns>Lista de hóspedes ordenada por nome.</returns>
        public List<Guest> Get()
        {
            var lista = from g in dc.Guests orderby g.Name select g;
            return lista.ToList();
        }

        /// <summary>
        /// Obtém um hóspede pelo seu ID.
        /// </summary>
        /// <param name="id">ID do hóspede.</param>
        /// <returns>Hóspede correspondente ou mensagem de erro se não existir.</returns>
        public IHttpActionResult Get(int id)
        {
            var guest = dc.Guests.SingleOrDefault(g => g.GuestId == id);
            if (guest != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, guest));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found"));
        }

        /// <summary>
        /// Adiciona um novo hóspede.
        /// </summary>
        /// <param name="newGuest">Objeto hóspede a adicionar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Post([FromBody] Guest newGuest)
        {

            if (newGuest.GuestId != 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Guest ID must not be set by client"));
            }

            if (!service.IsEmailUnique(newGuest.Email))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Email already in use"));
            }

            if (!service.IsIdentificationUnique(newGuest.IdentificationDocument))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Identification document already in use"));
            }

            dc.Guests.InsertOnSubmit(newGuest);

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
        /// Atualiza os dados de um hóspede existente.
        /// </summary>
        /// <param name="updatedGuest">Objeto hóspede com os dados atualizados.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Put([FromBody] Guest updatedGuest)
        {
            var guest = dc.Guests.SingleOrDefault(g => g.GuestId == updatedGuest.GuestId);
            if (guest == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found"));
            }

            if (!service.IsEmailUnique(updatedGuest.Email, updatedGuest.GuestId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Email already in use by another guest"));
            }

            if (!service.IsIdentificationUnique(updatedGuest.IdentificationDocument, updatedGuest.GuestId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Identification document already in use by another guest"));
            }

            guest.Name = updatedGuest.Name;
            guest.Email = updatedGuest.Email;
            guest.Contact = updatedGuest.Contact;
            guest.IdentificationDocument = updatedGuest.IdentificationDocument;

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
        /// Elimina um hóspede pelo seu ID.
        /// </summary>
        /// <param name="id">ID do hóspede a eliminar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Delete(int id)
        {
            var guest = dc.Guests.SingleOrDefault(g => g.GuestId == id);
            if (guest == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Guest not found"));
            }

            if (service.HasBookings(id))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Guest has bookings and cannot be deleted"));
            }

            dc.Guests.DeleteOnSubmit(guest);

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
