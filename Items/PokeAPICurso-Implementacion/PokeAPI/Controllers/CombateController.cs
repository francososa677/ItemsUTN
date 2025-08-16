using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PokeAPI.Adapters;
using PokeAPI.Logs;
using PokeAPI.Models;
using System.Data;

namespace PokeAPI.Controllers
{
    /// <summary>
    /// Este controlador permite manipular las funcionalidades relacionadas a combate
    /// </summary>
    [EnableCors("CorsRules")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CombateController : Controller
    {
        /// <summary>
        /// Nos permite generar un combate programado
        /// </summary>
        /// <returns>Devuelve el combate creado</returns>
        /// <remarks>
        /// Ejemplo de uso:
        /// 
        ///     PUT /api/Combate/CrearCombate
        ///
        ///     BODY:
        ///     {
        ///       "id_entrenador_local": 0,
        ///       "id_entrenador_visita": 0,
        ///       "fecha_combate": "2025-05-15T02:23:24.202Z",
        ///       "inicio_combate": "string",
        ///       "pokemones_local": [
        ///         0
        ///       ],
        ///       "pokemones_visita": [
        ///         0
        ///       ]
        ///     }
        ///     
        ///     RESPONSE:
        ///     {
        ///       "id": 0,
        ///       "id_entrenador_local": 0,
        ///       "id_entrenador_visita": 0,
        ///       "fecha_combate": "2025-05-15T02:23:24.203Z",
        ///       "inicio_combate": "string",
        ///       "final_combate": "string",
        ///       "resultado_combate": 0
        ///     }
        ///     
        /// </remarks>
        /// <response code="201" >Devuelve el pokemon si se actualizo</response>
        /// <response code="400" >Ocurre un error en la consulta </response>  
        /// <response code="409" >Ocurre un error en el procedimiento/vista de la base de datos </response>
        /// <response code="500" >Ocurre un error en la API o en el Servidor no documentada </response>
        /// <param name="nuevoCombate"> Es el pokemon que queremos actualizar en la base de datos</param>
        [HttpPost]
        [ActionName("CrearCombate")]
        [ProducesResponseType(typeof(Combate), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<Combate> CrearCombate([FromBody] CombateTransaccion nuevoCombate)
        {
            //Ejemplos de que puede ser un bad request en esta operacion
            if (nuevoCombate.id_entrenador_local == nuevoCombate.id_entrenador_visita) 
                return BadRequest("El id_entrenador_local y id_entrenador_visita deben ser diferentes");
            if(nuevoCombate.pokemones_local.Count == 0 || nuevoCombate.pokemones_visita.Count == 0)
                return BadRequest("Ambos equipos deben contener al menos un pokemon");

            //Creo el objeto transaccion con el nombre correspondiente
            TransaccionSQL transac = new("TransaccionCombate");

            //Agrego el procedimiento a la lista
            transac.procedimientos.Add("MODULO_COMBATE_INSERTAR_COMBATE");
            //Agrego sus parametros en la misma posicion
            transac.parametros.Add(new()
            {
                {"@id_entrenador_local",nuevoCombate.id_entrenador_local },
                {"@id_entrenador_visita",nuevoCombate.id_entrenador_visita },
                {"@fecha_combate",nuevoCombate.fecha_combate },
                {"@inicio_combate",nuevoCombate.inicio_combate },
            });

            //Cargamos el procedimiento con los pokemones locales
            foreach (int id_pokemon_local in nuevoCombate.pokemones_local)
            {
                //Agrego el procedimiento a la lista
                transac.procedimientos.Add("MODULO_COMBATE_INSERTAR_EQUIPO");
                //Agrego sus parametros en la misma posicion
                transac.parametros.Add(new()
                {
                    {"@id_pokemon",id_pokemon_local },
                    {"@id_entrenador",nuevoCombate.id_entrenador_local }
                });
            }

            //Agregamos los pokemones visitante
            foreach (int id_pokemon_visita in nuevoCombate.pokemones_visita)
            {
                //Agrego el procedimiento a la lista
                transac.procedimientos.Add("MODULO_COMBATE_INSERTAR_EQUIPO");
                //Agrego sus parametros en la misma posicion
                transac.parametros.Add(new()
                {
                    {"@id_pokemon", id_pokemon_visita},
                    {"@id_entrenador",nuevoCombate.id_entrenador_visita }
                });
            }

            try
            {
                GeneralAdapterSQL controlador = new();
                if (controlador.EjecutarTransaccion(transac))
                {
                    DataTable respuesta = controlador.EjecutarVista("MODULO_COMBATE_RECUPERAR_ULTIMO_COMBATE");
                    
                    //Puede ocurrir que la transaccion se completo pero fallo cuando buscamos el combate
                    if (respuesta.Rows.Count > 0 && respuesta.Rows[0][0].ToString()?.Trim() == "ERROR") 
                        return Conflict("ERROR: Se completo la transaccion pero no se pudo recuperar el combate");
                    else
                    {
                        //Creamos el combate y lo enviamos. Podria haber otro try catch por si la creacion falla
                        return Created("Combate creado: ",new Combate(respuesta.Rows[0]));
                    }
                }
                //No se completo exitosamente la transaccion levanto una excepcion
                else throw new Exception("TRANSACCION INVALIDA");
            }
            catch (Exception ex)
            {
                Logger.RegistrarERROR(ex, "ERROR: No se pudo completar el procedimiento CrearCombate ");
                return Conflict(ex.Message + " Imposible completar la accion");
            }
        }
    }
}