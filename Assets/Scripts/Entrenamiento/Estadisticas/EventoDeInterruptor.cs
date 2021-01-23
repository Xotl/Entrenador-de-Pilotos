using System;
using System.Collections.Generic;

namespace Entrenamiento.Estadisticas
{
    public class EventoDeInterruptor : Base_de_datos.Modelo
	{
        private uint _Delta;
        private Entrenamiento.Nucleo.EstadosDeInterruptores _EstadoDelInterruptor;

        /// <summary>
        /// Se desencadena cuando la propiedad Delta cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarDelta;

        /// <param name="estadoDelInterruptor">Estado que tiene el interruptor en este momento delta.</param>
        /// <param name="delta">Cantidad de milisegundos que han transcurrido desde que inició la sesión de entrenamiento.</param>
        public EventoDeInterruptor(Entrenamiento.Nucleo.EstadosDeInterruptores estadoDelInterruptor, uint delta)
        {
            this._EstadoDelInterruptor = estadoDelInterruptor;
            this._Delta = delta;
        }

        /// <summary>
        /// Obtiene o establece la cantidad de milisegundos que han transcurrido desde que inició la sesión de entrenamiento.
        /// </summary>
        public uint Delta
        {
            get
            {
                return this._Delta;
            }
            set
            {
                if (this._Delta != value)
                {
                    this._Delta = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarDelta(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene el estado que tiene el interruptor en este momento delta.
        /// </summary>
        public Entrenamiento.Nucleo.EstadosDeInterruptores EstadoDelInterruptor
        {
            get
            {
                return this._EstadoDelInterruptor;
            }
        }

        protected override string NombreDeLaTabla
        {
            get { throw new NotImplementedException(); }
        }

        protected override Dictionary<string, Npgsql.NpgsqlParameter> ParametrosParaConsulta
        {
            get { throw new NotImplementedException(); }
        }

        protected override string NombreDelPrimaryKeyDeLaTabla
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarDelta.
        /// </summary>
        private void eventoAlCambiarDelta(EventArgs e)
        {
            if (this.AlCambiarDelta != null)
                this.AlCambiarDelta(this, e);
        }
    }
}
