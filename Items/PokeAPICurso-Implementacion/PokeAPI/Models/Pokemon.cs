using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokeAPI.Adapters;
using PokeAPI.Logs;
using PokeAPI.Models;
using System.Data;

namespace PokeAPI.Models
{
    /// <summary>
    /// Esta clase representa un animal ficticio
    /// </summary>
    public class Pokemon
    {
        /// <summary>
        /// Su numero en la pokedex
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// El nombre del pokemon
        /// </summary>
        public string nombre { get; set; }
        /// <summary>
        /// La altura del pokemon en metros
        /// </summary>
        public float altura { get; set; }
        /// <summary>
        /// El peso del pokemon en kilogramos
        /// </summary>
        public float peso { get; set; }
        /// <summary>
        /// La generacion a la cual pertenece
        /// </summary>
        public int generacion { get; set; }
        /// <summary>
        /// Su tipo primario (obligatorio)
        /// </summary>
        public int id_tipo_primario { get; set; }
        /// <summary>
        /// Su tipo secundario (opcional)
        /// </summary>
        public int? id_tipo_secundario { get; set; }
        /// <summary>
        /// Una fila perteneciente a una tabla
        /// </summary>
        /// <param name="fila">Un DataRow proveniente de un DataTable</param>
        public Pokemon(DataRow fila)
        {
            //Los "??" significan que en caso de ser null o no existir deberia adoptar el valor que le digamos
            this.id = int.Parse(fila["id"].ToString() ?? "0");
            this.nombre = fila["nombre"].ToString() ?? "ERROR";
            this.altura = float.Parse(fila["altura"].ToString() ?? "0");
            this.peso = float.Parse(fila["peso"].ToString() ?? "0");
            this.generacion = int.Parse(fila["generacion"].ToString() ?? "1");
            this.id_tipo_primario = int.Parse(fila["id_tipo_primario"].ToString() ?? "0");

            string? secundario = fila["id_tipo_secundario"].ToString();
            //Hago un control de nulidad diferente ya que no queremos que muestre un valor predeterminado
            if (secundario != null && secundario.Trim() != "") this.id_tipo_secundario = int.Parse(secundario);
            //Agrego el diferente a "" porque puede ocurrir que lo convierta a esta cadena
            else this.id_tipo_secundario = null;
        }
        /// <summary>
        /// Debemos crear un constructor sin parametros para cuando se manda desde el body la clase
        /// </summary>
        public Pokemon()
        {
            this.id = -1;
            this.nombre = "";
            this.altura = 0;
            this.peso = 0;
            this.generacion = 0;
            this.id_tipo_primario = 0;
            this.id_tipo_secundario = null;
        }
    }
}