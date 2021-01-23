using System;
using System.Collections.Generic;
using Npgsql;

namespace Base_de_datos
{
    public class BaseDeDatos
    {
        //private NpgsqlConnection _Conexion;
        private string _Servidor;
        private const string ServidorPorDefault = "Nombre o IP del servidor";
        //private System.Collections.Generic.List<Npgsql.NpgsqlParameter> _Parametros = new List<NpgsqlParameter>();
        private const string _StringDeConexion = "Server=:servidor:;Port=5432;User Id=userPrueba;Password=12345;Database=Prueba;";
        /// <summary>
        /// Se desencadena cuando la propiedad Status cambia de valor.
        /// </summary>
        //public event System.Data.StateChangeEventHandler AlCambiarStatus;

        public BaseDeDatos()
        {
            //this._Conexion = new NpgsqlConnection();
            //this._Conexion.StateChange += new System.Data.StateChangeEventHandler(eventoAlCambiarStatus);
        }

        /// <summary>
        /// Obtiene o establece el servidor de la conexión. Asignar string.Empty cuando se quiera volver a usar el servidor por default.
        /// </summary>
        public string Servidor
        {
            get
            {
                if (this._Servidor == string.Empty)
                    return BaseDeDatos.ServidorPorDefault;

                return this._Servidor;
            }
            set
            {
                if (value != this._Servidor)
                {
                    this._Servidor = value;
                }
            }
        }

        /// <summary>
        /// Obtiene el status actual de la conexión.
        /// </summary>
        public StatusDeConexion Status
        {
            get
            {
                //switch (this._Conexion.State)
                //{
                //    case System.Data.ConnectionState.Broken:
                //    case System.Data.ConnectionState.Closed:
                //        return StatusDeConexion.No_Conectado;

                //    case System.Data.ConnectionState.Connecting:
                //    case System.Data.ConnectionState.Executing:
                //    case System.Data.ConnectionState.Fetching:
                //        return StatusDeConexion.Ocupado;
                    
                //    case System.Data.ConnectionState.Open:
                //    default:
                //        return StatusDeConexion.Conectado;
                //}

                return 0;
            }
        }

        /// <summary>
        /// Obtiene la lista de parametros que se usará para la consulta. Cada consulta exitosa limpia la lista.
        /// </summary>
        public System.Collections.Generic.List<Npgsql.NpgsqlParameter> Parametros
        {
            get
            {
                //return this._Parametros;
                return null;
            }
        }

        /// <summary>
        /// Obtiene el string de conexión a utilizar.
        /// </summary>
        private string StringDeConexion
        {
            get
            {
                return BaseDeDatos._StringDeConexion.Replace(":servidor:", this.Servidor);
            }
        }

        /// <summary>
        /// Intenta establecer una conexión con la base de datos.
        /// </summary>
        public void Conectar()
        {
            //if (this._Conexion.ConnectionString != this.StringDeConexion)
            //{// Significa que se cambió el string de conexión y hay que reconectar
            //    this.Desconectar();
            //    this._Conexion.ConnectionString = this.StringDeConexion;
            //}
            //this._Conexion.Open();
        }

        /// <summary>
        /// Cierra la conexión con la base de datos.
        /// </summary>
        public void Desconectar()
        {
            //this._Conexion.Close();
        }

        /// <summary>
        /// Realiza una consulta a la base de datos y devuelve la tabla de resultados.
        /// </summary>
        /// <param name="query">Query SQL que se desea realizar.</param>
        /// <returns>Lista que contiene los datos devueltos por la consulta.</returns>
        public List<Dictionary<string, object>> EjecutarQuery(string query)
        {
            List<Dictionary<string, object>> lista = new List<Dictionary<string, object>>();
            //using (NpgsqlCommand consulta = new NpgsqlCommand(query, this._Conexion))
            //{
            //    consulta.Parameters.AddRange(this.Parametros.ToArray());
            //    consulta.Prepare();
            //    using (NpgsqlDataReader dr = consulta.ExecuteReader())
            //    {
            //        Dictionary<string, object> fila;
            //        while (dr.Read())
            //        {
            //            fila = new Dictionary<string, object>();
            //            for (int indice = 0; indice < dr.VisibleFieldCount; indice++)
            //            {
            //                fila.Add(dr.GetName(indice), dr[dr.GetName(indice)]);
            //            }
            //            lista.Add(fila);
            //        }
            //    }
            //    this.Parametros.Clear();
            //}
            return lista;
        }

        /// <summary>
        /// Realiza una consulta a la base de datos.
        /// </summary>
        /// <param name="query">Query SQL que se desea realizar.</param>
        /// <returns>Cantidad de filas afectadas. Devuelve -1 si la consulta no se realizó.</returns>
        public int EjecutarNonQuery(string query)
        {
            int filasAfectadas = -1;
            //using (NpgsqlCommand consulta = new NpgsqlCommand(query, this._Conexion))
            //{
            //    consulta.Parameters.AddRange(this.Parametros.ToArray());
            //    consulta.Prepare();
            //    filasAfectadas = consulta.ExecuteNonQuery();
            //    this.Parametros.Clear();
            //}
            return filasAfectadas;
        }

        /// <summary>
        /// Realiza una consulta a la base de datos y devuelve un resultado.
        /// </summary>
        /// <param name="query">Query SQL que se desea realizar.</param>
        /// <returns>Resultado obtenido de la consulta.</returns>
        public object EjecutarQueryEscalar(string query)
        {
            object resultado = null;
            //using (NpgsqlCommand consulta = new NpgsqlCommand(query, this._Conexion))
            //{
            //    consulta.Parameters.AddRange(this.Parametros.ToArray());
            //    consulta.Prepare();
            //    resultado = consulta.ExecuteScalar();
            //    this.Parametros.Clear();
            //}
            return resultado;
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarStatus.
        /// </summary>
        //protected virtual void eventoAlCambiarStatus(object sender, System.Data.StateChangeEventArgs e)
        //{
        //    //if (this.AlCambiarStatus != null)
        //    //    this.AlCambiarStatus(this, e);
        //}
    }
}
