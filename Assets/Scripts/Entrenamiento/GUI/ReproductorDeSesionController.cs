using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Entrenamiento.Estadisticas;

namespace Entrenamiento.GUI
{
    public class ReproductorDeSesionController : MonoBehaviour
    {
        #region Campos públicos

        /// <summary>
        /// Mesaje que se muestra a pantalla completa.
        /// </summary>
        public GameObject MensajeAPantallaCompleta;

        /// <summary>
        /// Objeto que contiene la ventana de evaluación y sus componentes.
        /// </summary>
        public Transform VentanaDeEvaluacion;

        /// <summary>
        /// Combo box con la claificación de la sesión.
        /// </summary>
        public Interfaz.Utilities.ComboBoxBase cmbCalificacion;

        /// <summary>
        /// Text area con las observaciones de la sesión.
        /// </summary>
        public Interfaz.Utilities.AreaDeTexto txtObservacionesSesion;

        /// <summary>
        /// Botón que cierra la ventana de evaluación.
        /// </summary>
        public Interfaz.Utilities.BotonController btnCerrarVentanaDeEvaluacion;

        /// <summary>
        /// Botón que guarda los cambios en la evaluación.
        /// </summary>
        public Interfaz.Utilities.BotonController btnGuardarEvaluacion;

        /// <summary>
        /// Botón que abre la ventana de evaluación.
        /// </summary>
        public Interfaz.Utilities.BotonController btnEvaluar;

        /// <summary>
        /// Botón que permite guardar las observaciones hacia el Alumno.
        /// </summary>
        public Interfaz.Utilities.BotonController btnGuardarObservaciones;

        /// <summary>
        /// Interfaz para el reproductor de sesión lógico.
        /// </summary>
        public ReproductorDeSesion.ReproductorGUIController reproductorGUIController;

        /// <summary>
        /// Objeto que contiene los instrumentos y donde se instanciarán los instrumentos según el modelo de la aeronave.
        /// </summary>
        public Transform PanelDeInstrumentosGUI;

        /// <summary>
        /// Objeto que contiene los interruptores de la aeronave.
        /// </summary>
        public Transform PanelDeInterruptoresGUI;

        /// <summary>
        /// Los diferentes paneles de los distintos modelos de aeronaves. Nota: El índice de estos deben coincidir con el número de la enumeración ModelosDeHelicoptero.
        /// </summary>
        public Transform[] PanelesDeAeronavesPrefabs;

        /// <summary>
        /// Etiqueta con el nombre del alumno.
        /// </summary>
        public TextMesh lblNombreDelAlumno;

        /// <summary>
        /// Etiqueta con la fecha de realización de la sesión.
        /// </summary>
        public TextMesh lblFecha;

        /// <summary>
        /// Caja de texto con las observaciones hacia el alumno.
        /// </summary>
        public Interfaz.Utilities.AreaDeTexto txtObservaciones;

        /// <summary>
        /// 
        /// </summary>
        public Interfaz.Utilities.BotonController btnRegresar;

        #endregion


        #region Campos privados

        /// <summary>
        /// Variable usada para el fix de visualización en el mensaje a pantalla completa. Este valor indica si se estaba mostrando 
        /// la ventana de evaluación antes de activar el mensaje.
        /// </summary>
        private bool VentanaEvaluacionEstabaActiva = false;

        /// <summary>
        /// Sesión de entrenamiento a reproducir.
        /// </summary>
        private SesionDeEntrenamiento sesionDeEntrenamiento;

        /// <summary>
        /// Reproductor de sesión lógico.
        /// </summary>
        private Estadisticas.ReproductorDeSesion reproductorDeSesion;

        #endregion


        #region Eventos Unity

        private void Awake()
        {
            // Obtenemos los datos de la sesión de la aplicación.
            object[] data = (object[])EspacioGlobal.Sesion.Data;
            this.sesionDeEntrenamiento = (SesionDeEntrenamiento)data[0];

            // Inicializaciones
            this.lblNombreDelAlumno.text = this.sesionDeEntrenamiento.Piloto.Nombre;
            this.lblFecha.text = sesionDeEntrenamiento.Fecha.ToShortDateString();
            this.InstanciarPanelSegunModelo(this.sesionDeEntrenamiento.ModeloDeAeronave);

            // Eventos
            this.btnRegresar.Click += this.btnRegresar_Click;
            this.btnGuardarObservaciones.Click += this.btnGuardarObservaciones_Click;
            this.txtObservaciones.AlCambiarTexto += this.observacionesTextArea_AlCambiarTexto;
            this.btnEvaluar.Click += this.btnEvaluar_Click;
            this.btnCerrarVentanaDeEvaluacion.Click += this.btnCerrarVentanaDeEvaluacion_Click;
            this.btnGuardarEvaluacion.Click += this.btnGuardarEvaluacion_Click;
            this.txtObservacionesSesion.AlCambiarTexto += this.txtObservacionesSesion_AlCambiarTexto;
            this.cmbCalificacion.SelectedIndexChange += this.cmbCalificacion_SelectedIndexChange;
        }

