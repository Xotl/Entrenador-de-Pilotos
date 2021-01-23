using UnityEngine;
using Interfaz.Utilities;

namespace Entrenamiento.GUI.ReproductorDeSesion
{
    public class ReproductorGUIController : MonoBehaviour
    {
        #region Campos públicos

        public Material playMat;
        public Material pauseMat;

        public HitoGUI hitoAdvertenciaPrefab;
        public HitoGUI hitoAlertaPrefab;
        public HitoGUI hitoEquivocacionPrefab;
        public HitoGUI hitoEtapaPrefab;

        #endregion


        #region Campos privados

        /// <summary>
        /// Objeto que contiene todos los hitos mostrados.
        /// </summary>
        private Transform contenedorHitos;

        private HitoCheckBox chkAlertas;
        private HitoCheckBox chkAdvertencias;
        private HitoCheckBox chkEtapas;

        /// <summary>
        /// Botón de Play/Pause.
        /// </summary>
        private BotonController btnPlay;

        /// <summary>
        /// Representación del tiempo actual.
        /// </summary>
        private TextMesh txtTiempoActual;

        /// <summary>
        /// Representación del tiempo final.
        /// </summary>
        private TextMesh txtTiempoFinal;

        /// <summary>
        /// Posición de esferitaDeLaLineaDeTiempo cuando el tiempo actual es 0;
        /// </summary>
        private Vector3 posicionInicialDeEsferaDeTiempo;

        /// <summary>
        /// Tamaño total (a lo largo, es decir, en el eje X) de la línea de tiempo en unidades Unity.
        /// </summary>
        private float tamTotalLineaDeTiempo = 0f;

        /// <summary>
        /// Representación del tiempo actual en la línea del tiempo.
        /// </summary>
        private LineaDeTiempoGUIControl lineaDeTiempoGUIControl;

        /// <summary>
        /// Vector usado como buffer en el arrastre del control de le línea de tiempo.
        /// </summary>
        private Vector3 posMouse;

        #endregion


        #region Propiedades
        
        private Entrenamiento.Estadisticas.ReproductorDeSesion reproductorDeSesion;
        /// <summary>
        /// Obtiene o establece el reproductor de sesión lógico asociado a este control.
        /// </summary>
        public Entrenamiento.Estadisticas.ReproductorDeSesion ReproductorDeSesion
        {
            get
            {
                return this.reproductorDeSesion;
            }
            set
            {
                if (this.reproductorDeSesion != value)
                {
                    if (this.reproductorDeSesion != null)
                    {
                        this.reproductorDeSesion.AlCambiarTiempoActual -= this.reproductorDeSesion_AlCambiarTiempoActual;
                    }

                    this.reproductorDeSesion = value;

                    if (this.reproductorDeSesion != null)
                    {
                        this.txtTiempoActual.text = "0:0";
                        this.reproductorDeSesion.AlCambiarTiempoActual += this.reproductorDeSesion_AlCambiarTiempoActual;

                        int minutos = (int)(this.reproductorDeSesion.TiempoFinal / 60000);
                        this.txtTiempoFinal.text = minutos + ":" + ((this.reproductorDeSesion.TiempoFinal - (minutos * 60000)) / 1000);

                        this.DibujarHitosEnReproductor(this.reproductorDeSesion.SesionDeEntrenamiento.Hitos, true, true, true);
                    }
                }
            }
        }

