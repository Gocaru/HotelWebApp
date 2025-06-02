using HotelDB_API_Framework.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace HotelDB_API_Framework.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão de serviços extra.
    /// Permite consultar, adicionar, atualizar e remover serviços associados a reservas.
    /// </summary>
    public class ExtraServicesController : ApiController
    {
        private HotelDBDataClassesDataContext dc;
        private ExtraServiceService service;

        /// <summary>
        /// Construtor do controlador de serviços extra.
        /// Inicializa o DataContext.
        /// </summary>
        public ExtraServicesController()
        {
            dc = new HotelDBDataClassesDataContext(ConfigurationManager.ConnectionStrings["HotelDBConnectionString"].ConnectionString);
            service = new ExtraServiceService(dc);
        }

        /// <summary>
        /// Obtém a lista de todos os serviços extra.
        /// </summary>
        /// <returns>Lista de serviços ordenada por ID.</returns>
        public List<ExtraService> Get()
        {
            var lista = from es in dc.ExtraServices orderby es.ExtraServiceId select es;
            return lista.ToList();
        }

        /// <summary>
        /// Obtém um serviço extra pelo seu ID.
        /// </summary>
        /// <param name="id">ID do serviço extra.</param>
        /// <returns>Serviço correspondente ou mensagem de erro.</returns>
        public IHttpActionResult Get(int id)
        {
            var extra = dc.ExtraServices.SingleOrDefault(es => es.ExtraServiceId == id);
            if (extra != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, extra));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Extra service not found"));
        }

        /// <summary>
        /// Adiciona um novo serviço extra.
        /// </summary>
        /// <param name="newExtra">Objeto do serviço extra a adicionar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Post([FromBody] ExtraService newExtra)
        {
            if (newExtra.ExtraServiceId != 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "ExtraService ID must not be set by client"));
            }

            var booking = dc.Bookings.SingleOrDefault(b => b.BookingId == newExtra.BookingId);
            if (booking == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Associated booking not found"));
            }

            if (!service.IsValidBooking(newExtra.BookingId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Associated booking not found"));
            }

            dc.ExtraServices.InsertOnSubmit(newExtra);

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
        /// Atualiza um serviço extra existente.
        /// </summary>
        /// <param name="updatedExtra">Objeto com os dados atualizados.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Put([FromBody] ExtraService updatedExtra)
        {
            var extra = dc.ExtraServices.SingleOrDefault(es => es.ExtraServiceId == updatedExtra.ExtraServiceId);
            if (extra == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Extra service not found"));
            }

            var booking = dc.Bookings.SingleOrDefault(b => b.BookingId == updatedExtra.BookingId);
            if (booking == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Associated booking not found"));
            }

            if (!service.IsValidServiceType(updatedExtra.ServiceType))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid service type"));
            }

            extra.BookingId = updatedExtra.BookingId;
            extra.ServiceType = updatedExtra.ServiceType;
            extra.Price = updatedExtra.Price;

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
        /// Elimina um serviço extra pelo seu ID.
        /// </summary>
        /// <param name="id">ID do serviço extra a eliminar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Delete(int id)
        {
            var extra = dc.ExtraServices.SingleOrDefault(es => es.ExtraServiceId == id);
            if (extra == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Extra service not found"));
            }

            dc.ExtraServices.DeleteOnSubmit(extra);

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
