using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Entrenamiento.Estadisticas;
using JsonFx.Json;

public class EscenaInicialController : MonoBehaviour
{
    public TextMesh Texto;
    private bool DebeHaberCargadoLaEscena = false;

    private void Start()
    {
        StartCoroutine(this.MensajeDeTiempoExcedido());
        Application.ExternalCall("UnityAppIniciada", string.Empty);







        /*          Pruebas locales
         * 
         * No olvides Cambiar la ruta en la clase Modelo            
         */

        //this.Init("{\"action\":\"EditorDeEscenarios\", " +
        //    "\"data\":{\"nombre\":\"José Pérez López\",\"escenario_id\":\"5383a3603634418c101d3fbf\"}}");

        //this.Init("{\"action\":\"RealizarSesion\", " +
        //    "\"data\":{\"escenario_id\":\"53c2d11881d3990414c6034e\", \"modelo_de_aeronave\":0}}");

        //this.Init("{\"action\":\"ReproductorDeSesion\", " +
        //    "\"data\":{\"sesion_de_entrenamiento_id\":\"53b462209f20c0dc05ca0e52\", \"entrenador_id\":\"ID de Prueba\"}}");
    }

    /// <summary>
    /// Corutina que muestra un mensaje cuando se excede el tiempo límite en esta escena.
    /// </summary>
    /// <returns></returns>
    IEnumerator MensajeDeTiempoExcedido()
    {
        yield return new WaitForSeconds(4);

        if (this.DebeHaberCargadoLaEscena && !Application.isLoadingLevel)
            this.Texto.text = "Algo salió horriblemente mal. =(";// Se intentó cargar una escena que no existe.
        else
        {
            this.Texto.text = "Se está tardando...\nEsperemos un poco más. ;)";
            yield return new WaitForSeconds(7);
            this.Texto.text = "Ya tardó demasiado...\n\nSeguramente algo salió mal. =(";
        }
    }

    /// <summary>
    /// Función de inicialización que se desencadena a través de un mensaje del navegador.
    /// </summary>
    /// <param name="json">String JSON con los datos recibidos del navegador.</param>
    public void Init(string json)
    {
        JsonReader jsonReader = new JsonReader();
        Dictionary<string, object> data = jsonReader.Read<Dictionary<string, object>>(json);
        
        if (data.ContainsKey("_csrf"))
            EspacioGlobal.Sesion._csrf = data["_csrf"] as string;

        if (data.ContainsKey("data"))
            EspacioGlobal.Sesion.Data = data["data"] as Dictionary<string, object>;


        switch (data["action"] as string)
        {
            case "EditorDeEscenarios":
                data = EspacioGlobal.Sesion.Data as Dictionary<string, object>;
                string escenario_id = data["escenario_id"] as string;
                if (escenario_id.ToLower() != "nuevo")
                {// Obtención del Escenario
                    System.Action<Escenario[]> cb;
                    cb = (array) =>
                    {
                        EspacioGlobal.Sesion.Data = array[0];// Asignación del escenario.
                        Application.LoadLevel("EditorDeEscenarios");
                        DebeHaberCargadoLaEscena = true;
                    };
                    Comunicacion_JSON.Modelo.Obtener<Escenario>("\"id\":\"" + escenario_id + "\"", "", "", "", cb);
                }
                else
                {
                    Application.LoadLevel("EditorDeEscenarios");
                    DebeHaberCargadoLaEscena = true;
                }
                break;

            case "RealizarSesion":
                data = EspacioGlobal.Sesion.Data as Dictionary<string, object>;
                {
                    System.Action<Escenario[]> cb;
                    cb = (array) =>
                    {
                        object[] args = new object[2];
                        args[0] = array[0];// Asignación del escenario.
                        args[1] = System.Convert.ToInt32(data["modelo_de_aeronave"]);// modelo_de_aeronave

                        EspacioGlobal.Sesion.Data = args;
                        Application.LoadLevel("RealizarSesion");
                        DebeHaberCargadoLaEscena = true;
                    };
                    Comunicacion_JSON.Modelo.Obtener<Escenario>("\"id\":\"" + data["escenario_id"] + "\"", "", "", "", cb);
                }
                break;

            case "ReproductorDeSesion":
                data = EspacioGlobal.Sesion.Data as Dictionary<string, object>;
                {
                    System.Action<SesionDeEntrenamiento[]> cb;
                    cb = (array) =>
                    {
                        object[] args = new object[2];
                        args[0] = array[0];// Asignación de la sesión de entrenamiento.
                        args[1] = data["entrenador_id"];// ID de quien evalúa.

                        EspacioGlobal.Sesion.Data = args;
                        Application.LoadLevel("ReproductorDeSesion");
                        DebeHaberCargadoLaEscena = true;
                    };
                    Comunicacion_JSON.Modelo.Obtener<SesionDeEntrenamiento>("\"id\":\"" + data["sesion_de_entrenamiento_id"] + "\"", "", "", "", cb);
                }
                break;

            default:
                EspacioGlobal.Sesion.Data = data;
                Application.LoadLevel(data["action"] as string);
                DebeHaberCargadoLaEscena = true;
                break;
        }
    }
}

namespace EspacioGlobal
{
    public static class Sesion
    {
        public static string _csrf = string.Empty;

        private static object data = null;
        /// <summary>
        /// Obtiene o establece un objeto que puede ser accesado a través de diferentes partes de la aplicación. Nota: Una vez obtenido el objeto (get), entonces Data se quedará en NULL hasta la próxima asignación.
        /// </summary>
        public static object Data 
        {
            get
            {
                object tmp = data;
                data = null;
                return tmp;
            }
            set 
            {
                data = value;
            }
        }
    }
}