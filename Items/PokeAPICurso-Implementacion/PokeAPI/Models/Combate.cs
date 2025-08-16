using System.Data;

namespace PokeAPI.Models
{
    /// <summary>
    /// Es una competicion entre dos entrenadores
    /// </summary>
    public class Combate
    {
        /// <summary>
        /// El identificador del combate
        /// </summary>
        public int id {  get; set; }
        /// <summary>
        /// El id del entrenador local
        /// </summary>
        public int id_entrenador_local { get; set; }
        /// <summary>
        /// El id del entrenador visitante
        /// </summary>
        public int id_entrenador_visita { get; set; }
        /// <summary>
        /// En que fecha esta registrado el combate
        /// </summary>
        public DateTime fecha_combate { get; set; }
        /// <summary>
        /// En que horario inicia
        /// </summary>
        public TimeOnly inicio_combate { get; set; }
        /// <summary>
        /// En que horario finaliza
        /// </summary>
        public TimeOnly? final_combate { get; set; }
        /// <summary>
        /// Cual es el estado del combate
        /// </summary>
        public int resultado_combate { get; set; }
        /// <summary>
        /// El constructor para crear los datos del combate desde una fila
        /// </summary>
        /// <param name="fila">El registro de este combate en la base de datos</param>
        public Combate(DataRow fila)
        {
            this.id = int.Parse(fila["id"].ToString() ?? "0");
            this.id_entrenador_local = int.Parse(fila["id_entrenador_local"].ToString() ?? "0");
            this.id_entrenador_visita = int.Parse(fila["id_entrenador_visita"].ToString() ?? "0");
            this.fecha_combate = DateTime.Parse(fila["fecha_combate"].ToString() ?? "2022-12-18");
            this.inicio_combate = TimeOnly.Parse(fila["inicio_combate"].ToString() ?? "00:00:00");

            string? final = fila["final_combate"].ToString();
            //Hago un control de nulidad diferente ya que no queremos que muestre un valor predeterminado
            if (final != null && final.Trim() != "") this.final_combate = TimeOnly.Parse(final);
            //Agrego el diferente a "" porque puede ocurrir que lo convierta a esta cadena
            else this.final_combate = null;

            this.resultado_combate = int.Parse(fila["resultado_combate"].ToString() ?? "0");
        }
        /// <summary>
        /// Es el constructor generico para llamarlo desde el endpoint
        /// </summary>
        public Combate()
        {
            this.id = -1;
            this.id_entrenador_local = 0;
            this.id_entrenador_visita = 0;
            this.fecha_combate = DateTime.Now;
            this.inicio_combate = TimeOnly.MinValue;
            this.final_combate = TimeOnly.MinValue;
            this.resultado_combate = 0;
        }

    }
}