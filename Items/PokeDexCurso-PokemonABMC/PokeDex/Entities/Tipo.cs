using DAL.Adapters;
using DAL.Models;
using Microsoft.VisualBasic;
using PokeDex.Entities;

namespace PokeDex.Entities
{
    public class Tipo
    {
        private static List<Tipo>? AllTypes;
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Abreviatura { get; set; }
        public string Color { get; set; }
        public Tipo(int id, string nombre, string abreviatura, string color)
        {
            Id = id;
            Nombre = nombre;
            Abreviatura = abreviatura;
            Color = color;
        }
        public static List<Tipo> GetAllTypes()
        {
            //Si es nulo solicito la lista en la API
            if(AllTypes == null)
            {
                TipoPokemonAdapter consulta = new();
                List<TipoPokemon> listado = consulta.GetAllTypes();
                AllTypes = new();
                //En este caso es una conversion 1 a 1 no deberia haber muchos problemas
                foreach (var modelTipo in listado) AllTypes.Add(new(modelTipo.id, modelTipo.name, modelTipo.abreviatura, modelTipo.color));
            }
            return AllTypes;
        }
    }
}