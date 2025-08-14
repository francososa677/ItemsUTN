using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
//using PokeAPI.Logs;
using PokeAPI.Models;
using System.Data;

namespace PokeAPI.Controllers
{
    /// <summary>
    /// Es el controlador para los becarios del area y nacionales
    /// </summary>
    [EnableCors("CorsRules")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PokemonController : Controller
    {
        /// <summary>
        /// EN: The logger functions as a register of the exception that happen in the runtime. <br/>
        /// ES: El logger funciona como el registro de excpciones que pasan en tiempo de ejecuccion <br/>
        /// </summary>
        //private readonly Logger _logger = new();
        /// <summary>
        /// Recupera todos los pokemones
        /// </summary>
        /// <returns>Todos los pokemones</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     GET /api/Beca/ObtenerBecariosCompleto/
        ///     
        ///     RESPONSE:
        ///     [
        ///       {
        ///         "id": 0
        ///       }
        ///     ]
        ///     
        /// </remarks>
        /// <response code="200" >Devuelve todos los pokemones </response>
        /// <response code="204" >No se encontro ningun pokemon </response>
        /// <response code="400" >Ocurre un error en la consulta </response>  
        /// <response code="409" >Ocurre un error en el procedimiento/vista de la base de datos </response>
        /// <response code="500" >Ocurre un error en la API o en el Servidor no documentada </response>
        [HttpGet]
        [ActionName("ObtenerPokemones")]
        [ProducesResponseType(typeof(IEnumerable<Pokemones>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Pokemones>>> ObtenerPokemones()
        {
            return Ok();
        }
    }
}
