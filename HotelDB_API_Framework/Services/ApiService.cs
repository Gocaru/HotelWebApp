using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HotelDB_API_Framework.Services
{
    /// <summary>
    /// Serviço responsável por comunicar com a Web API do sistema de gestão hoteleira.
    /// Fornece métodos genéricos para operações GET, POST, PUT e DELETE.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        /// <summary>
        /// Inicializa o ApiService com o endereço base da API.
        /// </summary>
        /// <param name="baseUrl">URL base da API (ex: https://localhost:44319/api/)</param>
        public ApiService(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/') + "/";
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Obtém todos os elementos de um endpoint.
        /// </summary>
        public async Task<List<T>> GetAllAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(_baseUrl + endpoint);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// Obtém um elemento por ID.
        /// </summary>
        public async Task<T> GetByIdAsync<T>(string endpoint, int id)
        {
            var response = await _httpClient.GetAsync(_baseUrl + endpoint + "/" + id);
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Envia um objeto para a API para criar (POST).
        /// </summary>
        public async Task<bool> PostAsync<T>(string endpoint, T item)
        {
            string json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl + endpoint, content);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Atualiza um objeto (PUT).
        /// </summary>
        public async Task<bool> PutAsync<T>(string endpoint, T item)
        {
            string json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(_baseUrl + endpoint, content);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Elimina um recurso pelo ID.
        /// </summary>
        public async Task<bool> DeleteAsync(string endpoint, int id)
        {
            var response = await _httpClient.DeleteAsync(_baseUrl + endpoint + "/" + id);
            return response.IsSuccessStatusCode;
        }
    }
}