using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JsonFx.Json;
using UnityEngine;

namespace Comunicacion_JSON
{

    public class RequestEventArgs : System.EventArgs 
    {

        private object jsonObject;
        /// <summary>
        /// Objeto JSON recibido como respuesta del servidor.
        /// </summary>
        public object JsonObject
        {
            get { return this.jsonObject; }
        }

        private string error;
        /// <summary>
        /// Obtiene el mensaje de error si es que hubo un error durante la descarga.
        /// </summary>
        public string Error
        {
            get { return this.error; }
        }

        private byte[] bytes;
        /// <summary>
        /// Obtiene el contenido del Request como un arreglo de bytes.
        /// </summary>
        public byte[] Bytes
        {
            get { return this.bytes; }
        }

        private string text;
        /// <summary>
        /// Obtiene el contenido del Request como string.
        /// </summary>
        public string Text
        {
            get { return this.text; }
        }

        public RequestEventArgs(UnityEngine.WWW request)
        {
            this.error = request.error;
            this.text = request.text;
            this.bytes = request.bytes;

            if (string.IsNullOrEmpty(this.error) && !string.IsNullOrEmpty(this.text))
            {
                JsonReader jsonReader = new JsonReader();
                this.jsonObject = jsonReader.Read(this.text);
            }
        }
    }

    /// <summary>
    /// Capa de Modelo que utiliza un servidor externo para almacenar los datos. Las transacciones de datos son realizadas mediante JSON.
    /// </summary>
    public abstract class Modelo : IToJsonFx
    {
        #region Campos privados

        protected bool _SeHaModificado = false;

        /// <summary>
        /// Url del servidor que responderá a las peticiones.
        /// </summary>
        private const string Servidor = "";//@"http://localhost:1337";

        /// <summary>
        /// Prefijo usado para acceder a las rutas de la API.
        /// </summary>
        private const string prefijoApi = "/api/";
        
        #endregion


        #region Propiedades

