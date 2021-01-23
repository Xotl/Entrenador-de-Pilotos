using UnityEngine;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    public class IndicadorLuminosoController : InstrumentoGUIController
    {
        /// <summary>
        /// Material usado cuando la propiedad LuzEncendida es TRUE.
        /// </summary>
        public Material Encendido_Mat;

        /// <summary>
        /// Material usado cuando la propiedad LuzEncendida es FALSE.
        /// </summary>
        public Material Apagado_Mat;

        /// <summary>
        /// Obtiene o establece un nombre inicial para el indicador.
        /// </summary>
        public string nombreInicial = string.Empty;

        #region Propiedades

        [SerializeField]
        private bool luzEncendida = false;
        /// <summary>
        /// Obtiene o establece un valor que indica si la luz está encendida.
        /// </summary>
        public bool LuzEncendida
        {
            get
            {
                return this.luzEncendida;
            }
            set
            {
                if (this.luzEncendida != value)
                {
                    this.luzEncendida = value;

                    if (this.luzEncendida)
                        this.renderer.material = this.Encendido_Mat;
                    else
                        this.renderer.material = this.Apagado_Mat;

                    if (this.Instrumento != null)
                    {
                        this.Valor[0] = System.Convert.ToSingle(this.LuzEncendida);
                    }

                    this.eventoAlCambiarDeEstado(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Definición de Eventos de la clase

        /// <summary>
        /// Se produce cuando la propiedad LuzEncendida cambia.
        /// </summary>
        public System.EventHandler AlCambiarDeEstado;


        private void eventoAlCambiarDeEstado(System.EventArgs e)
        {
            if (this.AlCambiarDeEstado != null)
                this.AlCambiarDeEstado(this, e);
        }

        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            this.renderer.material = this.Apagado_Mat;
            this.AlCambiarValor += IndicadorLuminosoController_AlCambiarValor;

            if (string.IsNullOrEmpty(this.nombreInicial))
                this.transform.Find("Texto").GetComponent<TextMesh>().text = this.NombreDelInstrumento.ToString();
            else
                this.transform.Find("Texto").GetComponent<TextMesh>().text = this.nombreInicial;
        }

        #endregion

        private void IndicadorLuminosoController_AlCambiarValor(object sender, System.EventArgs e)
        {
            this.LuzEncendida = System.Convert.ToBoolean(this.Valor[0]);
        }
    }
}