using UnityEngine;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI
{
    public class BotonInstrumentoDePanel : Interfaz.Utilities.BotonController
    {
        private Instrumentos.InstrumentoGUIController instrumentoGUI;

        [SerializeField]
        private Entrenamiento.Nucleo.NombresDeInstrumentos nombreDelInstrumento = NombresDeInstrumentos.Desconocido;
        /// <summary>
        /// Obtiene el nombre del instrumento.
        /// </summary>
        public Entrenamiento.Nucleo.NombresDeInstrumentos NombreDelInstrumento
        {
            get
            {
                return this.nombreDelInstrumento;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            this.instrumentoGUI = this.GetComponent<Instrumentos.InstrumentoGUIController>();
            if (this.instrumentoGUI != null)
            {
                this.nombreDelInstrumento = this.instrumentoGUI.NombreDelInstrumento;
            }
        }
    }
}