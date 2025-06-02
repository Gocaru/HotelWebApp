using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelDB_WPF_Framework.Enums
{
    public enum BookingStatus
    {
        /// <summary>
        /// Reserva foi criada, mas o hóspede ainda não fez check-in
        /// </summary>
        Reserved = 0,

        /// <summary>
        /// O hóspede fez check-in
        /// </summary>
        CheckedIn = 1,

        /// <summary>
        /// // O hóspede saiu
        /// </summary>
        CheckedOut = 2,

        /// <summary>
        /// A reserva foi cancelada antes ou durante
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Reserva terminada com sucesso
        /// </summary>
        Completed = 4
    }
}
