using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PokeAPI.Adapters;
using PokeAPI.Logs;
using System.Collections.Generic;
using System.Data;

namespace PokeAPI.Adapters
{
    /// <summary>
    /// Nuestra clase que nos va a permitir controlar las conexiones a la base de datos.
    /// </summary>
    public class GeneralAdapterSQL
    {
        /// <summary>
        /// La cadena de conexion que esta usando actualmente nuestra API
        /// </summary>
        private static string? CadenaConexion = null;
        /// <summary>
        /// La configuracion que tenemos en nuestra API
        /// </summary>
        private static SettingsReader? Configuracion = null;
        /// <summary>
        /// Metodo que recupera la configuracion de nuestro appsettings.json
        /// </summary>
        private static SettingsReader ObtenerConfiguracion() => SettingsReader.GetAppSettings();
        /// <summary>
        /// Recupera la cadena de conexion desde nuestra configuracion
        /// </summary>
        private static void ObtenerCadenaConexion()
        {
            if (CadenaConexion != null && CadenaConexion.Trim() != "") return;
            //Este if no tiene el else porque una vez que recupera la conexion deberia continuar normalmente
            if (Configuracion == null) Configuracion = ObtenerConfiguracion();

            //En esta situacion siempre esta asignado algun valor a Configuracion

            //Comprueba que exista un valor de env y una clave para este entorno
            if (Configuracion.Env.Trim() != "" && Configuracion.ConnectionStrings.ContainsKey(Configuracion.Env))
                CadenaConexion = Configuracion.ConnectionStrings[Configuracion.Env];
            else CadenaConexion = null;
        }
        /// <summary>
        /// Con el nombre de la vista permite recuperar los valores que contiene
        /// </summary>
        /// <param name="vista">El nombre de la vista que queremos consultar</param>
        /// <returns>Una tabla con valores</returns>
        public DataTable EjecutarVista(string vista)
        {
            ObtenerCadenaConexion();
            //Es necesario ponerlo por fuera para poder usar el bloque finally
            using SqlConnection conexionBase = new(CadenaConexion);
            DataTable respuesta = new();
            try
            {
                //Con esto recupera la informacion de la vista
                string consulta = "SELECT * FROM "+ vista;
                using var comando = new SqlCommand(consulta, conexionBase);//Creamos el comando en la base con la conexion
                comando.CommandType = CommandType.Text;//Notificamos a la base que vamos a enviar
                SqlDataAdapter Adaptador = new(comando);
                conexionBase.Open(); //Abre la conexion (Puede fallar)
                Adaptador.Fill(respuesta); //Contacta la base y ejecuta el comando
            }
            catch (Exception ex)
            {
                //Registramos el error en nuestra carpeta
                Logger.RegistrarERROR(ex, "Error al consultar la vista: " + vista);
                //Esta parte es para devolver un codigo de error al endpoint
                respuesta.Columns.Add("RESULTADO");
                respuesta.Rows.Add("ERROR");
            }
            //Lo va a ejecutar no importa que parte del codigo realice
            finally
            {
                //Limpia cualquier cadena de conexion que tengamos
                SqlConnection.ClearAllPools();
                //Cierra la base de datos siempre
                conexionBase.Close();
            }
            return respuesta;
        }
        /// <summary>
        /// Es un esquema de conversion de tipo de variables C# a tipo de variables SQL
        /// </summary>
        /// <param name="variable">La variable que usamos de parametro</param>
        /// <returns>Un tipo de base de datos SQL para ejecutar el procedimiento</returns>
        private static SqlDbType GetDBType(object variable)
        {
            string name = variable.GetType().Name;
            switch (variable.GetType().Name)
            {
                case "Int32":
                    return SqlDbType.Int;
                case "String" :
                    return SqlDbType.VarChar;
                case "DateTime":
                    return SqlDbType.DateTime;
                //Lo agregue despues
                case "TimeOnly":
                    return SqlDbType.Time;
                case "Single"://Considera valores con un decimal como single
                    return SqlDbType.Decimal;
                case "Decimal":
                    return SqlDbType.Decimal;
                case "Float":
                    return SqlDbType.Float;
                case "Bool":
                    return SqlDbType.Bit;
                //Es muy probable que la base de datos pueda convertir de string a cualquier tipo de variable
                default:
                    return SqlDbType.VarChar;
            }
        }


