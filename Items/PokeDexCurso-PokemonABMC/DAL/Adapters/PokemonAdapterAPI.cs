
using DAL.Adapters;
using DAL.Models;
using Newtonsoft.Json;

namespace DAL.Adapters
{
    public class PokemonAdapterAPI
    {
        public PokemonAdapterAPI()
        {

        }
        public List<Pokemon> GetAllPokemon()
        {
            GeneralAdapterAPIRest consult = new();
            string result = consult.GetFromAPI("api/Pokemon/ObtenerPokemones");

            if (result.Trim() != "" && result != "ERROR") return  JsonConvert.DeserializeObject<List<Pokemon>>(result) ?? new();
            else return new();
        }
        public Pokemon PostPokemon(DAL.Models.Pokemon postPokemon)
        {
            GeneralAdapterAPIRest consult = new();
            string result = consult.PostSyncAPI("api/Pokemon/CrearPokemon", postPokemon);

            if (result.Trim() != "" && result != "ERROR" )
            {
                if (result == "OK") return postPokemon;
                else return JsonConvert.DeserializeObject<Pokemon>(result) ?? new() { id = -1 };
            }
            else return new() { id = -1};
        }
        public bool PutPokemon(DAL.Models.Pokemon putPokemon)
        {
            GeneralAdapterAPIRest consult = new();
            string result = consult.PutSyncAPI("api/Pokemon/ActualizarPokemon", putPokemon);

            if (result.Trim() != "" && result == "OK") return true;
            else return false;
        }
        public bool DeletePokemon(int idPokemon)
        {
            GeneralAdapterAPIRest consult = new();
            string result = consult.DeleteSyncAPI("/api/Pokemon/EliminarPokemon/"+ idPokemon);

            if (result.Trim() != "" && result == "OK") return true;
            else return false;
        }
    }
}