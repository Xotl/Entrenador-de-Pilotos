using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Npgsql;

namespace Base_de_datos
{
    [Serializable]
    public abstract class Modelo
	{
        protected string _Id = string.Empty;
        private Base_de_datos.BaseDeDatos _ConexionDB;
        protected bool _SeHaModificado = false;

        /// <summary>
        /// Obtiene o establece el Id de este objeto. Una vez establecido no puede ser editado.
        /// </summary>
        public string Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                if (this._Id == string.Empty)
                {
                    Guid id = new Guid(value);// De esta forma se valida que sea un UUID válido.
                    this._Id = id.ToString();
                }
                else
                {
                    throw new InvalidOperationException("Este objeto ya cuenta con un Id y no se puede sustituir.");
                }
            }
        }

        /// <summary>
        /// Obtiene el nombre de la tabla en la base de datos que representa este objeto.
        /// </summary>
        protected abstract string NombreDeLaTabla
        {
            get;
        }

        /// <summary>
        /// Obtiene el diccionario que contiene los valores del objeto indizados según la columna en la tabla de la base de datos.
        /// </summary>
        protected abstract Dictionary<string, Npgsql.NpgsqlParameter> ParametrosParaConsulta
        {
            get;
        }

        /// <summary>
        /// Obtiene o establece el objeto BaseDeDatos que se usa en las consultas. Asignar NULL si se quiere abrir una nueva conexión. Nota: Asignar NULL no cerrará la conexión anterior.
        /// </summary>
        public BaseDeDatos ConexionDB
        {
            get
            {
                if (this._ConexionDB == null)
                {
                    this._ConexionDB = new BaseDeDatos();
                }

                return this._ConexionDB;
            }
            set
            {
                this.ConexionDB = value;
            }
        }

        protected abstract string NombreDelPrimaryKeyDeLaTabla
        {
            get;
        }

        /// <summary>
        /// Obtiene un valor que indica si existen datos no almacenados en este objeto. Nota: Este valor NO indica si hay objetos dependientes no guardados.
        /// </summary>
        /// <value>TRUE si hay datos que guardar, de lo contrario FALSE.</value>
        public bool HayQueGuardarCambios
        {
            get
            {
                if (this._SeHaModificado || this.Id == string.Empty)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// Realiza una consulta SELECT a la base de datos y devuelve los objetos obtenidos.
        /// </summary>
        /// <remarks>Este método depende de la implementación del get de la propiedad "StringDeSelect" y del método "ConvertirResultadoEnInstancias".</remarks>
        /// <param name="where">Condiciones del tipo WHERE para la consulta.</param>
        /// <param name="orderBy">Tipo de ordenación de la consulta.</param>
        /// <param name="limit">Límites de la consulta.</param>
        /// <returns>Array de objetos resultantes por la consulta.</returns>
        public static T[] Obtener<T>(string where, string orderBy, string limit) where T : Modelo
        {
            return Modelo.Obtener<T>(new BaseDeDatos(), where, orderBy, limit);
        }

        /// <summary>
        /// Realiza una consulta SELECT a la base de datos y devuelve los objetos obtenidos.
        /// </summary>
        /// <param name="ConexionDB">Conexión a utilizar durante la obtención.</param>
        /// <param name="where">Condiciones del tipo WHERE para la consulta.</param>
        /// <param name="orderBy">Tipo de ordenación de la consulta.</param>
        /// <param name="limit">Límites de la consulta.</param>
        /// <returns>Array de objetos resultantes por la consulta.</returns>
        public static T[] Obtener<T>(BaseDeDatos ConexionDB, string where, string orderBy, string limit) where T : Modelo
        {
            Type tipo = typeof(T);
            PropertyInfo propiedad = tipo.GetProperty("StringDeSelect");
            if (propiedad == null)
            {
                throw new System.NotImplementedException("La clase " + tipo.ToString() + " no cuenta con la implementación de la propiedad estática StringDeSelect.");
            }

            MethodInfo metodo = tipo.GetMethod("ConvertirResultadoEnInstancias");
            if (metodo == null)
            {
                throw new System.NotImplementedException("La clase " + tipo.ToString() + " no cuenta con la implementación de la función estática ConvertirResultadoEnInstancias.");
            }

            ParameterInfo[] parametros = metodo.GetParameters();
            if (parametros.Length != 2)
                throw new InvalidOperationException("La función estática ConvertirResultadoEnInstancias debe tener 2 argumentos, un List<Dictionary<string, object>> y un Base_de_datos.BaseDeDatos en ese orden.");


            string query = propiedad.GetGetMethod().Invoke(null, null).ToString();// Obtiene el StringDeSelect

            where = where.Trim();
            orderBy = orderBy.Trim();
            limit = limit.Trim();

            if (where != string.Empty)
                where = " AND (" + where + ")";

            if (orderBy != string.Empty)
                orderBy = " ORDER BY " + orderBy;

            if (limit != string.Empty)
                limit = " LIMIT " + limit;

            query.Replace(":where:", where);
            query.Replace(":orderBy:", orderBy);
            query.Replace(":limit:", limit);

            ConexionDB.Conectar();
            List<Dictionary<string, object>> dr = ConexionDB.EjecutarQuery(query);
            T[] resultado = (T[])metodo.Invoke(null, new object[] { dr, ConexionDB });
            ConexionDB.Desconectar();
            return resultado;
        }

        /// <summary>
        /// Guarda el objeto en la base de datos. Si no tiene Id realiza un INSERT, de lo contrario realiza un UPDATE.
        /// </summary>
        public virtual void Guardar()
        {
            this.Guardar(false);
        }

        /// <summary>
        /// Guarda el objeto en la base de datos. Si no tiene Id realiza un INSERT, de lo contrario realiza un UPDATE.
        /// </summary>
        /// <param name="mantenerConexionAbierta">TRUE para mantener la conexión ConexionDB abierta al finalizar, de lo contrario FALSE.</param>
        public virtual void Guardar(bool mantenerConexionAbierta)
        {
            if (this.HayQueGuardarCambios)
            {// Este objeto necesita ser guardado
                string query = string.Empty;
                Dictionary<string, NpgsqlParameter> parametrosParaConsulta = this.ParametrosParaConsulta;

                this.ConexionDB.Parametros.Clear();

                if (this.Id == string.Empty)
                {// Prepara un INSERT
                    string cols = string.Empty;
                    foreach (string key in parametrosParaConsulta.Keys)
                    {
                        cols += key + ", ";
                        query += parametrosParaConsulta[key].ParameterName + ", ";
                        this.ConexionDB.Parametros.Add(parametrosParaConsulta[key]);
                    }
                    parametrosParaConsulta[this.NombreDelPrimaryKeyDeLaTabla].Value = Guid.NewGuid().ToString();// Nuevo UUID


                    if (parametrosParaConsulta.Count > 0)
                    {// Quita los ", " del final
                        cols.Substring(0, cols.Length - 2);
                        query.Substring(0, cols.Length - 2);
                    }

                    query = "INSERT INTO " + this.NombreDeLaTabla + " (" + cols + ") VALUES(" + query + ");";
                }
                else
                {// Prepara un UPDATE
                    foreach (string key in parametrosParaConsulta.Keys)
                    {
                        if (key != this.NombreDelPrimaryKeyDeLaTabla)
                        {
                            query += this.NombreDeLaTabla + "." + key + " = " + parametrosParaConsulta[key].ParameterName + ", ";
                            this.ConexionDB.Parametros.Add(parametrosParaConsulta[key]);
                        }
                    }

                    if (parametrosParaConsulta.Count > 0)
                    {// Quita los ", " del final
                        query.Substring(0, query.Length - 2);
                    }

                    query = "UPDATE " + this.NombreDeLaTabla + " SET " + query +
                        " WHERE " + this.NombreDeLaTabla + "." + this.NombreDelPrimaryKeyDeLaTabla + " = " +
                        parametrosParaConsulta[this.NombreDelPrimaryKeyDeLaTabla].ParameterName + ";";
                }

                this.ConexionDB.Conectar();
                this.ConexionDB.EjecutarNonQuery(query);
                if (this.Id == string.Empty)
                {// Asigna el nuevo UUID
                    this.Id = (string)parametrosParaConsulta[this.NombreDelPrimaryKeyDeLaTabla].Value;
                }
            }
            
            this.GuardarObjetosDependientes();
            
            if (!mantenerConexionAbierta)
                this.ConexionDB.Desconectar();
        }

        /// <summary>
        /// Guarda los objetos dependientes de este objeto.
        /// </summary>
        protected virtual void GuardarObjetosDependientes()
        {
            PropertyInfo[] propiedades = this.GetType().GetProperties();
            foreach (PropertyInfo propiedad in propiedades)
            {
                object valor = propiedad.GetValue(this, null);
                if (valor != null)
                {
                    if (valor is IEnumerable)
                    {// Es una lista o un array, en general algo por lo que puedo iterar
                        IEnumerable enumerable = (IEnumerable)valor;
                        bool esModelo = false;
                        foreach (object primero in enumerable)
                        {
                            esModelo = primero is Modelo;
                            break;
                        }

                        if (esModelo)
                        {
                            foreach (Modelo m in enumerable)
                            {// Relación uno a muchos, es decir, el objeto "m" tiene la llave foránea de este objeto (this) en su base de datos.
                                m.ConexionDB = this.ConexionDB;// Reutiliza la conexión
                                m.Guardar(true);
                                this.GuardarRelacionesEnLaTabla(this, m);
                                m.ConexionDB = null;
                            }
                        }
                    }
                    else if (valor is Modelo)
                    {// Relación 1 a 1, es decir, este objeto tiene la llave foránea de "valor" en la base de datos.
                        Modelo m = ((Modelo)valor);
                        m.ConexionDB = this.ConexionDB;// Reutiliza la conexión
                        m.Guardar(true);
                        this.GuardarRelacionesEnLaTabla(m, this);
                        m.ConexionDB = null;
                    }
                }
            }
        }

        /// <summary>
        /// Guarda llaves foráneas en la tabla de los objetos dependientes.
        /// </summary>
        /// <param name="objetoForaneo">Objeto foráneo cuya llave primaria se quiere almacenar.</param>
        /// <param name="objetoDependiente">Objeto que en su tabla de la base de datos contiene la llave foránea del objetoForaneo.</param>
        private void GuardarRelacionesEnLaTabla(Modelo objetoForaneo, Modelo objetoDependiente)
        {
            string query = "UPDATE " + objetoDependiente.NombreDeLaTabla + " SET " +
                objetoDependiente.NombreDeLaTabla + "." + objetoForaneo.NombreDelPrimaryKeyDeLaTabla + " = :foranea WHERE " +
                objetoDependiente.NombreDeLaTabla + "." + objetoDependiente.NombreDelPrimaryKeyDeLaTabla + " = :uuid";

            this.ConexionDB.Parametros.Clear();
            this.ConexionDB.Parametros.Add(new NpgsqlParameter(":foranea", objetoForaneo.Id));
            this.ConexionDB.Parametros.Add(new NpgsqlParameter(":uuid", objetoDependiente.Id));

            this.ConexionDB.Conectar();
            this.ConexionDB.EjecutarNonQuery(query);
        }
    }
}
