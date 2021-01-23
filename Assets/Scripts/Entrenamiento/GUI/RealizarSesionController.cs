using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Entrenamiento.Estadisticas;
using Entrenamiento.GUI.Instrumentos;
using Entrenamiento.GUI.Interruptores;

namespace Entrenamiento.GUI
{
    public class RealizarSesionController : MonoBehaviour
    {
        #region Campos públicos

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
        /// Mesaje que se muestra a pantalla completa cuando finaliza el escenario.
        /// </summary>
        public Transform MensajeDeFinalizacion;

        #endregion


        #region Campos privados

        private bool sesionVaAComenzar = false;

        private GrabadoraDeSesion grabadoraDeSesion;

        /// <summary>
        /// Diccionario con los objetos que simularán el comportamiento de los síntomas.
        /// </summary>
        private Dictionary<NombresDeInstrumentos, SimuladorDeSintoma> SimuladoresDeSintoma;

        #endregion


        #region Propiedades

        //private SesionDeEntrenamiento sesionActual;
        ///// <summary>
        ///// Obtiene la sesión de entrenamiento actual.
        ///// </summary>
        //public SesionDeEntrenamiento SesionActual
        //{
        //    get
        //    {
        //        return this.sesionActual;
        //    }
        //}


        private Interruptor[] interruptoresDeLaAeronave = null;
        /// <summary>
        /// Obtiene los interruptores de la aeronave.
        /// </summary>
        public Interruptor[] InterruptoresDeLaAeronave
        {
            get
            {
                if (this.interruptoresDeLaAeronave == null)
                {
                    InterruptorGuiController[] interruptores = 
                        this.PanelDeInterruptoresGUI.GetComponentsInChildren<InterruptorGuiController>();

                    this.interruptoresDeLaAeronave = new Interruptor[interruptores.Length];
                    for (int i = 0; i < this.interruptoresDeLaAeronave.Length; i++)
                    {
                        this.interruptoresDeLaAeronave[i] = interruptores[i].Interruptor;
                    }
                }

                return this.interruptoresDeLaAeronave;
            }
        }


        private Instrumento[] _InstrumentosDeLaAeronave = null;
        /// <summary>
        /// Obtiene los instrumentos de la aeronave para esta sesión.
        /// </summary>
        public Instrumento[] InstrumentosDeLaAeronave
        {
            get
            {
                if (this._InstrumentosDeLaAeronave == null)
                {// Obtenemos los instrumentos
                    SimuladorDeSintoma[] simuladores = new SimuladorDeSintoma[this.SimuladoresDeSintoma.Count];
                    this.SimuladoresDeSintoma.Values.CopyTo(simuladores, 0);
                    this._InstrumentosDeLaAeronave = new Instrumento[simuladores.Length];
                    for (int i = 0; i < this._InstrumentosDeLaAeronave.Length; i++)
                    {
                        this._InstrumentosDeLaAeronave[i] = simuladores[i].InstrumentoAfectado;
                    }
                }
                
                return this._InstrumentosDeLaAeronave;
            }
        }


        private Escenario escenario;
        /// <summary>
        /// Obtiene o establece el escenario de la sesión.
        /// </summary>
        public Escenario Escenario
        {
            get
            {
                return this.escenario;
            }
            set
            {
                if (this.escenario != value)
                {
                    if (this.escenario != null)
                    {
                        this.escenario.AlIniciarSimulacion -= this.escenario_AlIniciarSimulacion;
                        this.escenario.AlFinalizarEscenario -= this.escenario_AlFinalizarEscenario;
                    }
                    
                    this.escenario = value;

                    if (this.escenario != null)
                    {
                        this.AsignarInterruptoresALasSoluciones();
                        this.grabadoraDeSesion =
                            new GrabadoraDeSesion(this.escenario, this.interruptoresDeLaAeronave, this.InstrumentosDeLaAeronave);

                        this.escenario.AlIniciarSimulacion += this.escenario_AlIniciarSimulacion;
                        this.escenario.AlFinalizarEscenario += this.escenario_AlFinalizarEscenario;
                    }
                }
            }
        }