        /// <summary>
        /// Metodo para ejecutar procedimientos en la base de datos de manera comoda
        /// </summary>
        /// <param name="procedimiento">Nombre del procedimiento</param>
        /// <param name="parametros">Un diccionario con los parametros con su nombre en el procedimiento</param>
        /// <returns>Una tabla con la respuesta del procedimiento</returns>
        public DataTable ExecuteStoredProcedure(string procedimiento, Dictionary<string, object?> parametros)
        {
            ObtenerCadenaConexion();
            //Es necesario ponerlo por fuera para poder usar el bloque finally
            using SqlConnection conexionBase = new(CadenaConexion);
            DataTable respuesta = new();
            try
            {
                using var comando = new SqlCommand(procedimiento, conexionBase);//Creamos el comando en la base con la conexion
                comando.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter Adaptador = new(comando);
                foreach (var item in parametros)
                {
                    if (item.Value == null || item.Value.ToString()?.Trim() == "NULL")
                    {
                        comando.Parameters.AddWithValue(item.Key, DBNull.Value);
                    }
                    else
                    {
                        comando.Parameters.Add(item.Key, GetDBType(item.Value));
                        comando.Parameters[item.Key].Value = item.Value;
                    }
                }
                conexionBase.Open();
                Adaptador.Fill(respuesta); 
            }
            catch (Exception ex)
            {
                //Registramos el error en nuestra carpeta
                Logger.RegistrarERROR(ex, "Error al consultar el procedimiento: " + procedimiento);
                //Esta parte es para devolver un codigo de error al endpoint
                respuesta.Columns.Add("RESULTADO");
                respuesta.Rows.Add("ERROR");
            }
            //Lo va a ejecutar no importa que parte del codigo realice
            finally
            {
                //Limpia cualquier cadena de conexion que tengamos
                SqlConnection.ClearAllPools();
                //Cierra la base de datos siempre
                conexionBase.Close();
            }
            return respuesta;
        }
        /// <summary>
        /// Este metodo nos permite ejecutar un conjunto de procedimientos con su parametros en un bloque TRANSACT
        /// </summary>
        /// <param name="transaccionPropia">Es un objeto que contiene la informacion para realizar la transaccion</param>
        /// <returns>Un boleano dependiendo si se completo exitosamente o no la accion</returns>
        public bool EjecutarTransaccion(TransaccionSQL transaccionPropia)
        {
            ObtenerCadenaConexion();

            DataTable respuesta = new();
            bool resultado = true;

            using (var conexionBase = new SqlConnection(CadenaConexion))
            {
                conexionBase.Open();

                //Iniciamos la transaccion
                using var transaccionSQL = conexionBase.BeginTransaction(transaccionPropia.nombre_transaccion);
                try
                {
                    //Recorro todos los procedimientos en orden
                    for (int i = 0; i < transaccionPropia.procedimientos.Count; i++)
                    {
                        string procedimiento = transaccionPropia.procedimientos[i];
                        Dictionary<string, object> parametros = transaccionPropia.parametros[i];

                        respuesta = new();
                        using var comando = new SqlCommand(procedimiento, conexionBase)
                        {
                            CommandType = CommandType.StoredProcedure,
                            Transaction = transaccionSQL
                        };

                        SqlDataAdapter adapter = new(comando);

                        foreach (var item in parametros)
                        {
                            if (item.Value == null || item.Value.ToString()?.Trim() == "NULL")
                            {
                                comando.Parameters.AddWithValue(item.Key, DBNull.Value);
                            }
                            else
                            {
                                comando.Parameters.Add(item.Key, GetDBType(item.Value));
                                comando.Parameters[item.Key].Value = item.Value;
                            }
                        }

                        adapter.Fill(respuesta);

                        //En todas las respuestas debemos devolver algo
                        //Podemos elegir que sea un codigo o que sea la tabla
                        if (respuesta.Rows.Count == 0)
                        {
                            Logger.RegistrarERROR(new(), "ERROR EJECUTANDO EL PROCEDIMIENTO: " + procedimiento);
                            respuesta = new();
                            //Esto es para que la API pueda devolver un conflict() (409)
                            respuesta.Columns.Add("MENSAJE");
                            respuesta.Rows.Add("ERROR");
                            //Si falla hacemos un rollback y rompemos le ciclo con return
                            transaccionSQL.Rollback();
                            resultado = false;
                            break;
                        }
                    }
                    if(resultado) transaccionSQL.Commit();

                }
                catch (Exception ex)
                {
                    respuesta = new();
                    //Esto es para que la API pueda devolver un conflict() (409)
                    respuesta.Columns.Add("MENSAJE");
                    respuesta.Rows.Add("ERROR");
                    //Registramos en el Log el error
                    Logger.RegistrarERROR(ex, "ERROR EJECUTANDO LA TRANSACCION: " + transaccionPropia.nombre_transaccion);
                    transaccionSQL.Rollback();
                    resultado = false;
                }
                finally
                {
                    SqlConnection.ClearAllPools();
                    conexionBase.Close();
                }

            };

            return resultado;
        }
    }
    /// <summary>
    /// Clase que nos permite manipular una transaccion
    /// </summary>
    public class TransaccionSQL
    {
        /// <summary>
        /// El nombre que le vamos a asignar a esta operacion en la base de datos
        /// </summary>
        public string nombre_transaccion { get; set; }
        /// <summary>
        /// Es el nombre de los procedimientos que queremos ejecutar
        /// </summary>
        public List<string> procedimientos { get; set; }
        /// <summary>
        /// Es un listado de parametros asociados a cada procedimiento
        /// </summary>
        public List<Dictionary<string, object>> parametros { get; set; }
        //No los creo dentro de un diccionario porque se hacia muy complejo de explicar. Pero quedaria como:
        //Dictionary<string,Dictionary<string, object>>

        /// <summary>
        /// Constructor Generico del controlador
        /// </summary>
        public TransaccionSQL(string nombreTransaccion)
        {
            this.nombre_transaccion = nombreTransaccion;
            this.procedimientos = new();
            this.parametros = new();
        }
    }
}