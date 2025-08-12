using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace PokeAPI.Controllers
{
    [ApiController]
    [Route("Items/api")]
    public class ItemsController : ControllerBase
    {
        private readonly string _connectionString;
        private readonly PokeAPI.Logs.Logger<ItemsController> _logger;

        public ItemsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DESA");
            _logger = new PokeAPI.Logs.Logger<ItemsController>();
        }

        /// <summary>
        /// Recupera todos los items, activos o no.
        /// </summary>
        /// <returns>Lista completa de items.</returns>
        [HttpGet("ObtenerItemsCompleto")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerItemsCompleto()
        {
            DataTable respuesta = new();
            try
            {
                string consulta = "SELECT * FROM Items"; // O la vista/procedimiento que uses
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand comando = new(consulta, conexion);
                comando.CommandType = CommandType.Text;

                SqlDataAdapter adaptador = new(comando);
                conexion.Open();
                adaptador.Fill(respuesta);
                conexion.Close();

                if (respuesta.Rows.Count == 0)
                    return NoContent();

                List<Item> lista = new();
                foreach (DataRow fila in respuesta.Rows)
                    lista.Add(new Item(fila));

                return Ok(lista);
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

        /// <summary>
        /// Recupera solo los items activos.
        /// </summary>
        /// <returns>Lista de items activos.</returns>
        [HttpGet("ObtenerItemsActivo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerItemsActivo()
        {
            DataTable respuesta = new();
            try
            {
                string consulta = "SELECT * FROM Items WHERE item_activo = 1";
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand comando = new(consulta, conexion);
                comando.CommandType = CommandType.Text;

                SqlDataAdapter adaptador = new(comando);
                conexion.Open();
                adaptador.Fill(respuesta);
                conexion.Close();

                if (respuesta.Rows.Count == 0)
                    return NoContent();

                List<Item> lista = new();
                foreach (DataRow fila in respuesta.Rows)
                    lista.Add(new Item(fila));

                return Ok(lista);
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

        /// <summary>
        /// Recupera un item por su identificador único.
        /// </summary>
        /// <param name="id_item">Identificador del item.</param>
        /// <returns>Item correspondiente al id.</returns>
        [HttpGet("ObtenerItemXid/{id_item}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerItemXid(int id_item)
        {
            DataTable respuesta = new();
            try
            {
                string consulta = "SELECT * FROM Items WHERE id = @id_item";
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand comando = new(consulta, conexion);
                comando.CommandType = CommandType.Text;
                comando.Parameters.AddWithValue("@id_item", id_item);

                SqlDataAdapter adaptador = new(comando);
                conexion.Open();
                adaptador.Fill(respuesta);
                conexion.Close();

                if (respuesta.Rows.Count == 0)
                    return NoContent();

                Item item = new Item(respuesta.Rows[0]);
                return Ok(item);
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

        /// <summary>
        /// Carga un nuevo item.
        /// </summary>
        /// <param name="item">Datos del item a cargar.</param>
        /// <returns>Item cargado con éxito.</returns>
        [HttpPost("CargarItem")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CargarItem([FromBody] Item item)
        {
            if (item == null)
                return BadRequest("El item no puede ser nulo.");

            try
            {
                string consulta = @"INSERT INTO Items (nombre_item, stock_maximo, fecha_creacion, efecto, item_activo) 
                                    VALUES (@nombre_item, @stock_maximo, @fecha_creacion, @efecto, @item_activo)";
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand comando = new(consulta, conexion);
                comando.CommandType = CommandType.Text;

                comando.Parameters.AddWithValue("@nombre_item", item.Nombre_Item);
                comando.Parameters.AddWithValue("@stock_maximo", item.Stock_Maximo);
                comando.Parameters.AddWithValue("@fecha_creacion", item.Fecha_Creacion);
                comando.Parameters.AddWithValue("@efecto", item.Efecto);
                comando.Parameters.AddWithValue("@item_activo", item.Item_Activo);

                conexion.Open();
                int filas = comando.ExecuteNonQuery();
                conexion.Close();

                if (filas > 0)
                    return CreatedAtAction(nameof(ObtenerItemXid), new { id_item = item.Id }, item);
                else
                    return Conflict("No se pudo insertar el item.");
            }
            catch (SqlException ex)
            {
                _logger.RegistrarERROR(ex, "Error en la base de datos al insertar.");
                return Conflict("Error en la base de datos al insertar.");
            }
            catch (Exception ex)
            {
                _logger.RegistrarERROR(ex, "Error interno del servidor.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Modifica un item existente.
        /// </summary>
        /// <param name="item">Datos actualizados del item.</param>
        /// <returns>Item modificado.</returns>
        [HttpPut("ModificarItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ModificarItem([FromBody] Item item)
        {
            if (item == null)
                return BadRequest("El item no puede ser nulo.");

            try
            {
                string consultaExiste = "SELECT COUNT(*) FROM Items WHERE id = @id";
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand cmdExiste = new(consultaExiste, conexion);
                cmdExiste.Parameters.AddWithValue("@id", item.Id);

                conexion.Open();
                int count = (int)cmdExiste.ExecuteScalar();
                if (count == 0)
                {
                    conexion.Close();
                    return NotFound("El item no existe.");
                }

                string consultaUpdate = @"UPDATE Items SET 
                                            nombre_item = @nombre_item,
                                            stock_maximo = @stock_maximo,
                                            fecha_creacion = @fecha_creacion,
                                            efecto = @efecto,
                                            item_activo = @item_activo
                                        WHERE id = @id";

                using SqlCommand cmdUpdate = new(consultaUpdate, conexion);
                cmdUpdate.Parameters.AddWithValue("@nombre_item", item.Nombre_Item);
                cmdUpdate.Parameters.AddWithValue("@stock_maximo", item.Stock_Maximo);
                cmdUpdate.Parameters.AddWithValue("@fecha_creacion", item.Fecha_Creacion);
                cmdUpdate.Parameters.AddWithValue("@efecto", item.Efecto);
                cmdUpdate.Parameters.AddWithValue("@item_activo", item.Item_Activo);
                cmdUpdate.Parameters.AddWithValue("@id", item.Id);

                int filas = cmdUpdate.ExecuteNonQuery();
                conexion.Close();

                if (filas > 0)
                    return Ok(item);
                else
                    return Conflict("No se pudo modificar el item.");
            }
            catch (SqlException ex)
            {
                _logger.RegistrarERROR(ex, "Error en la base de datos al modificar.");
                return Conflict("Error en la base de datos al modificar.");
            }
            catch (Exception ex)
            {
                _logger.RegistrarERROR(ex, "Error interno del servidor.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }

        /// <summary>
        /// Desactiva (baja lógica) un item por id.
        /// </summary>
        /// <param name="id_item">Id del item a desactivar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPatch("DesactivarItem/{id_item}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DesactivarItem(int id_item)
        {
            try
            {
                string consultaExiste = "SELECT COUNT(*) FROM Items WHERE id = @id";
                using SqlConnection conexion = new(_connectionString);
                using SqlCommand cmdExiste = new(consultaExiste, conexion);
                cmdExiste.Parameters.AddWithValue("@id", id_item);

                conexion.Open();
                int count = (int)cmdExiste.ExecuteScalar();
                if (count == 0)
                {
                    conexion.Close();
                    return NotFound("El item no existe.");
                }

                string consultaUpdate = "UPDATE Items SET item_activo = 0 WHERE id = @id";
                using SqlCommand cmdUpdate = new(consultaUpdate, conexion);
                cmdUpdate.Parameters.AddWithValue("@id", id_item);

                int filas = cmdUpdate.ExecuteNonQuery();
                conexion.Close();

                if (filas > 0)
                    return Ok("Item desactivado correctamente.");
                else
                    return Conflict("No se pudo desactivar el item.");
            }
            catch (SqlException ex)
            {
                _logger.RegistrarERROR(ex, "Error en la base de datos al desactivar.");
                return Conflict("Error en la base de datos al desactivar.");
            }
            catch (Exception ex)
            {
                _logger.RegistrarERROR(ex, "Error interno del servidor.");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