        /// <summary>
        /// Obtiene o establece un valor que indica si la sesión está en simulación.
        /// </summary>
        public bool EnSimulacion
        {
            get
            {
                if (this.Escenario == null)
                    return false;

                return this.Escenario.EnSimulacion;
            }
            set
            {
                if (this.Escenario == null)
                {
                    throw new System.InvalidOperationException("La simulación no puede iniciar porque no hay un escenario asignado.");
                }

                this.Escenario.EnSimulacion = value;
            }
        }


        private ModelosDeHelicoptero modeloDeAeronave = ModelosDeHelicoptero.Desconocido;
        /// <summary>
        /// Obtiene o establece el modelo de la aeronave usada en esta sesión.
        /// </summary>
        public ModelosDeHelicoptero ModeloDeAeronave
        {
            get
            {
                return this.modeloDeAeronave;
            }
            set
            {
                if (this.modeloDeAeronave != value)
                {
                    this.modeloDeAeronave = value;

                    Transform instrumentos;
                    while (this.PanelDeInstrumentosGUI.childCount > 0) 
                    {// Quitamos el panel anterior.
                        instrumentos = this.PanelDeInstrumentosGUI.GetChild(0);
                        instrumentos.parent = null;// Lo quito de este objeto porque no se destruye hasta finalizar este update.
                        Destroy(instrumentos.gameObject);
                    }



                    // Se agrega el nuevo Panel según el modelo.
                    instrumentos = Instantiate(this.PanelesDeAeronavesPrefabs[(int)this.modeloDeAeronave]) as Transform;
                    RealizarSesion.ReglasDePanelDeInstrumentos.AgregarReglasAPanel(instrumentos.gameObject, this.modeloDeAeronave);
                    instrumentos.parent = this.PanelDeInstrumentosGUI;
                    instrumentos.localRotation = Quaternion.identity;
                    instrumentos.localPosition = Vector3.zero;


                    this._InstrumentosDeLaAeronave = null;// Se quita la referencia a los instrumentos anteriores
                    this.SimuladoresDeSintoma.Clear();// Limpiamos los simuladores anteriores
                    SimuladorDeSintoma[] simuladores = this.PanelDeInstrumentosGUI.GetComponentsInChildren<SimuladorDeSintoma>();
                    foreach (SimuladorDeSintoma s in simuladores)
                    {
                        this.SimuladoresDeSintoma.Add(s.InstrumentoAfectado.Nombre, s);
                    }
                }
            }
        }
        
        #endregion


        #region Eventos relacionados con el Escenario

        private void escenario_AlIniciarSimulacion(object sender, System.EventArgs e)
        {
            this.grabadoraDeSesion.GrabacionIniciada = true;
            this.Escenario.AlIniciarEtapa += this.Escenario_AlIniciarEtapa;
            this.Escenario.AlFinalizarEtapa += this.Escenario_AlFinalizarEtapa;
        }

        private void Escenario_AlIniciarEtapa(object sender, EtapaEnEscenarioEventArgs e)
        {
            Debug.Log(e.Etapa + " iniciada!!");
            this.ActivarSimuladoresDeSintomas(this.Escenario.EtapaActual.Sintomas);
        }

        private void Escenario_AlFinalizarEtapa(object sender, EtapaEnEscenarioEventArgs e)
        {
            Debug.Log(e.Etapa + " terminada!!");
        }

        private void escenario_AlFinalizarEscenario(object sender, System.EventArgs e)
        {
            if (this.grabadoraDeSesion.GrabacionIniciada)
            {// Esperamos a que finalice la grabación.
                this.grabadoraDeSesion.AlCambiarGrabacionIniciada += grabadoraDeSesion_AlCambiarGrabacionIniciada;
            }
            else
            {
                StartCoroutine(this.GuardarGrabacion());
            }
        }

