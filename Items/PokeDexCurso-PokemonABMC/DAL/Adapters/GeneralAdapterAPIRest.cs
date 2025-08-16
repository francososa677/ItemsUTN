using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace DAL.Adapters
{
    public class GeneralAdapterAPIRest
    {
        public GeneralAdapterAPIRest()
        {

        }
        private string BaseAdress = "http://localhost:91/";
        private string EndPoint = "";
        private object? BodyRequest;
        private string? BodyResponse;
        private int HttpOption;

        /// <summary>
        /// Metodo para utilizar el metodo GET
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public string GetFromAPI(string endPoint)
        {
            HttpClient client = new()
            {
                //BaseAddress es donde publicamos o tenemos ejecutando la aplicacion.
                BaseAddress = new Uri(BaseAdress)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Endpoint que queremos consultar.
            HttpResponseMessage response = client.GetAsync(endPoint).Result;

            //Los codigos aceptables son los 200
            if (response.IsSuccessStatusCode) return response.Content.ReadAsStringAsync().Result;
            //Devuelvo error pero podemos agregar mas si asi lo deseamos
            else return "ERROR";
        }

        public string PutSyncAPI(string endPoint,object putObject)
        {
            HttpClient client = new()
            {
                //BaseAddress es donde publicamos o tenemos ejecutando la aplicacion.
                BaseAddress = new Uri(BaseAdress)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Endpoint que queremos consultar.
            HttpResponseMessage response = client.PutAsJsonAsync(endPoint, putObject).Result;

            if (response.IsSuccessStatusCode) return "OK";
            //Devuelvo error pero podemos agregar mas si asi lo deseamos
            else return "ERROR";
        }
        public string PostSyncAPI(string endPoint, object postObject)
        {
            HttpClient client = new()
            {
                //BaseAddress es donde publicamos o tenemos ejecutando la aplicacion.
                BaseAddress = new Uri(BaseAdress)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Endpoint que queremos consultar.
            HttpResponseMessage response = client.PostAsJsonAsync(endPoint, postObject).Result;

            if (response.IsSuccessStatusCode)
            {
                //Esto es un salvaguarda si la API no devuelve el objeto creado en el codigo
                if (response.StatusCode == HttpStatusCode.Created && !response.Content.Equals(""))
                    return response.Content.ReadAsStringAsync().Result;
                else return "OK";
            }
            //Devuelvo error pero podemos agregar mas si asi lo deseamos
            else return "ERROR";
        }
        public string DeleteSyncAPI(string endPoint)
        {
            HttpClient client = new()
            {
                //BaseAddress es donde publicamos o tenemos ejecutando la aplicacion.
                BaseAddress = new Uri(BaseAdress)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Endpoint que queremos consultar.
            HttpResponseMessage response = client.DeleteAsync(endPoint).Result;

            if (response.IsSuccessStatusCode) return "OK";
            //Devuelvo error pero podemos agregar mas si asi lo deseamos
            else return "ERROR";
        }
    }
}