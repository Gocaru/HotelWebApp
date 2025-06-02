using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace HotelDB_WPF_Framework.Services
{
    /// <summary>
    /// Serviço responsável por verificar a conectividade de rede e disponibilidade da API.
    /// </summary>
    public class NetworkService
    {
        private readonly string _testUrl;

        /// <summary>
        /// Inicializa o serviço de rede com o URL a testar
        /// </summary>
        /// <param name="testUrl">Endpoint da API para verificação.</param>
        public NetworkService(string testUrl)
        {
            _testUrl = testUrl;
        }

        /// <summary>
        /// Verifica se a máquina está ligada à internet.
        /// </summary>
        /// <returns>Verdadeiro se houver ligação à Internet, falso caso contrário.</returns>
        public bool IsInternetAvailable()
        {
            try
            {
                return NetworkInterface.GetIsNetworkAvailable();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se a API está acessível via HTTP.
        /// </summary>
        /// <returns>Verdadeiro se a API responder com sucesso, falso caso contrário.</returns>
        public async Task<bool> IsApiAvailableAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(_testUrl);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}