        private void Start()
        {
            this.txtObservaciones.Texto = this.sesionDeEntrenamiento.Piloto.Observaciones;

            Instrumento[] instrumentos;
            Interruptor[] interruptores;

            // Se obtienen los instrumentos
            Instrumentos.InstrumentoGUIController[] instrumentosGUI =
                this.PanelDeInstrumentosGUI.GetComponentsInChildren<Instrumentos.InstrumentoGUIController>();
            instrumentos = new Instrumento[instrumentosGUI.Length];
            for (int i = 0; i < instrumentos.Length; i++)
            {
                instrumentos[i] = instrumentosGUI[i].Instrumento;
            }

            // Se obtienen los interruptores
            Interruptores.InterruptorGuiController[] interruptoresGUI =
                this.PanelDeInterruptoresGUI.GetComponentsInChildren<Interruptores.InterruptorGuiController>();
            interruptores = new Interruptor[interruptoresGUI.Length];
            for (int i = 0; i < interruptores.Length; i++)
            {
                interruptores[i] = interruptoresGUI[i].Interruptor;
            }

            // Se instancia e inicializa el ReproductorDeSesion
            this.reproductorDeSesion = new Estadisticas.ReproductorDeSesion();
            this.reproductorDeSesion.Instrumentos = instrumentos;
            this.reproductorDeSesion.Interruptores = interruptores;
            this.reproductorDeSesion.SesionDeEntrenamiento = this.sesionDeEntrenamiento;

            // Se asigna al reproductor GUI
            this.reproductorGUIController.ReproductorDeSesion = this.reproductorDeSesion;

            if (!string.IsNullOrEmpty(this.sesionDeEntrenamiento.Observaciones))
            {// Si existen observaciones entonces sobreescribimos el texto por default
                this.txtObservacionesSesion.Texto = this.sesionDeEntrenamiento.Observaciones;
            }

            foreach (Calificaciones c in (Calificaciones[])System.Enum.GetValues(typeof(Calificaciones)))
            {// Rellenamos el comboBox de calificaciones
                this.cmbCalificacion.AgregarElemento(c);
            }

            // Asignamos la calificación que tiene la sesión.
            this.cmbCalificacion.SelectedIndex = 
                this.cmbCalificacion.IndexOf((Calificaciones)this.sesionDeEntrenamiento.CalificacionGeneral);

            // Se oculta la ventana porque en este punto ya se debe haber inicializado todo lo necesario para su funcionamiento.
            this.MostrarVentanaDeEvaluacion(false);
        }

        #endregion


        #region Eventos de la interfaz

        private void cmbCalificacion_SelectedIndexChange(object sender, System.EventArgs e)
        {
            this.sesionDeEntrenamiento.CalificacionGeneral = System.Convert.ToUInt16(cmbCalificacion.SelectedItem.Valor);
        }

        private void txtObservacionesSesion_AlCambiarTexto(object sender, System.EventArgs e)
        {
            this.sesionDeEntrenamiento.Observaciones = this.txtObservacionesSesion.Texto;
        }

        private void btnGuardarEvaluacion_Click(object sender, System.EventArgs e)
        {
            this.GuardarEvaluacion();
        }

        private void btnCerrarVentanaDeEvaluacion_Click(object sender, System.EventArgs e)
        {
            this.MostrarVentanaDeEvaluacion(false);
        }

        private void btnEvaluar_Click(object sender, System.EventArgs e)
        {
            this.MostrarVentanaDeEvaluacion(true);
        }

        private void observacionesTextArea_AlCambiarTexto(object sender, System.EventArgs e)
        {
            this.sesionDeEntrenamiento.Piloto.Observaciones = this.txtObservaciones.Texto;
        }

        private void btnGuardarObservaciones_Click(object sender, System.EventArgs e)
        {
            this.GuardarObservacionesDelAlumno();
        }

