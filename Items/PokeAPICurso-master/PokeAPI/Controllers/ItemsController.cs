using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokeAPI.Models;
using System.Data;

namespace PokeAPI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de Items.
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ItemsController : Controller
    {
        /// <summary>
        /// Devuelve todos los items de la base de datos.
        /// </summary>
        /// <returns>Lista de Items</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     GET /api/Items/GetItems
        ///     
        ///     RESPONSE:
        ///     [
        ///       {
        ///         "id": 1,
        ///         "nombreItem": "Poción",
        ///         "stockMaximo": 100,
        ///         "fechaCreacion": "2025-01-01T00:00:00",
        ///         "efecto": "Restaura 20 HP",
        ///         "itemActivo": true
        ///       }
        ///     ]
        /// </remarks>
        /// <response code="200">Devuelve la lista de items</response>
        /// <response code="204">No hay items en la base de datos</response>
        /// <response code="400">Error en la solicitud</response>
        /// <response code="409">Error en la base de datos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ActionName("GetItems")]
        [ProducesResponseType(typeof(List<Items>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Items>> GetItems()
        {
            try
            {
                DataTable respuesta = new();
                string consulta = "SELECT * FROM Items";

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new(consulta, conexionBase);
                comando.CommandType = CommandType.Text;

                SqlDataAdapter Adaptador = new(comando);
                conexionBase.Open();
                Adaptador.Fill(respuesta);
                conexionBase.Close();

                if (respuesta != null && respuesta.Rows.Count > 0)
                {
                    List<Items> listado = new();
                    foreach (DataRow row in respuesta.Rows)
                    {
                        listado.Add(new Items(row));
                    }
                    return Ok(listado); // 200
                }
                else
                {
                    return NoContent(); // 204
                }
            }
            catch (SqlException)
            {
                return Conflict("Ocurrió un error en la base de datos."); // 409
            }
            catch (ArgumentException)
            {
                return BadRequest("Solicitud inválida."); // 400
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor."); // 500
            }
        }
    }
}
