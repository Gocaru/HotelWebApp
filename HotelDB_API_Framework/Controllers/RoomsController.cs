using HotelDB_API_Framework.Services;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HotelDB_API_Framework.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos quartos.
    /// Expõe endpoints da API para consultar, criar, atualizar e eliminar quartos,
    /// delegando as regras de negócio ao <see cref="RoomService"/>.
    /// </summary>
    public class RoomsController : ApiController
    {
        private HotelDBDataClassesDataContext dc;

        private RoomService service;

        /// <summary>
        /// Construtor do controlador de quartos.
        /// Inicializa a ligação à base de dados e o serviço de quartos responsável pela aplicação das regras de negócio.
        /// </summary>
        public RoomsController()
        {
            dc = new HotelDBDataClassesDataContext(ConfigurationManager.ConnectionStrings["HotelDBConnectionString"].ConnectionString);

            service = new RoomService(dc);
        }

        /// <summary>
        /// Obtém a lista de todos os quartos.
        /// </summary>
        /// <returns>Lista de quartos ordenada pelo número.</returns>
        public List<Room> Get()
        {
            var lista = from r in dc.Rooms orderby r.Number select r;
            return lista.ToList();
        }

        /// <summary>
        /// Obtém um quarto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do quarto.</param>
        /// <returns>Quarto correspondente ou mensagem de erro se não existir.</returns>
        public IHttpActionResult Get(int id)
        {
            var room = dc.Rooms.SingleOrDefault(r => r.RoomId == id);
            if (room != null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK, room));
            }

            return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Room not found"));
        }

        /// <summary>
        /// Adiciona um novo quarto.
        /// </summary>
        /// <param name="newRoom">Objeto quarto a adicionar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Post([FromBody] Room newRoom)
        {
            if (newRoom.RoomId != 0)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest, "Room ID must not be set by client"));
            }

            if (!service.IsRoomNumberUnique(newRoom.Number))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Room number already in use"));
            }

            dc.Rooms.InsertOnSubmit(newRoom);

            try
            {
                dc.SubmitChanges();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (System.Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }

        }

        /// <summary>
        /// Atualiza os dados de um quarto existente.
        /// </summary>
        /// <param name="updatedRoom">Objeto quarto com os dados atualizados.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Put([FromBody] Room updatedRoom)
        {
            var room = dc.Rooms.SingleOrDefault(r => r.RoomId == updatedRoom.RoomId);
            if (room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Room not found"));
            }

            if (!service.IsRoomNumberUnique(updatedRoom.Number, updatedRoom.RoomId))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.Conflict, "Room number already in use by another room"));
            }

            room.Number = updatedRoom.Number;
            room.Type = updatedRoom.Type;
            room.Capacity = updatedRoom.Capacity;
            room.Status = updatedRoom.Status;
            room.PricePerNight = updatedRoom.PricePerNight;

            try
            {
                dc.SubmitChanges();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (System.Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }
        }

        /// <summary>
        /// Elimina um quarto pelo seu ID.
        /// </summary>
        /// <param name="id">ID do quarto a eliminar.</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        public IHttpActionResult Delete(int id)
        {
            var room = dc.Rooms.SingleOrDefault(r => r.RoomId == id);
            if (room == null)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.NotFound, "Room not found"));
            }

            if (!service.CanRoomBeDeleted(room))
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.BadRequest,
                    "Room cannot be deleted. It is either not available or has active bookings."));
            }

            dc.Rooms.DeleteOnSubmit(room);

            try
            {
                dc.SubmitChanges();
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.OK));
            }
            catch (System.Exception e)
            {
                return ResponseMessage(Request.CreateResponse(HttpStatusCode.ServiceUnavailable, e));
            }
        }
    }
}
