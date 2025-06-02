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
    /// Controlador responsável pela gestão das faturas.
    /// Aplica regras de negócio através do <see cref="InvoiceService"/>.
    /// </summary>
    public class InvoicesController : ApiController
    {
        private HotelDBDataClassesDataContext dc;
        private InvoiceService service;

        /// <summary>
        /// Construtor do controlador de faturas.
        /// Inicializa o DataContext e o serviço associado.
        /// </summary>
        public InvoicesController()
        {
            dc = new HotelDBDataClassesDataContext(ConfigurationManager.ConnectionStrings["HotelDBConnectionString"].ConnectionString);
            service = new InvoiceService(dc);
        }

        /// <summary>
        /// Obtém a lista de todas as faturas.
        /// </summary>
        /// <returns>Lista de faturas ordenada pela data de emissão.</returns>
        public List<Invoice> Get()
        {
            var lista = from i in dc.Invoices orderby i.IssueDate descending select i;
            return lista.ToList();
        }

        /// <summary>
        /// Obtém uma fatura pelo seu ID.
        /// </summary>
        /// <param name="id">ID da fatura.</param>
        /// <returns>Fatura correspondente ou mensagem de erro.</returns>
        public IHttpActionResult Get(int id)
        {
            var invoice = dc.Invoices.SingleOrDefault(i => i.InvoiceId == id);
            if (invoice != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, invoice));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Invoice not found"));
        }

        /// <summary>
        /// Adiciona uma nova fatura.
        /// </summary>
        /// <param name="newInvoice">Objeto fatura a adicionar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Post([FromBody] Invoice newInvoice)
        {
            if (newInvoice.InvoiceId != 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Invoice ID must not be set by client"));
            }

            if (!service.IsValidBooking(newInvoice.BookingId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Associated booking not found"));
            }

            if (service.IsBookingAlreadyInvoiced(newInvoice.BookingId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Invoice already exists for this booking"));
            }

            service.CalculateTotals(newInvoice);

            dc.Invoices.InsertOnSubmit(newInvoice);

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
        /// Atualiza os dados de uma fatura existente.
        /// </summary>
        /// <param name="updatedInvoice">Fatura com os dados atualizados.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Put([FromBody] Invoice updatedInvoice)
        {
            var invoice = dc.Invoices.SingleOrDefault(i => i.InvoiceId == updatedInvoice.InvoiceId);
            if (invoice == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Invoice not found"));
            }

            if (!service.IsValidBooking(updatedInvoice.BookingId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Associated booking not found"));
            }

            invoice.BookingId = updatedInvoice.BookingId;
            invoice.PaymentMethod = updatedInvoice.PaymentMethod;

            service.CalculateTotals(invoice);

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
        /// Elimina uma fatura pelo seu ID.
        /// </summary>
        /// <param name="id">ID da fatura a eliminar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Delete(int id)
        {
            var invoice = dc.Invoices.SingleOrDefault(i => i.InvoiceId == id);
            if (invoice == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Invoice not found"));
            }

            dc.Invoices.DeleteOnSubmit(invoice);

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
