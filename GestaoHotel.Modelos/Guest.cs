using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HotelManagement.Core
{
    public class Guest
    {
        public int GuestId { get; set; }  //Identificador único(PK)
        public string Name { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdentificationDocument { get; set; } = string.Empty;
    }
}
