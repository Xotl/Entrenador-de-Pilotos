using System;
using System.Collections.Generic;

namespace Entrenamiento.Estadisticas
{
    public class EventoDeInstrumento : Base_de_datos.Modelo
	{
        private uint _Delta;
        private Entrenamiento.Nucleo.ValoresDeInstrumento _ValoresDelInstrumento;

        /// <summary>
        /// Se desencadena cuando la propiedad Delta cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarDelta;

        /// <param name="valoresDelInstrumento">Valores que tiene el instrumento en este momento delta.</param>
        /// <param name="delta">Cantidad de milisegundos que han transcurrido desde que inició la sesión de entrenamiento.</param>
        public EventoDeInstrumento(Entrenamiento.Nucleo.ValoresDeInstrumento valoresDelInstrumento, uint delta)
        {
            this._ValoresDelInstrumento = valoresDelInstrumento;
            this._Delta = delta;
        }

        /// <summary>
        /// Obtiene los valores que tiene el instrumento en este momento delta.
        /// </summary>
        public Entrenamiento.Nucleo.ValoresDeInstrumento ValoresDelInstrumento
        {
            get
            {
                return this._ValoresDelInstrumento;
            }
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
