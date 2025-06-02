using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelDB_API_Framework.Enums
{
    /// <summary>
    /// Representa os tipos possíveis de serviços extra.
    /// </summary>
    public enum ExtraServiceType
    {
        /// <summary>O serviço extra é o pequeno-almoço.</summary> 
        Breakfast = 0,

        /// <summary>O serviço extra é o acesso ao Spa.</summary>
        SpaAccess = 1,

        /// <summary>O serviço extra é o estacionamento no parque do Hotel.</summary>
        PrivateParking = 2,

        /// <summary>O serviço extra é o serviço de lavandaria.</summary>
        Laundry = 3,
    }
}