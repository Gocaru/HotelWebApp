using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelDB_WPF_Framework.Enums
{
    public enum RoomStatus
    {
        /// <summary>O quarto está disponível para reserva.</summary>
        Available = 0,

        /// <summary>O quarto está reservado, mas ainda não ocupado.</summary>
        Reserved = 1,

        /// <summary>O quarto está atualmente ocupado por um hóspede.</summary>
        Occupied = 2,

        /// <summary>O quarto está em manutenção e indisponível.</summary>
        UnderMaintenance = 3
    }
}