        private bool reproduciendo = false;
        /// <summary>
        /// Obtiene un valor que indica si se está reproduciendo la sesión actualmente.
        /// </summary>
        public bool Reproduciendo
        {
            get
            {
                return this.reproduciendo;
            }

            private set
            {
                if (this.reproduciendo != value)
                {
                    this.reproduciendo = value;

                    if (this.reproduciendo && this.reproductorDeSesion.TiempoActual == this.reproductorDeSesion.TiempoFinal)
                    {// Si está al final y quiere reproducir, entonces lo mando al principio de la reproducción
                        this.reproductorDeSesion.TiempoActual = 0;
                    }

                    if (this.reproduciendo)
                        this.btnPlay.renderer.material = this.pauseMat;
                    else
                        this.btnPlay.renderer.material = this.playMat;

                    this.eventoAlCambiarEstadoDeReproduccion(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando la propiedad Reproduciendo cambia de valor.
        /// </summary>
        public event System.EventHandler AlCambiarEstadoDeReproduccion;

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarEstadoDeReproduccion.
        /// </summary>
        private void eventoAlCambiarEstadoDeReproduccion(System.EventArgs e)
        {
            if (this.AlCambiarEstadoDeReproduccion != null)
                this.AlCambiarEstadoDeReproduccion(this, e);
        }


        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            // Obtención de objetos
            this.chkAdvertencias = this.transform.Find("Selector de Visualizacion/Advertencias").GetComponent<HitoCheckBox>();
            this.chkAlertas = this.transform.Find("Selector de Visualizacion/Alertas").GetComponent<HitoCheckBox>();
            this.chkEtapas = this.transform.Find("Selector de Visualizacion/Etapas").GetComponent<HitoCheckBox>();
            this.contenedorHitos = this.transform.Find("Hitos");
            this.btnPlay = this.transform.Find("Play button").GetComponent<BotonController>();
            this.txtTiempoActual = this.transform.Find("Tiempo actual").GetComponent<TextMesh>();
            this.txtTiempoFinal = this.transform.Find("Tiempo final").GetComponent<TextMesh>();
            this.lineaDeTiempoGUIControl = 
                this.transform.Find("Línea del tiempo/Esfera de reproduccion").GetComponent<LineaDeTiempoGUIControl>();

            // Inicialización
            this.tamTotalLineaDeTiempo = this.lineaDeTiempoGUIControl.transform.parent.renderer.bounds.size.x;
            this.posicionInicialDeEsferaDeTiempo = new Vector3(
                this.lineaDeTiempoGUIControl.transform.position.x - this.tamTotalLineaDeTiempo / 2,
                this.lineaDeTiempoGUIControl.transform.position.y,
                this.lineaDeTiempoGUIControl.transform.position.z
            );
            this.lineaDeTiempoGUIControl.transform.position = this.posicionInicialDeEsferaDeTiempo;

            // Eventos
            this.btnPlay.Click += btnPlay_Click;
            this.lineaDeTiempoGUIControl.AlIniciarDrag += this.lineaDeTiempoGUIControl_AlIniciarDrag;
            this.lineaDeTiempoGUIControl.DuranteElDrag += this.lineaDeTiempoGUIControl_DuranteElDrag;
            this.chkAdvertencias.AlCambiarChecked += this.chkHitos_AlCambiarChecked;
            this.chkAlertas.AlCambiarChecked += this.chkHitos_AlCambiarChecked;
            this.chkEtapas.AlCambiarChecked += this.chkHitos_AlCambiarChecked;
        }

        private void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetButtonDown("Start"))
            {
                this.Reproduciendo = !this.Reproduciendo;
            }

            if (this.Reproduciendo && !this.lineaDeTiempoGUIControl.Arrastrando)
            {
                this.reproductorDeSesion.TiempoActual += System.Convert.ToUInt32(Time.deltaTime * 1000);
                
                if (this.reproductorDeSesion.TiempoActual == this.reproductorDeSesion.TiempoFinal)
                {
                    this.Reproduciendo = false;
                }
            }
        }

        #endregion


        #region Métodos de la clase
        
        private void chkHitos_AlCambiarChecked(object sender, System.EventArgs e)
        {
            this.DibujarHitosEnReproductor(this.reproductorDeSesion.SesionDeEntrenamiento.Hitos, this.chkAlertas.Checked,
                this.chkAdvertencias.Checked, this.chkEtapas.Checked);
        }

        private void lineaDeTiempoGUIControl_DuranteElDrag(object sender, System.EventArgs e)
        {
            Vector3 nuevaPos = Input.mousePosition;
            float dif = Camera.main.ScreenToWorldPoint(nuevaPos).x - Camera.main.ScreenToWorldPoint(this.posMouse).x;
            this.posMouse = nuevaPos;

            // Transformo la diferencia de unidades Unity a milisegundos
            dif = dif / this.tamTotalLineaDeTiempo * this.reproductorDeSesion.TiempoFinal;

            if (dif < 0)
            {
                dif = Mathf.Abs(dif);
                if (dif > this.reproductorDeSesion.TiempoActual)// Evito que al restar de más se vaya al valor máximo del uInt
                    dif = this.reproductorDeSesion.TiempoActual;

                this.reproductorDeSesion.TiempoActual -= System.Convert.ToUInt32(Mathf.Abs(dif));
            }
            else
            {
                this.reproductorDeSesion.TiempoActual += System.Convert.ToUInt32(dif);
            }
        }

        private void lineaDeTiempoGUIControl_AlIniciarDrag(object sender, System.EventArgs e)
        {
            this.posMouse = Input.mousePosition;
        }

        private void reproductorDeSesion_AlCambiarTiempoActual(object sender, System.EventArgs e)
        {
            int minutos = (int)(this.reproductorDeSesion.TiempoActual / 60000);
            this.txtTiempoActual.text = minutos + ":" + ((this.reproductorDeSesion.TiempoActual - (minutos * 60000)) / 1000);
            this.ActualizarPosicionDeEsferita();
        }

        private void btnPlay_Click(object sender, System.EventArgs e)
        {
            this.Reproduciendo = !this.Reproduciendo;
        }

        /// <summary>
        /// Actualiza la posición de la esferita de la línea del tiempo según el tiempo actual.
        /// </summary>
        private void ActualizarPosicionDeEsferita()
        {
            float dif = this.reproductorDeSesion.TiempoActual * this.tamTotalLineaDeTiempo / this.reproductorDeSesion.TiempoFinal;
            this.lineaDeTiempoGUIControl.transform.position = this.posicionInicialDeEsferaDeTiempo + new Vector3(dif, 0, 0);
        }

        /// <summary>
        /// Instancia y coloca los marcadores de hitos en este control.
        /// </summary>
        /// <param name="hitos">Conjunto de hitos a dibujar.</param>
        /// <param name="Alertas">Indica si se desean colocar los hitos del tipo Alerta_de_instrumento.</param>
        /// <param name="Advertencias">Indica si se desean colocar los hitos del tipo Advertencia_de_instrumento.</param>
        /// <param name="Etapas">Indica si se desean colocar los hitos del tipo Inicio_de_etapa.</param>
        private void DibujarHitosEnReproductor(Estadisticas.Hito[] hitos, bool Alertas, bool Advertencias, bool Etapas)
        {
            Transform objHijo;
            while (this.contenedorHitos.childCount > 0)
            {// Destruyo todos los hitos viejos
                objHijo = this.contenedorHitos.GetChild(0);
                objHijo.parent = null;
                Destroy(objHijo.gameObject);
            }


            HitoGUI hitoObj = null;
            foreach (Estadisticas.Hito hito in hitos)
            {
                hitoObj = null;
                switch (hito.Tipo)
                {// Identifico e instancio según el tipo de hito
                    case Estadisticas.TiposDeHitos.Advertencia_de_instrumento:
                        if (Advertencias)
                        {
                            hitoObj = (HitoGUI)Instantiate(this.hitoAdvertenciaPrefab);
                        }
                        break;

                    case Estadisticas.TiposDeHitos.Alerta_de_instrumento:
                        if (Alertas)
                        {
                            hitoObj = (HitoGUI)Instantiate(this.hitoAlertaPrefab);
                        }
                        break;

                    case Estadisticas.TiposDeHitos.Inicio_de_etapa:
                    case Estadisticas.TiposDeHitos.Fin_de_etapa:
                        if (Etapas)
                        {
                            hitoObj = (HitoGUI)Instantiate(this.hitoEtapaPrefab);
                        }
                        break;

                    case Estadisticas.TiposDeHitos.Interruptor_esperado_pero_estado_incorrecto:
                    case Estadisticas.TiposDeHitos.Interruptor_no_esperado:
                        hitoObj = (HitoGUI)Instantiate(this.hitoEquivocacionPrefab);
                        break;
                }

                if (hitoObj != null)
                {
                    hitoObj.Hito = hito;
                    hitoObj.transform.parent = this.contenedorHitos;
                    hitoObj.transform.position = new Vector3(
                        this.posicionInicialDeEsferaDeTiempo.x +
                            hito.Delta * this.tamTotalLineaDeTiempo / this.reproductorDeSesion.TiempoFinal,
                        this.posicionInicialDeEsferaDeTiempo.y,
                        this.contenedorHitos.position.z
                    );
                }
            }
        }

        #endregion
    }
}