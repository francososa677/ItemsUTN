using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PokeAPI.Adapters;
using PokeAPI.Logs;
using PokeAPI.Models;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PokeAPI.Controllers
{
    /// <summary>
    /// Este controlador esta en su version inicial y es para que tengan referencia de como funcionan los endpoints
    /// </summary>
    [EnableCors("CorsRules")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PokemonController : Controller
    {
        /// <summary>
        /// Recupera todos los pokemones
        /// </summary>
        /// <returns>Todos los pokemones como listado</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     GET /api/Pokemon/ObtenerPokemones/
        ///     
        ///     RESPONSE:
        ///     [
        ///        {
        ///          "id": 0,
        ///          "nombre": "string",
        ///          "altura": 0,
        ///          "peso": 0,
        ///          "generacion": 0,
        ///          "id_tipo_primario": 0,
        ///          "id_tipo_secundario": 0
        ///        }
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
        [ProducesResponseType(typeof(IEnumerable<Pokemon>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<Pokemon>> ObtenerPokemones()
        {
            //Creo la instancia para ejecutar el metodo
            GeneralAdapterSQL consultor = new();

            //Ejecuto la vista
            DataTable respuesta = consultor.EjecutarVista("MODULO_POKEMON_OBTENER_POKEMONES_COMPLETO");
            //Verifico que tenga al menos un registro
            if (respuesta.Rows.Count > 0)
            {
                //Rows[0] es primera fila y Rows[0][0] es primera columna de la primera fila.
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                //Uso el codigo 409 para notificar que ocurrio un error en la consulta del servidor
                else
                {
                    //Creo una nueva lista
                    List<Pokemon> listadoCompleto = new();
                    //Creo el listado de pokemones:
                    try
                    {
                        //Recorro los registros de la fila
                        foreach (DataRow registro in respuesta.Rows)
                        {
                            //Agrego todo al listado
                            listadoCompleto.Add(new(registro));
                        }
                        //Devuelvo el listado completo con el codigo 200
                        return Ok(listadoCompleto);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex,"Error buscando el pokemon");
                        return Conflict("Ocurrio un error en la creacion de los datos");
                    }
                }
            }
            else return NoContent();//No ocurrio un error simplemente que no existen pokemones cargados
        }

        /// <summary>
        /// Recupera un pokemon por el ID
        /// </summary>
        /// <returns>Devuelve un pokemon</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     GET /api/Pokemon/ObtenerPokemones/{id_pokemon}
        ///     
        ///     RESPONSE:
        ///     {
        ///       "id": 0,
        ///       "nombre": "string",
        ///       "altura": 0,
        ///       "peso": 0,
        ///       "generacion": 0,
        ///       "id_tipo_primario": 0,
        ///       "id_tipo_secundario": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200" >Devuelve el pokemon con ese ID</response>
        /// <response code="204" >No se encontro ningun pokemon </response>
        /// <response code="400" >Ocurre un error en la consulta </response>  
        /// <response code="409" >Ocurre un error en el procedimiento/vista de la base de datos </response>
        /// <response code="500" >Ocurre un error en la API o en el Servidor no documentada </response>
        /// <param name="id_pokemon">Es su numero idenficatorio en la pokedex</param>
        [HttpGet("{id_pokemon}")]
        [ActionName("ObtenerPokemonXId")]
        [ProducesResponseType(typeof(Pokemon), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Pokemon> ObtenerPokemonXId(int id_pokemon)
        {
            //Creo la instancia para ejecutar el metodo
            GeneralAdapterSQL consultor = new();

            //Ejecuto la vista
            DataTable respuesta = consultor.ExecuteStoredProcedure("MODULO_POKEMON_OBTENER_POKEMON_ID",
                new() { {"@id_pokemon",id_pokemon } });
            //Verifico que tenga al menos un registro
            if (respuesta.Rows.Count > 0)
            {
                //Rows[0] es primera fila y Rows[0][0] es primera columna de la primera fila.
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                //Uso el codigo 409 para notificar que ocurrio un error en la consulta del servidor
                else
                {
                    try
                    {
                        //Pokemon encontrado
                        Pokemon busqueda = new(respuesta.Rows[0]);
                        //Devuelvo el pokemon encontrado
                        return Ok(busqueda);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Error en la busqueda del pokemon");
                        return Conflict("Ocurrio un error en la creacion de los datos");
                    }
                }
            }
            else return NoContent();//No ocurrio un error simplemente que no existen pokemones cargados
        }
        /// <summary>
        /// Nos permite actualizar los datos del pokemon
        /// </summary>
        /// <returns>Devuelve el pokemon actualizado</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     PUT /api/Pokemon/ActualizarPokemon
        ///
        ///     BODY:
        ///     {
        ///       "id": 0,
        ///       "nombre": "string",
        ///       "altura": 0,
        ///       "peso": 0,
        ///       "generacion": 0,
        ///       "id_tipo_primario": 0,
        ///       "id_tipo_secundario": 0
        ///     }
        ///     
        ///     RESPONSE:
        ///     {
        ///       "id": 0,
        ///       "nombre": "string",
        ///       "altura": 0,
        ///       "peso": 0,
        ///       "generacion": 0,
        ///       "id_tipo_primario": 0,
        ///       "id_tipo_secundario": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="200" >Devuelve el pokemon si se actualizo</response>
        /// <response code="204" >No se encontro ningun pokemon para actualizar </response>
        /// <response code="400" >Ocurre un error en la consulta </response>  
        /// <response code="409" >Ocurre un error en el procedimiento/vista de la base de datos </response>
        /// <response code="500" >Ocurre un error en la API o en el Servidor no documentada </response>
        /// <param name="pokemonActualizado"> Es el pokemon que queremos actualizar en la base de datos</param>
        [HttpPut]
        [ActionName("ActualizarPokemon")]
        [ProducesResponseType(typeof(Pokemon), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Pokemon> ActualizarPokemon([FromBody]Pokemon pokemonActualizado)
        {
            //Creo la instancia para ejecutar el metodo
            GeneralAdapterSQL consultor = new();

            //Ejecuto el procedimiento de actualizar
            DataTable respuesta = consultor.ExecuteStoredProcedure
                ("MODULO_POKEMON_ACTUALIZAR_POKEMON",
                new() { { "@id_pokemon", pokemonActualizado.id },
                    {"@nombre",pokemonActualizado.nombre },
                    {"@altura",pokemonActualizado.altura },
                    {"@peso",pokemonActualizado.peso },
                    {"@generacion",pokemonActualizado.generacion },
                    {"@id_tipo_primario",pokemonActualizado.id_tipo_primario },
                    {"@id_tipo_secundario",pokemonActualizado.id_tipo_secundario },
                });
            //En la base por decision devuelto el registro modificado

            //Verifico que tenga al menos un registro
            if (respuesta.Rows.Count > 0)
            {
                //Rows[0] es primera fila y Rows[0][0] es primera columna de la primera fila.
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                //Uso el codigo 409 para notificar que ocurrio un error en la consulta del servidor
                else
                {
                    try
                    {
                        Pokemon actualizado = new(respuesta.Rows[0]);
                        //Devuelvo el pokemon actualizado
                        return Ok(actualizado);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Error modificando datos");
                        return Conflict("Ocurrio un error en la creacion de los datos");
                    }
                }
            }
            else return NoContent();//No ocurrio un error simplemente que no existen pokemones cargados
        }
        /// <summary>
        /// Nos permite agregar un pokemon a la pokedex
        /// </summary>
        /// <returns>Devuelve el pokemon creado</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     PUT /api/Pokemon/CrearPokemon
        ///
        ///     BODY:
        ///     {
        ///       "id": 0,
        ///       "nombre": "string",
        ///       "altura": 0,
        ///       "peso": 0,
        ///       "generacion": 0,
        ///       "id_tipo_primario": 0,
        ///       "id_tipo_secundario": 0
        ///     }
        ///     
        ///     RESPONSE:
        ///     {
        ///       "id": 0,
        ///       "nombre": "string",
        ///       "altura": 0,
        ///       "peso": 0,
        ///       "generacion": 0,
        ///       "id_tipo_primario": 0,
        ///       "id_tipo_secundario": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="201" >Devuelve el pokemon si se actualizo</response>
        /// <response code="400" >Ocurre un error en la consulta </response>  
        /// <response code="409" >Ocurre un error en el procedimiento/vista de la base de datos </response>
        /// <response code="418" >No es la pokedex correcta. Es una tetera </response>
        /// <response code="500" >Ocurre un error en la API o en el Servidor no documentada </response>
        /// <param name="pokemonActualizado"> Es el pokemon que queremos actualizar en la base de datos</param>
        [HttpPost]
        [ActionName("CrearPokemon")]
        [ProducesResponseType(typeof(Pokemon), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status418ImATeapot)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Pokemon> CrearPokemon([FromBody] Pokemon pokemonActualizado)
        {
            //Creo la instancia para ejecutar el metodo
            GeneralAdapterSQL consultor = new();

            //Ejecuto el procedimiento de actualizar
            DataTable respuesta = consultor.ExecuteStoredProcedure
                ("MODULO_POKEMON_INSERTAR_POKEMON",
                new() {
                    {"@nombre",pokemonActualizado.nombre },
                    {"@altura",pokemonActualizado.altura },
                    {"@peso",pokemonActualizado.peso },
                    {"@generacion",pokemonActualizado.generacion },
                    {"@id_tipo_primario",pokemonActualizado.id_tipo_primario },
                    {"@id_tipo_secundario",pokemonActualizado.id_tipo_secundario },
                });
            //En la base por decision devuelto el registro modificado

            //Verifico que tenga al menos un registro
            if (respuesta.Rows.Count > 0)
            {
                //Rows[0] es primera fila y Rows[0][0] es primera columna de la primera fila.
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                //Uso el codigo 409 para notificar que ocurrio un error en la consulta del servidor
                else
                {
                    try
                    {
                        //Pokemon actualizado
                        Pokemon pokemonCreado = new(respuesta.Rows[0]);
                        //Devuelve el pokemon creado 201
                        return Created("Pokemon Creado: ",pokemonCreado);
                    }
                    catch (Exception ex)
                    {
                        Logger.RegistrarERROR(ex, "Error creando el pokemon");
                        return Conflict("Ocurrio un error en la creacion de los datos");
                    }
                }
            }
            else
            {
                ObjectResult result = new("ERROR: No es la pokedex correcta")
                {
                    StatusCode = 418
                };
                return result;
            }
        }
        /// <summary>
        /// Elimina el pokemon de la pokedex
        /// </summary>
        /// <returns>Devuelve un codigo OK</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     GET /api/Pokemon/EliminarPokemon/{id_pokemon}
        ///     
        ///     RESPONSE:
        ///     "Pokemon Eliminado"
        ///     
        /// </remarks>
        /// <response code="200" >Devuelve un mensaje de exito</response>
        /// <response code="400" >Ocurre un error en la consulta </response>  
        /// <response code="409" >Ocurre un error en el procedimiento/vista de la base de datos </response>
        /// <response code="500" >Ocurre un error en la API o en el Servidor no documentada </response>
        /// <param name="id_pokemon">Es su numero idenficatorio en la pokedex</param>
        [HttpDelete("{id_pokemon}")]
        [ActionName("EliminarPokemon")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Pokemon> EliminarPokemon(int id_pokemon)
        {
            //Creo la instancia para ejecutar el metodo
            GeneralAdapterSQL consultor = new();

            //Ejecuto la vista
            DataTable respuesta = consultor.ExecuteStoredProcedure("MODULO_POKEMON_ELIMINAR_POKEMON",
                new() { { "@id_pokemon", id_pokemon } });
            //Si no tiene registro es que no existe mas el pokemon
            if (respuesta.Rows.Count > 0)
            {
                //Rows[0] es primera fila y Rows[0][0] es primera columna de la primera fila.
                if (respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") return Conflict();
                else return BadRequest();
            }
            else return Ok("Pokemon Eliminado");
        }
    }
}