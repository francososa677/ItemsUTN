using System.Data;

namespace DAL.Models
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
            id = int.Parse(fila["id"].ToString() ?? "0");
            nombre = fila["nombre"].ToString() ?? "ERROR";
            altura = float.Parse(fila["altura"].ToString() ?? "0");
            peso = float.Parse(fila["peso"].ToString() ?? "0");
            generacion = int.Parse(fila["generacion"].ToString() ?? "1");
            id_tipo_primario = int.Parse(fila["id_tipo_primario"].ToString() ?? "0");

            string? secundario = fila["id_tipo_secundario"].ToString();
            //Hago un control de nulidad diferente ya que no queremos que muestre un valor predeterminado
            if (secundario != null && secundario.Trim() != "") id_tipo_secundario = int.Parse(secundario);
            //Agrego el diferente a "" porque puede ocurrir que lo convierta a esta cadena
            else id_tipo_secundario = null;
        }
        /// <summary>
        /// Debemos crear un constructor sin parametros para cuando se manda desde el body la clase
        /// </summary>
        public Pokemon()
        {
            id = -1;
            nombre = "";
            altura = 0;
            peso = 0;
            generacion = 0;
            id_tipo_primario = 0;
            id_tipo_secundario = null;
        }
    }
}