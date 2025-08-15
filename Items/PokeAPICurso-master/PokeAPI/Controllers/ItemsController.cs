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
        /// <summary>
        /// Obtiene todos los items que están activos en la base de datos.
        /// </summary>
        /// <returns>
        /// Una lista de objetos <see cref="Items"/> que representan los registros activos.
        /// </returns>

        [HttpGet]
        [ActionName("GetItemsActivo")]
        public ActionResult<List<Items>> GetItemsActivo()
        {
            try
            {
                DataTable respuesta = new();
                string consulta = "SELECT * FROM Items WHERE ItemActivo = 1";

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

        /// <summary>
        /// Devuelve un item según su ID.
        /// </summary>
        /// <param name="idItem">ID del item a buscar</param>
        /// <returns>Item encontrado o vacío si no existe</returns>
        /// <response code="200">Devuelve el item encontrado</response>
        /// <response code="204">No se encontró el item</response>
        /// <response code="400">Error en la solicitud</response>
        /// <response code="409">Error en la base de datos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{idItem}")]
        [ActionName("GetById")]
        [ProducesResponseType(typeof(Items), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult<Items> GetById(int idItem)
        {
            try
            {
                DataTable respuesta = new();
                string consulta = "SELECT * FROM Items WHERE id= " + idItem;

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
                    Items x = new(respuesta.Rows[0]);
                    return Ok(x); // 200
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

        /// <summary>
        /// Crea un nuevo item en la base de datos.
        /// </summary>
        /// <param name="itemCargar">Objeto Item a crear</param>
        /// <returns>Item creado</returns>
        /// <response code="201">Item creado correctamente</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="409">Error en la base de datos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ActionName("CargarItem")]
        [ProducesResponseType(typeof(Items), StatusCodes.Status201Created)]
        public ActionResult<Items> CargarItem([FromBody] Items itemCargar)
        {
            try
            {
                DataTable respuesta = new();
                string consulta =
                "INSERT INTO Items VALUES ('" + itemCargar.NombreItem + "'," + itemCargar.StockMaximo + ","
                + itemCargar.FechaCreacion + ",'" + itemCargar.Efecto + "'," + itemCargar.ItemActivo + ");" +
                "SELECT TOP 1 * FROM Items ORDER BY id DESC";

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
                    Items x = new(respuesta.Rows[0]);
                    return Created("Creado", x); // 201
                }
                else
                {
                    return Conflict(); // 409
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