        protected string _Id = string.Empty;
        /// <summary>
        /// Obtiene el Id de este objeto. Devuelve un string vacío si aún no existe en la base de datos.
        /// </summary>
        public string id
        {
            get
            {
                return this._Id;
            }
            protected set 
            {
                if (this.id != value)
                {
                    if (string.IsNullOrEmpty(this._Id))
                    {
                        this._Id = value;
                    }
                    else
                    {
                        throw new InvalidOperationException("Una vez que el ID ha sido asignado ya no puede ser modificado.");
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si existen datos no almacenados en este objeto. Nota: Este valor NO indica si hay objetos dependientes no guardados.
        /// </summary>
        /// <value>TRUE si hay datos que guardar, de lo contrario FALSE.</value>
        public bool HayQueGuardarCambios
        {
            get
            {
                if (this._SeHaModificado || this.id == string.Empty)
                    return true;

                return false;
            }
        }

        /// <summary>
        /// [Legacy] No usar esta propiedad. Obtiene o establece el objeto BaseDeDatos que se usa en las consultas. Asignar NULL si se quiere abrir una nueva conexión. Nota: Asignar NULL no cerrará la conexión anterior.
        /// </summary>
        [JsonIgnoreAttribute]
        public Base_de_datos.BaseDeDatos ConexionDB
        {
            get { return null; }
            set { return; }
        }

        #endregion


        #region Métodos públicos de la clase

        /// <summary>
        /// [Legacy] Usar el método Guardar(). Guarda el objeto en la base de datos. Si no tiene Id realiza un INSERT, de lo contrario realiza un UPDATE.
        /// </summary>
        /// <param name="mantenerConexionAbierta">[No aplica] Valor no usado.</param>
        [Obsolete("Usar otra sobrecarga de éste mismo método.")]
        public virtual void Guardar(bool mantenerConexionAbierta)
        {
            this.Guardar();
        }

        /// <summary>
        /// Guarda el objeto en la base de datos. Si no tiene Id realiza un INSERT, de lo contrario realiza un UPDATE.
        /// </summary>
        public virtual void Guardar()
        {
            this.Guardar(null);
        }

        /// <summary>
        /// Guarda el objeto en la base de datos. Si no tiene Id realiza un INSERT, de lo contrario realiza un UPDATE.
        /// </summary>
        /// <param name="Callback">Callback a ejecutar una vez que haya finalizado. Recibe un string con el mensaje de error, en caso de que exista alguno.</param>
        public virtual void Guardar(Action<string> Callback)
        {
            if (this.HayQueGuardarCambios)
            {
                string ruta = this.GetType().Name.ToLower() + "/guardar";
                string data = this.toJSON();

                Action<RequestEventArgs> del = (args) => 
                {
                    if (string.IsNullOrEmpty(args.Error))
                    {
                        this._SeHaModificado = false;
                        Dictionary<string, object> respuesta = args.JsonObject as Dictionary<string, object>;
                     
                        if (string.IsNullOrEmpty(this.id) && respuesta != null)
                            this.id = respuesta["id"] as string;
                    }

                    if (Callback != null)
                    {
                        Callback(args.Error);
                    }
                };

                Modelo.JsonRequest(ruta, data, del);
            }
            else
            {
                if (Callback != null)
                {
                    Callback(string.Empty);
                }
            }
        }

        /// <summary>
        /// [Legacy] Usar el método Obtener<T>() que no devuelve nada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ConexionDB">[Legacy] Valor no usado.</param>
        /// <param name="where">Objeto JSON con condiciones para la consulta según la especificación de SailsJs.</param>
        /// <param name="orderBy">Tipo de ordenación de la consulta.</param>
        /// <param name="limit">Límites de la consulta.</param>
        /// <returns>Array de objetos resultantes por la consulta.</returns>
        [Obsolete("Usar otra sobrecarga de éste mismo método.", true)]
        public static T[] Obtener<T>(Base_de_datos.BaseDeDatos ConexionDB, string where, string orderBy, string limit) where T : Modelo
        {
            return Modelo.Obtener<T>(where, orderBy, limit);
        }

        /// <summary>
        /// [Legacy] Usar el método Obtener<T>() que no devuelve nada.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where">Objeto JSON con condiciones para la consulta según la especificación de SailsJs.</param>
        /// <param name="orderBy">Tipo de ordenación de la consulta.</param>
        /// <param name="limit">Límites de la consulta.</param>
        /// <returns>Array de objetos resultantes por la consulta.</returns>
        [Obsolete("Usar otra sobrecarga de éste mismo método.", true)]
        public static T[] Obtener<T>(string where, string orderBy, string limit) where T : Modelo
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Realiza una consulta find() a la base de datos y devuelve los objetos obtenidos.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto que se desea obtener. Debe ser un objeto derivado de Modelo.</typeparam>
        /// <param name="where">Objeto JSON con condiciones para la consulta según la especificación de SailsJs.</param>
        /// <param name="orderBy">Tipo de ordenación de la consulta.</param>
        /// <param name="limit">Límites de la consulta.</param>
        /// /// <param name="skip">Skip según la especificación de SailsJs.</param>
        /// <param name="Callback">Acción  a realizar cuando finalice la solicitud.</param>
        /// <remarks>Este método requiere que la función estática ConvertirJsonEnArray exista en la clase.</remarks>
        public static void Obtener<T>(string where, string orderBy, string limit, string skip, Action<T[]> Callback) where T : Modelo
        {
            where = "\"where\":{" + where + "},";

            if (orderBy != string.Empty)
                orderBy = "\"sort\":" + orderBy + ",";

            if (limit != string.Empty)
                limit = "\"limit\":" + limit + ",";

            if (skip != string.Empty)
                skip = "\"skip\":" + skip + ",";

            string jsonData = where + orderBy + limit + skip;
            if (jsonData[jsonData.Length - 1] == ',')// Quita la última ´,´ del string
                jsonData = "{" + jsonData.Substring(0, jsonData.Length - 1) + "}";
            else
                jsonData = "{" + jsonData + "}";

            Action<RequestEventArgs> conversion = (args) =>
            {
                T[] array = Modelo.invocarMetodo<T>("ConvertirJsonEnArray", args.JsonObject) as T[];
                if (Callback != null)
                    Callback(array);
            };
            
            Modelo.JsonRequest(prefijoApi + typeof(T).Name.ToLower(), jsonData, conversion);
        }

        private static object invocarMetodo<T>(string nombre, params object[] args)
        {
            if(args == null)
                args =  new object[0];

            return Modelo.ValidarMetodo<T>(nombre, args).Invoke(null, args);
        }

        /// <summary>
        /// Verifica que el método especificado exista en la definiciíon de la clase.
        /// </summary>
        /// <typeparam name="T">Clase a verificar.</typeparam>
        /// <param name="nombre">Nombre del método que se desea verificar.</param>
        /// <param name="args">Argumentos que debe recibir el método.</param>
        private static MethodInfo ValidarMetodo<T>(string nombre, object[] args)
        {
            Type tipo = typeof(T);
            MethodInfo metodo = typeof(T).GetMethod(nombre);
            if (metodo == null)
            {
                throw new System.NotImplementedException("La clase " + tipo.ToString() + " no cuenta con la implementación de la función estática " + nombre + ".");
            }

            ParameterInfo[] parametros = metodo.GetParameters();
            if (parametros.Length != args.Length)
            {
                throw new InvalidOperationException("La función estática " + nombre + " debe recibir " + args.Length + " argumentos.");
            }

            for (int i = 0; i < parametros.Length; i++)
            {
                if (!parametros[i].ParameterType.IsAssignableFrom(args[i].GetType()))
                {
                    throw new InvalidOperationException("El argumento cuyo índice es " + i + " de la función estática " + nombre + " debe ser del tipo " + parametros[i].ParameterType + ".");
                }
            }

            return metodo;
        }

        /// <summary>
        /// Realiza un Request al servidor.
        /// </summary>
        /// <param name="url">Ruta relativa al servidor.</param>
        /// <param name="Callback">Acción  a realizar cuando finalice la solicitud.</param>
        public static void simpleGetRequest(string url, Action<RequestEventArgs> Callback)
        {
            Modelo.JsonRequest(url, string.Empty, Callback);
        }

        /// <summary>
        /// Realiza un Request al servidor con el objeto JSON dado.
        /// </summary>
        /// /// <param name="url">Ruta relativa al servidor.</param>
        /// <param name="JsonData">Objeto JSON a enviar por POST.</param>
        /// <param name="Callback">Acción  a realizar cuando finalice la solicitud.</param>
        public static void JsonRequest(string url, string JsonData, Action<RequestEventArgs> Callback)
        {
            StaticCoroutine.IniciarCoroutine(realizarRequest(Modelo.producirJsonRequest(url, JsonData), Callback));
        }

        /// <summary>
        /// Construye un objeto UnityEngine.WWW listo para ser usado en alguna co-rutina.
        /// </summary>
        /// /// <param name="url">Ruta relativa al servidor.</param>
        /// <param name="JsonData">Objeto JSON a enviar por POST.</param>
        /// <returns>Objeto UnityEngine.WWW listo para ser usado en alguna co-rutina.</returns>
        private static UnityEngine.WWW producirJsonRequest(string url, string JsonData)
        {
            WWWForm form = new WWWForm();
            form.headers["Content-Type"] = "application/json";
            
            if (JsonData != string.Empty)
                form.AddField("data", JsonData);

            if (url[0] != '/')// Nos aseguramos que inicie con '/'
                url = "/" + url;

            return new WWW(Modelo.Servidor + url, form);
        }

        /// <summary>
        /// Realiza un request a lo largo de diferentes frames.
        /// </summary>
        /// <param name="request">Request a realizar.</param>
        /// <param name="Callback">Acción  a realizar cuando finalice la solicitud.</param>
        private static IEnumerator realizarRequest(UnityEngine.WWW request, Action<RequestEventArgs> Callback)
        {
            yield return request;

            if (Callback != null)
                Callback(new RequestEventArgs(request));
        }

        /// <summary>
        /// Convierte el objeto en un string con formato JSON.
        /// </summary>
        /// <returns>Representación JSON de este objeto.</returns>
        public virtual string toJSON()
        {
            JsonWriter jsonWriter = new JsonWriter();
            return jsonWriter.Write(this);
        }

        /// <summary>
        /// Transforma una cadena de formato JSON en un objeto de un tipo dado.
        /// </summary>
        /// <typeparam name="T">Tipo del objeto.</typeparam>
        /// <param name="json">Cadena en formato JSON.</param>
        /// <returns>Instancia de dicho objeto</returns>
        public static T FromJson<T>(string json) where T : Comunicacion_JSON.Modelo
        {
            JsonReader jsonReader = new JsonReader();
            return jsonReader.Read<T>(json);
        }

        #endregion
    }
}
