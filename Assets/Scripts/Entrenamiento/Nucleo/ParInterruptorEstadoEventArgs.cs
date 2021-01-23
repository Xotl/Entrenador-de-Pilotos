using System;

namespace Entrenamiento.Nucleo
{
    public class ParEstadoInterruptorEventArgs : EventArgs
    {
        private ParDeDatosInterruptorEstado _ParDeDatosInterruptorEstado;
        /// <summary>
        /// Obtiene el par de datos involucrado en el evento.
        /// </summary>
        public ParDeDatosInterruptorEstado ParDeDatosInterruptorEstado
        {
            get
            {
                return this._ParDeDatosInterruptorEstado;
            }
        }

        public ParEstadoInterruptorEventArgs(ParDeDatosInterruptorEstado par)
        {
            this._ParDeDatosInterruptorEstado = par;
        }
    }
}