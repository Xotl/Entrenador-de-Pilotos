using UnityEngine;
using Entrenamiento.Nucleo;
using Entrenamiento.GUI.Instrumentos;


namespace Entrenamiento.GUI.RealizarSesion
{
    public class PanelDeInstrumentosInitializer : MonoBehaviour
    {
        public ModelosDeHelicoptero Modelo = ModelosDeHelicoptero.Desconocido;

        private void Awake()
        {
            if (this.Modelo == ModelosDeHelicoptero.Desconocido)
            {
                throw new System.InvalidOperationException("No se puede inicializar un panel de instrumentos para el tipo Desconocido.");
            }

            InstrumentoGUIController[] instrumentosGUI = this.GetComponentsInChildren<InstrumentoGUIController>();
            foreach (InstrumentoGUIController i in instrumentosGUI)
            {
                i.Instrumento = Instrumentacion.InstanciarInstrumentoPorNombre(i.NombreDelInstrumento, this.Modelo);
            }
        }
    }
}
