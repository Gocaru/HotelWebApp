using System.Linq;

namespace HotelDB_API_Framework.Services
{
    /// <summary>
    /// Serviço responsável por aplicar regras de negócio relacionadas com os hóspedes.
    /// </summary>
    public class GuestService
    {
        private HotelDBDataClassesDataContext _dc;

        /// <summary>
        /// Construtor do serviço de hóspedes.
        /// </summary>
        /// <param name="dc">Instância do DataContext usada para acesso à base de dados.</param>
        public GuestService(HotelDBDataClassesDataContext dc)
        {
            _dc = dc;
        }

        /// <summary>
        /// Verifica se o hóspede tem reservas associadas.
        /// </summary>
        /// <param name="guestId">ID do hóspede a verificar.</param>
        /// <returns>Verdadeiro se existirem reservas, falso caso contrário.</returns>
        public bool HasBookings(int guestId)
        {
            return _dc.Bookings.Any(b => b.GuestId == guestId);
        }

        /// <summary>
        /// Verifica se o email fornecido é único no sistema.
        /// Exclui o hóspede com o ID indicado (caso esteja a editar os próprios dados).
        /// </summary>
        /// <param name="email">Email a verificar.</param>
        /// <param name="excludeGuestId">ID do hóspede a excluir da verificação (edição).</param>
        /// <returns>Verdadeiro se for único, falso caso contrário.</returns>
        public bool IsEmailUnique(string email, int? excludeGuestId = null)
        {
            return !_dc.Guests.Any(g =>
                g.Email == email &&                                 //Verifica se existe algum hóspede com o mesmo email
                g.GuestId != excludeGuestId.GetValueOrDefault());   //Ignora o próprio hóspede em caso de edição (para não comparar consigo mesmo)

        }

        /// <summary>
        /// Verifica se o documento de identificação fornecido é único no sistema.
        /// Exclui o próprio hóspede (em caso de edição).
        /// </summary>
        /// <param name="document">Documento de identificação (ex.: NIF, CC).</param>
        /// <param name="excludeGuestId">ID do hóspede a excluir da verificação (edição).</param>
        /// <returns>Verdadeiro se for único, falso caso contrário.</returns>
        public bool IsIdentificationUnique(string document, int? excludeGuestId = null)
        {
            return !_dc.Guests.Any(g =>
                g.IdentificationDocument == document &&
                g.GuestId != excludeGuestId.GetValueOrDefault());
        }
    }
}