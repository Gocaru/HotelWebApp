using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelWebApp.Mobile.Helpers
{
    public static class Constants
    {
        // Para desenvolvimento local com dispositivo USB
        public const string ApiBaseUrl = "http://192.168.1.165:5113/";

        // Quando publicar, alterar para o URL de produção
        // public const string ApiBaseUrl = "https://seudominio.com/api";
    }
}
