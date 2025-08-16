using DAL.Adapters;
using PokeDex.Entities;

namespace PokeDex.Entities
{
    public class Pokemon
    {
        public static int GetLastEntryId()
        {
            if (AllPokemons == null) return 1;
            else return AllPokemons.Last().Id;
        }
        private static List<Pokemon>? AllPokemons;
        public int Id { get; set; }
        public string Nombre { get; set; }
        public float Altura { get; set; }
        public float Peso { get; set; }
        public int Generacion { get; set; }
        public Tipo TipoPrimario { get; set; }
        public Tipo? TipoSecundario { get; set; }
        public Pokemon(int id, string nombre, float altura, float peso, int generacion, Tipo tipoPrimario, Tipo? tipoSecundario)
        {
            Id = id;
            Nombre = nombre;
            Altura = altura;
            Peso = peso;
            Generacion = generacion;
            TipoPrimario = tipoPrimario;
            TipoSecundario = tipoSecundario;
        }
        public static List<Pokemon> GetAll()
        {
            if(AllPokemons == null)
            {
                List<Tipo> allTypes = Tipo.GetAllTypes();

                PokemonAdapterAPI consultor = new();

                //Recuperamos el listado original
                List<DAL.Models.Pokemon> ListPokeModel = consultor.GetAllPokemon();

                AllPokemons = new();
                //Recorremos cada model
                foreach (DAL.Models.Pokemon pokeModel in ListPokeModel)
                {
                    //Buscamos el tipo primario con esta funcion
                    Tipo primario = allTypes.Find(x => x.Id == pokeModel.id_tipo_primario) ?? new(0, "SIN TIPO", "ab", "ffffff");

                    Tipo? secudario;
                    if (pokeModel.id_tipo_secundario != null) secudario = allTypes.Find(x => x.Id == pokeModel.id_tipo_secundario);
                    else secudario = null;

                    //Creamos la entidad con los tipos que encontramos.
                    AllPokemons.Add(new(
                        pokeModel.id,
                        pokeModel.nombre,
                        pokeModel.altura,
                        pokeModel.peso,
                        pokeModel.generacion,
                        primario,
                        secudario
                        ));
                }

            }
            
            return AllPokemons;
        }
        public static Pokemon CreateNewPokemon(Pokemon loadPokemon)
        {
            PokemonAdapterAPI helper = new();
            DAL.Models.Pokemon response= helper.PostPokemon(new()
            {
                id = loadPokemon.Id,
                nombre = loadPokemon.Nombre,
                altura = loadPokemon.Altura,
                peso = loadPokemon.Peso,
                generacion = loadPokemon.Generacion,
                id_tipo_primario = loadPokemon.TipoPrimario.Id,
                id_tipo_secundario = loadPokemon.TipoSecundario?.Id
            });
            loadPokemon.Id = response.id;
            //Lo agregamos asi siempre muestra el ultimo
            AllPokemons?.Add(loadPokemon);
            return loadPokemon;
        }
        public static bool UpdatePokemon(Pokemon updatedPokemon)
        {
            PokemonAdapterAPI helper = new();
            bool response = helper.PutPokemon(new()
            {
                id = updatedPokemon.Id,
                nombre = updatedPokemon.Nombre,
                altura = updatedPokemon.Altura,
                peso = updatedPokemon.Peso,
                generacion = updatedPokemon.Generacion,
                id_tipo_primario = updatedPokemon.TipoPrimario.Id,
                id_tipo_secundario = updatedPokemon.TipoSecundario?.Id
            });
            return response;
        }
        public static bool DeletePokemon(int id_pokemon)
        {
            PokemonAdapterAPI helper = new();
            bool response = helper.DeletePokemon(id_pokemon);

            //Se confima que se elimino
            if (response)
            {
                Pokemon? rem = AllPokemons?.Find(x => x.Id == id_pokemon);

                //Si lo encontramos lo eliminamos de la lista para que no lo muestre mas
                if (rem != null) AllPokemons?.Remove(rem);
            }

            return response;
        }
    }
}