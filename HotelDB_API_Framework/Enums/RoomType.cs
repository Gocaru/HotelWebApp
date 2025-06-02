using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelDB_API_Framework.Enums
{
    /// <summary>
    /// Representa os tipos possíveis de um quarto.
    /// </summary>
    public enum RoomType
    {

        /// <summary>O quarto é do tipo Standard.</summary>
        Standard = 0,

        /// <summary>O quarto é do tipo Suite.</summary>
        Suite = 1,

        /// <summary>O quarto é do tipo Luxury.</summary>
        Luxury = 2,

    }
}