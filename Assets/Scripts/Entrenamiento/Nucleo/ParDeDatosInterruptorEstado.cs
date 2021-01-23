using System;

namespace Entrenamiento.Nucleo
{
    public struct ParDeDatosInterruptorEstado
    {
        /// <summary>
        /// Nombre del interruptor afectado.
        /// </summary>
        public NombresDeInterruptores Interruptor;
        /// <summary>
        /// Estado que debe tener el interruptor.
        /// </summary>
        public EstadosDeInterruptores Estado;

        public override string ToString()
        {
            return Interruptor/*.Nombre*/ + " - " + Estado;
        }
    }
}