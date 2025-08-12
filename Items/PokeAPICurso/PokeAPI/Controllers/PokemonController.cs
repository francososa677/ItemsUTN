using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PokeAPI.Logs;
using PokeAPI.Models;
using System.Data;
using Microsoft.Data.SqlClient;  // Para SqlConnection, SqlCommand, SqlDataAdapter

namespace PokeAPI.Controllers
{
    /// <summary>
    /// Controlador para manejar operaciones sobre Pokémon.
    /// </summary>
    [EnableCors("CorsRules")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly Logger<PokemonController> _logger = new();
        private readonly string _connectionString;

        public PokemonController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DESA");
        }

        /// <summary>
        /// Recupera todos los pokemones.
        /// </summary>
        /// <returns>Lista de pokemones</returns>
        /// <response code="200">Devuelve todos los pokemones.</response>
        /// <response code="204">No se encontró ningún pokemon.</response>
        /// <response code="400">Ocurrió un error en la consulta.</response>
        /// <response code="409">Ocurrió un error en el procedimiento/vista de la base de datos.</response>
        /// <response code="500">Ocurrió un error en la API o en el servidor no documentada.</response>
        [HttpGet]
        [ActionName("ObtenerPokemones")]
        [ProducesResponseType(typeof(IEnumerable<Pokemones>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerPokemones()
        {
            DataTable respuesta = new();
            try
            {
                string consulta = "MODULO_POKEMON_OBTENER_POKEMONES_COMPLETO"; // Vista o procedimiento almacenado
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand comando = new(consulta, conexion);
                comando.CommandType = CommandType.StoredProcedure; // Cambiar a StoredProcedure si es procedimiento, o Text si es consulta directa

                SqlDataAdapter adaptador = new(comando);
                conexion.Open();
                adaptador.Fill(respuesta);
                conexion.Close();

                if (respuesta.Rows.Count == 0)
                {
                    _logger.RegistrarINFO("No se encontraron pokemones.");
                    return NoContent();
                }

                List<Pokemones> listadoCompleto = new();

                foreach (DataRow fila in respuesta.Rows)
                {
                    try
                    {
                        listadoCompleto.Add(new Pokemones(fila));
                    }
                    catch (Exception ex)
                    {
                        _logger.RegistrarERROR(ex, "Error al mapear DataRow a Pokemones.");
                        return Conflict("Ocurrió un error en la creación de los datos.");
                    }
                }

                return Ok(listadoCompleto);
            }
            catch (SqlException ex)
            {
                _logger.RegistrarERROR(ex, "Error en la consulta a la base de datos.");
                return Conflict("Error en la consulta a la base de datos.");
            }
            catch (Exception ex)
            {
                _logger.RegistrarERROR(ex, "Error interno del servidor.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