        private void grabadoraDeSesion_AlCambiarGrabacionIniciada(object sender, System.EventArgs e)
        {
            StartCoroutine(this.GuardarGrabacion());
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Reliza un cuenta regresiva y luego inicia la simulación de le sesión de entrenamiento.
        /// </summary>
        /// <param name="segundos">Cantidad de segundos en la cuenta regresiva.</param>
        /// <returns></returns>
        private IEnumerator IniciarSesionDeEntrenamiento(int segundos)
        {
            this.sesionVaAComenzar = true;
            this.MensajeDeFinalizacion.gameObject.SetActive(true);
            while (segundos > 0)
            {
                this.MensajeDeFinalizacion.guiText.text = segundos.ToString();
                yield return new WaitForSeconds(1);
                segundos--;
            }
            this.MensajeDeFinalizacion.gameObject.SetActive(false);
            this.EnSimulacion = true;
        }

        /// <summary>
        /// Inicia el proceso de guardar la sesión realizada.
        /// </summary>
        /// <returns></returns>
        private IEnumerator GuardarGrabacion()
        {
            bool guardado = false;
            this.MensajeDeFinalizacion.guiText.text = "¡Ha Finalizado!\n\nGuardando,\npor favor espere...";
            this.MensajeDeFinalizacion.gameObject.SetActive(true);
            System.Action<string> cb = (res) =>
            {
                guardado = true;
                this.MensajeDeFinalizacion.guiText.text = "¡Guardado!\n\nRegresando...";
                Application.ExternalCall("Regresar", string.Empty);
            };

            SesionDeEntrenamiento sesion = new SesionDeEntrenamiento(this.grabadoraDeSesion, ModeloDeAeronave);
            sesion.Guardar(cb);

            yield return new WaitForSeconds(5);
            if (!guardado)
            {
                this.MensajeDeFinalizacion.guiText.text = "Está tardando más de lo normal.\n\nGuardando,\npor favor espere...";
            }

            yield return new WaitForSeconds(15);
            this.MensajeDeFinalizacion.guiText.text = "Ha demorado mucho, \nquizás algo salió mal. =(\n\nPor favor verifique su conexión.";
        }

        /// <summary>
        /// Asigna los interruptores de la aeronave a las soluciones del escenario.
        /// </summary>
        private void AsignarInterruptoresALasSoluciones()
        {
            if (this.Escenario != null && this.Escenario.Etapas != null && this.InterruptoresDeLaAeronave != null)
            {
                foreach (Etapa etapa in this.escenario.Etapas)
                {
                    foreach (Solucion solucion in etapa.Soluciones)
                    {
                        solucion.InterruptoresDeLaAeronave = this.InterruptoresDeLaAeronave;
                    }
                }
            }
        }

        /// <summary>
        /// Asigna el modelo de la aeronave y el escenario a partir de datos del objeto global Sesion.
        /// </summary>
        private void InicializarDesdeDatosDeSesion()
        {
            /*  data [
             *      Escenario,
             *      ModelosDeHelicoptero
             *  ]
             */
            object[] data = (object[])EspacioGlobal.Sesion.Data;

            this.ModeloDeAeronave = (ModelosDeHelicoptero)data[1];
            this.Escenario = (Escenario)data[0];
        }

        /// <summary>
        /// Activa o desactiva los simuladores de síntoma.
        /// </summary>
        /// <param name="sintomas">Sintomas a simular, o NULL para detener los simuladores de síntomas.</param>
        private void ActivarSimuladoresDeSintomas(Sintoma[] sintomas)
        {
            foreach (SimuladorDeSintoma s in SimuladoresDeSintoma.Values)
            {// Limpiamos los síntomas anteriores
                s.SimulacionIniciada = false;
                if (s.InstrumentoAfectado.Tipo == TiposDeIntrumentos.Indicador_Luminoso)
                {// Apagamos todos los indicadores luminosos
                    s.InstrumentoAfectado.Valores[0] = 0;
                }
                s.SintomaASimular = null;
            }



            if (sintomas != null)
            {// Se agregan los nuevos síntomas e inicia la simulación
                SimuladorDeSintoma s;
                foreach (Sintoma sintoma in sintomas)
                {
                    if (this.SimuladoresDeSintoma.ContainsKey(sintoma.InstrumentoAfectado))
                    {
                        s = this.SimuladoresDeSintoma[sintoma.InstrumentoAfectado];
                        s.SintomaASimular = sintoma;
                        s.SimulacionIniciada = true;
                    }
                }
            }
        }

        #endregion
        

        #region Eventos Unity

        private void Awake()
        {
            this.SimuladoresDeSintoma = new Dictionary<NombresDeInstrumentos, SimuladorDeSintoma>();
        }

        private void Start()
        {
            this.InicializarDesdeDatosDeSesion();
        }

        private void Update()
        {
            if (Input.GetButtonDown("Start") && !this.sesionVaAComenzar)
            {
                StartCoroutine(this.IniciarSesionDeEntrenamiento(3));
            }
        }

        #endregion
    }
}