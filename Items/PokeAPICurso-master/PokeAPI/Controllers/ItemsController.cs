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
        /// Recupera todos los items de la base de datos, sin importar si están activos o no.
        /// </summary>
        /// <returns>Lista de Items</returns>
        /// <response code="200">Devuelve la lista de items</response>
        /// <response code="204">No hay items en la base de datos</response>
        /// <response code="400">Error en la solicitud</response>
        /// <response code="409">Error en la base de datos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ActionName("ObtenerItemsCompleto")]
        [ProducesResponseType(typeof(List<Items>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Items>> ObtenerItemsCompleto()
        {
            try
            {
                DataTable respuesta = new();
                string consulta = "SELECT * FROM Items";

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new SqlCommand(consulta, conexionBase);
                comando.CommandType = CommandType.Text;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                conexionBase.Open();
                adaptador.Fill(respuesta);
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
        /// Recupera solo los ítems activos.
        /// </summary>
        /// <returns>Lista de Items activos</returns>
        [HttpGet]
        [ActionName("ObtenerItemsActivo")]
        [ProducesResponseType(typeof(List<Items>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<List<Items>> ObtenerItemsActivo()
        {
            try
            {
                DataTable respuesta = new();
                string consulta = "SELECT * FROM Items WHERE item_activo = 1";

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new SqlCommand(consulta, conexionBase);
                comando.CommandType = CommandType.Text;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                conexionBase.Open();
                adaptador.Fill(respuesta);
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
        /// Recupera un item según su ID.
        /// </summary>
        /// <param name="id_item">ID del item a buscar</param>
        [HttpGet("{id_item}")]
        [ActionName("ObtenerItemXid")]
        [ProducesResponseType(typeof(Items), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Items> ObtenerItemXid([FromRoute] int id_item)
        {
            try
            {
                if (id_item <= 0)
                    return BadRequest("El id del item debe ser mayor que cero."); // 400

                DataTable respuesta = new();
                string consulta = "SELECT * FROM Items WHERE id = " + id_item;

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new SqlCommand(consulta, conexionBase);
                comando.CommandType = CommandType.Text;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                conexionBase.Open();
                adaptador.Fill(respuesta);
                conexionBase.Close();

                if (respuesta != null && respuesta.Rows.Count > 0)
                {
                    Items x = new Items(respuesta.Rows[0]);
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
        [HttpPost]
        [ActionName("CargarItem")]
        [ProducesResponseType(typeof(Items), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Items> CargarItem([FromBody] Items itemCargar)
        {
            try
            {
                DataTable respuesta = new();
                string fechaFormateada = itemCargar.FechaCreacion.ToString("yyyy-MM-dd");
                int activo = itemCargar.ItemActivo ? 1 : 0;

                string consulta =
                    "INSERT INTO Items (nombre_item, stock_maximo, fecha_creacion, efecto, item_activo) " +
                    "VALUES ('" + itemCargar.NombreItem + "', " + itemCargar.StockMaximo + ", '" +
                    fechaFormateada + "', '" + itemCargar.Efecto + "', " + activo + "); " +
                    "SELECT TOP 1 * FROM Items ORDER BY id DESC";

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new SqlCommand(consulta, conexionBase);
                comando.CommandType = CommandType.Text;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                conexionBase.Open();
                adaptador.Fill(respuesta);
                conexionBase.Close();

                if (respuesta != null && respuesta.Rows.Count > 0)
                {
                    Items x = new Items(respuesta.Rows[0]);
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

        /// <summary>
        /// Modifica un item existente.
        /// </summary>
        /// <param name="itemModificado">Objeto Item con los datos modificados</param>
        [HttpPut]
        [ActionName("ModificarItem")]
        [ProducesResponseType(typeof(Items), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Items> ModificarItem([FromBody] Items itemModificado)
        {
            try
            {
                if (itemModificado == null || itemModificado.Id <= 0)
                    return BadRequest("Datos inválidos."); // 400

                DataTable respuesta = new();
                string fechaFormateada = itemModificado.FechaCreacion.ToString("yyyy-MM-dd");
                int activo = itemModificado.ItemActivo ? 1 : 0;

                string consulta =
                    "UPDATE Items SET " +
                    "nombre_item = '" + itemModificado.NombreItem + "', " +
                    "stock_maximo = " + itemModificado.StockMaximo + ", " +
                    "fecha_creacion = '" + fechaFormateada + "', " +
                    "efecto = '" + itemModificado.Efecto + "', " +
                    "item_activo = " + activo + " " +
                    "WHERE id = " + itemModificado.Id + "; " +
                    "SELECT * FROM Items WHERE id = " + itemModificado.Id;

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new SqlCommand(consulta, conexionBase);
                comando.CommandType = CommandType.Text;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                conexionBase.Open();
                adaptador.Fill(respuesta);
                conexionBase.Close();

                if (respuesta != null && respuesta.Rows.Count > 0)
                {
                    Items x = new Items(respuesta.Rows[0]);
                    return Ok(x); // 200
                }
                else
                {
                    return NotFound("No se encontró el ítem a modificar."); // 404
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
        /// Desactiva un item (cambia item_activo a false).
        /// </summary>
        /// <param name="id_item">ID del item a desactivar</param>
        [HttpDelete("{id_item}")]
        [ActionName("DesactivarItem")]
        [ProducesResponseType(typeof(Items), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Items> DesactivarItem([FromRoute] int id_item)
        {
            try
            {
                if (id_item <= 0)
                    return BadRequest("El id del item debe ser mayor que cero."); // 400

                DataTable respuesta = new();
                string consulta =
                    "UPDATE Items SET item_activo = 0 " +
                    "WHERE id = " + id_item + "; " +
                    "SELECT * FROM Items WHERE id = " + id_item;

                using SqlConnection conexionBase = new SqlConnection(
                    "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\USUARIO\\Documents\\ItemsUTN\\Items\\PokeApiCurso-master\\PokeApi\\PokeDB.mdf;Integrated Security=True"
                );

                using SqlCommand comando = new SqlCommand(consulta, conexionBase);
                comando.CommandType = CommandType.Text;
                SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                conexionBase.Open();
                adaptador.Fill(respuesta);
                conexionBase.Close();

                if (respuesta != null && respuesta.Rows.Count > 0)
                {
                    Items x = new Items(respuesta.Rows[0]);
                    return Ok(x); // 200
                }
                else
                {
                    return NotFound("No se encontró el ítem a desactivar."); // 404
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
