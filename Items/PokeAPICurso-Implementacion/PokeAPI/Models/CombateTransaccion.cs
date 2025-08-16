namespace PokeAPI.Models
{
    /// <summary>
    /// Esta clase contiene todos los datos necesario para dar de alta un combate
    /// </summary>
    public class CombateTransaccion
    {
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
        /// Un listado de ids asociado a nuestro entrenador local
        /// </summary>
        public List<int> pokemones_local { get; set; }
        /// <summary>
        /// Un listado de ids asociado a nuestro entrenador visitante
        /// </summary>
        public List<int> pokemones_visita { get; set; }
        /// <summary>
        /// Constructor generico para recibirlo desde el endpoint
        /// </summary>
        public CombateTransaccion()
        {
            this.id_entrenador_local = 0;
            this.id_entrenador_visita = 0;
            this.fecha_combate = DateTime.Now;
            this.inicio_combate = TimeOnly.MaxValue;
            this.pokemones_local = new();
            this.pokemones_visita = new();
        }
    }
}