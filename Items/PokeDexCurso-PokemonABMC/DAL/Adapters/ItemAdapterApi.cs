using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Adapters
{
    public class ItemAdapterApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:1984/api/Items/";
        // ⚠️ Cambia el puerto por el que use tu API

        public ItemAdapterApi()
        {
            _httpClient = new HttpClient();
        }

        // GET: api/Items/ObtenerItemsCompleto
        public async Task<List<Items>> ObtenerItemsCompleto()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "ObtenerItemsCompleto");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Items>>();
            return new List<Items>();
        }

        // GET: api/Items/ObtenerItemsActivo
        public async Task<List<Items>> ObtenerItemsActivo()
        {
            var response = await _httpClient.GetAsync(_baseUrl + "ObtenerItemsActivo");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<Items>>();
            return new List<Items>();
        }

        // GET: api/Items/ObtenerItemXid/{id}
        public async Task<Items> ObtenerItemXid(int id)
        {
            var response = await _httpClient.GetAsync(_baseUrl + $"ObtenerItemXid/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Items>();
            return null;
        }

        // POST: api/Items/CargarItem
        public async Task<Items> CargarItem(Items nuevoItem)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl + "CargarItem", nuevoItem);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Items>();
            return null;
        }

        // PUT: api/Items/ModificarItem
        public async Task<Items> ModificarItem(Items itemModificado)
        {
            var response = await _httpClient.PutAsJsonAsync(_baseUrl + "ModificarItem", itemModificado);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Items>();
            return null;
        }

        // DELETE: api/Items/DesactivarItem/{id}
        public async Task<Items> DesactivarItem(int id)
        {
            var response = await _httpClient.DeleteAsync(_baseUrl + $"DesactivarItem/{id}");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Items>();
            return null;
        }
    }
}