        private void btnRegresar_Click(object sender, System.EventArgs e)
        {
            Application.ExternalCall("Regresar", string.Empty);
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Muestra u oculta la ventana de evaluación.
        /// </summary>
        /// <param name="mostrar">TRUE para mostrar, de lo contrario FALSE.</param>
        private void MostrarVentanaDeEvaluacion(bool mostrar)
        {
            this.VentanaDeEvaluacion.gameObject.SetActive(mostrar);
        }

        /// <summary>
        /// Guarda los cambios en la evaluación de esta sesión.
        /// </summary>
        private void GuardarEvaluacion()
        {
            System.Action<Comunicacion_JSON.RequestEventArgs> cb = req =>
            {
                if (string.IsNullOrEmpty(req.Error))
                {
                    if ((bool)((Dictionary<string, object>)req.JsonObject)["Guardado"])
                    {
                        this.MensajeAPantallaCompleta.guiText.text = "¡Evaluación guardada!.\n\nRegresando, por favor espere...";
                        Application.ExternalCall("Regresar", string.Empty);
                    }
                    else
                    {
                        this.MensajeAPantallaCompleta.guiText.text = "No se pudo guardar. =(\n\nInténtelo de nuevo más tarde.";
                        StartCoroutine(this.DesvanecerMensajeAPantallaCompleta(5));
                    }
                }
                else
                {
                    this.MensajeAPantallaCompleta.guiText.text = "No se pudo guardar. =(\n\nPor favor verifique su conexión.";
                    StartCoroutine(this.DesvanecerMensajeAPantallaCompleta(5));
                }
            };
            Comunicacion_JSON.Modelo.JsonRequest("entrenador/sesiones/" + this.sesionDeEntrenamiento.id,
                "{\"Observaciones\":\"" + this.sesionDeEntrenamiento.Observaciones + 
                "\",\"CalificacionGeneral\":" + this.sesionDeEntrenamiento.CalificacionGeneral + "}", cb);

            mostrarMensajeAPantallaCompleta("Guardando.\n\nPor favor espere...");
        }

        /// <summary>
        /// Guarda las observaciones del alumno.
        /// </summary>
        private void GuardarObservacionesDelAlumno()
        {
            System.Action<Comunicacion_JSON.RequestEventArgs> cb = req =>
            {
                if (string.IsNullOrEmpty(req.Error))
                {
                    if ((bool)((Dictionary<string, object>)req.JsonObject)["Guardado"])
                    {
                        this.MensajeAPantallaCompleta.guiText.text = "¡Observaciones guardadas!";
                        StartCoroutine(this.DesvanecerMensajeAPantallaCompleta(2));
                    }
                    else
                    {
                        this.MensajeAPantallaCompleta.guiText.text = "No se pudo guardar. =(\n\nInténtelo de nuevo más tarde.";
                        StartCoroutine(this.DesvanecerMensajeAPantallaCompleta(5));
                    }
                }
                else
                {
                    this.MensajeAPantallaCompleta.guiText.text = "No se pudo guardar. =(\n\nPor favor verifique su conexión.";
                    StartCoroutine(this.DesvanecerMensajeAPantallaCompleta(5));
                }
            };
            Comunicacion_JSON.Modelo.JsonRequest("alumno/observaciones/" + this.sesionDeEntrenamiento.Piloto.Id, 
                this.sesionDeEntrenamiento.Piloto.Observaciones, cb);

            mostrarMensajeAPantallaCompleta("Guardando observaciones.\n\nPor favor espere...");
        }

        /// <summary>
        /// Instancia y coloca los instrumentos del panel según el modelo.
        /// </summary>
        /// <param name="modelo"></param>
        private void InstanciarPanelSegunModelo(ModelosDeHelicoptero modelo)
        {
            Transform instrumentos = Instantiate(this.PanelesDeAeronavesPrefabs[(int)modelo]) as Transform;
            instrumentos.parent = this.PanelDeInstrumentosGUI;
            instrumentos.localRotation = Quaternion.identity;
            instrumentos.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Muestra el mensaje a pantalla completa
        /// </summary>
        /// <param name="mensaje">Mensaje que se desea mostrar.</param>
        private void mostrarMensajeAPantallaCompleta(string mensaje)
        {
            // Fix para evitar problemas de visualización.
            this.VentanaEvaluacionEstabaActiva = this.VentanaDeEvaluacion.gameObject.activeSelf;
            this.txtObservaciones.gameObject.SetActive(false);
            this.VentanaDeEvaluacion.gameObject.SetActive(false);
            
            this.MensajeAPantallaCompleta.guiText.text = mensaje;
            this.MensajeAPantallaCompleta.SetActive(true);
        }

        /// <summary>
        /// Corutina que desvanece el MensajeAPantallaCompleta gradualmente.
        /// </summary>
        /// <param name="segundos">Segundos que deben pasar antes de desvanecerse.</param>
        /// <returns></returns>
        private IEnumerator DesvanecerMensajeAPantallaCompleta(float segundos)
        {
            yield return new WaitForSeconds(segundos);

            GUITexture Fondo = this.MensajeAPantallaCompleta.GetComponentInChildren<GUITexture>();
            Color original = this.MensajeAPantallaCompleta.guiText.color;
            Color originalFondo = Fondo.color;

            Color aux = original;
            Color auxFondo = originalFondo;

            // Fix para evitar problemas de visualización. -- Reactivación
            this.txtObservaciones.gameObject.SetActive(true);
            this.VentanaDeEvaluacion.gameObject.SetActive(this.VentanaEvaluacionEstabaActiva);

            while (aux.a > 0)
            {
                aux.a = aux.a - Time.deltaTime * 2;
                auxFondo.a = auxFondo.a - Time.deltaTime * 2;
                this.MensajeAPantallaCompleta.guiText.color = aux;
                Fondo.color = auxFondo;
                yield return null;
            }

            this.MensajeAPantallaCompleta.guiText.color = original;
            Fondo.color = originalFondo;

            this.MensajeAPantallaCompleta.SetActive(false);
        }

        #endregion
    }
}
