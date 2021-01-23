using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class NuevoEstadoEnSolucionEventArgs : EventArgs
    {
        private bool _EraUnValorEsperado;
        private string _Descripcion;
        private bool _EraUnInterruptorValido;

        /// <param name="EraUnValorEsperado">Valor indicando si el interruptor debió haberse cambiado de estado o no.</param>
        /// <param name="EraUnInterruptorValido">Valor indicando si este era o no un valor esperado para la solución.</param>
        public NuevoEstadoEnSolucionEventArgs(bool EraUnInterruptorValido, bool EraUnValorEsperado)
        {
            this._EraUnInterruptorValido = EraUnInterruptorValido;
            this._EraUnValorEsperado = EraUnValorEsperado;
        }

        /// <param name="EraUnValorEsperado">Valor indicando si el interruptor debió haberse cambiado de estado o no.</param>
        /// <param name="EraUnInterruptorValido">Valor indicando si este era o no un valor esperado para la solución.</param>
        /// <param name="Descripcion">Descripción con detalles de lo que ocurrió.</param>
        public NuevoEstadoEnSolucionEventArgs(bool EraUnInterruptorValido, bool EraUnValorEsperado, string Descripcion)
        {
            this._EraUnInterruptorValido = EraUnInterruptorValido;
            this._EraUnValorEsperado = EraUnValorEsperado;
            this._Descripcion = Descripcion;
        }

        /// <summary>
        /// Obtiene un valor indicando si este era o no un valor esperado para la solución.
        /// </summary>
        public bool EraUnValorEsperado
        {
            get
            {
                return this._EraUnValorEsperado;
            }
        }

        /// <summary>
        /// Obtiene una descripción con detalles de lo que ocurrió.
        /// </summary>
        public string Descripcion
        {
            get
            {
                return this._Descripcion;
            }
        }

        /// <summary>
        /// Obtiene un valor indicando si el interruptor debió haberse cambiado de estado o no.
        /// </summary>
        public bool EraUnInterruptorValido
        {
            get
            {
                return this._EraUnInterruptorValido;
            }
        }
    }
}