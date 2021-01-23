using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class Interruptor
    {
        protected EstadosDeInterruptores[] _EstadosPermitidos;
        private EstadosDeInterruptores _EstadoActual = EstadosDeInterruptores.Desconocido;
        private NombresDeInterruptores _Nombre;

        /// <summary>
        /// Sucede cuando la propiedad Finalizado cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarSuEstado;

        public Interruptor(NombresDeInterruptores Nombre, EstadosDeInterruptores[] EstadosPermitidos)
        {
            if (EstadosPermitidos == null || EstadosPermitidos.Length < 2)
                throw new NoHayElementosException("Deben existir al menos 2 posiciones permitidas.");

            this._Nombre = Nombre;
            this._EstadosPermitidos = EstadosPermitidos;
        }

        /// <summary>
        /// Obtiene el nombre del interruptor.
        /// </summary>
        public NombresDeInterruptores Nombre
        {
            get
            {
                return this._Nombre;
            }
        }

        /// <summary>
        /// Obtiene una lista con los estados permitidos para este interruptor.
        /// </summary>
        public EstadosDeInterruptores[] EstadosPermitidos
        {
            get
            {
                return this._EstadosPermitidos;
            }
        }

        /// <summary>
        /// Obtiene o establece el estado actual del interruptor.
        /// </summary>
        /// <remarks>Si el estado que se quiere asignar no está dentro de la lista de estados permitidos arrojará una excepción.</remarks>
        public virtual EstadosDeInterruptores EstadoActual
        {
            get
            {
                return this._EstadoActual;
            }
            set
            {
                if (this._EstadoActual != value)
                {
                    if (!this.EsUnEstadoPermitido(value))
                        throw new PosicionInvalidaException(this.Nombre +
                            ": El estado que se quiere asignar no es válido para este interruptor.");

                    this._EstadoActual = value;
                    this.eventoAlCambiarSuEstado(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarSuEstado.
        /// </summary>
        private void eventoAlCambiarSuEstado(EventArgs e)
        {
            if (this.AlCambiarSuEstado != null)
                this.AlCambiarSuEstado(this, e);
        }

        /// <summary>
        /// Determina si un estado está permitido para este interruptor.
        /// </summary>
        /// <param name="Interruptor_no_esperado">Interruptor_no_esperado que se quiere validar.</param>
        /// <returns>Devuelde TRUE si es un estado permitido, de contrario, FALSE.</returns>
        public bool EsUnEstadoPermitido(EstadosDeInterruptores Estado)
        {
            foreach (EstadosDeInterruptores estado in this.EstadosPermitidos)
            {
                if (estado == Estado)
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            return this.Nombre + "[" + this.EstadosPermitidos.Length + "] - " + this.EstadoActual;
        }
    }
}