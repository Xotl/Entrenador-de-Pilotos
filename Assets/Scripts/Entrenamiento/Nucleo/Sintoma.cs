using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class Sintoma : Base_de_datos.Modelo
    {
        private NombresDeInstrumentos _InstrumentoAfectado;
        private TipoDeFuncionDeSintoma _TipoDeFuncion = TipoDeFuncionDeSintoma.Interpolacion;
        private Entrenamiento.Nucleo.ValoresDeInstrumento _Valores;
        private int _Intervalo;

        /// <summary>
        /// Se desencadena cuando el intervalo de este síntoma es modificado.
        /// </summary>
        public event EventHandler AlCambiarIntervalo;

        /// <summary>
        /// Se desencadena cuando el tipo de función de este síntoma es modificado.
        /// </summary>
        public event EventHandler AlCambiarTipoDeFuncion;

        /// <summary>
        /// Se desencadena cuando alguno de sus valores o la cantidad de valores han sido modificados.
        /// </summary>
        public event EventHandler AlCambiarValores;

        public Sintoma(NombresDeInstrumentos nombreInstrumento)
        {
            this._InstrumentoAfectado = nombreInstrumento;
            this._Valores = Instrumentacion.ObtenerValoresVaciosDeInstrumento(nombreInstrumento);
            this.Valores.AlCambiarUnValor += Valores_AlCambiarUnValor;
        }

        private void Valores_AlCambiarUnValor(object sender, EventArgs e)
        {
            this.eventoAlCambiarValores(EventArgs.Empty);
        }

        /// <summary>
        /// Obtiene el nombre del instrumento al que se le aplicará este síntoma.
        /// </summary>
        public NombresDeInstrumentos InstrumentoAfectado
        {
            get
            {
                return this._InstrumentoAfectado;
            }
        }

        /// <summary>
        /// Obtiene o establece los valores de interpolación para este síntoma.
        /// </summary>
        public ValoresDeInstrumento Valores
        {
            get
            {
                return this._Valores;
            }
            set
            {
                if (this._Valores != value)
                {
                    this._Valores = value;
                    this._SeHaModificado = true;
                    this.Valores.AlCambiarUnValor += Valores_AlCambiarUnValor;
                    this.eventoAlCambiarValores(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el tipo de función a realizar para este síntoma.
        /// </summary>
        public TipoDeFuncionDeSintoma TipoDeFuncion
        {
            get
            {
                return this._TipoDeFuncion;
            }
            set
            {
                if (value != this._TipoDeFuncion)
                {
                    this._TipoDeFuncion = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarTipoDeFuncion(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el intervalo de la interpolación en segundos.
        /// </summary>
        public int Intervalo
        {
            get
            {
                return this._Intervalo;
            }
            set
            {
                if (value != this._Intervalo)
                {
                    this._Intervalo = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarIntervalo(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarIntervalo.
        /// </summary>
        private void eventoAlCambiarIntervalo(EventArgs e)
        {
            if (this.AlCambiarIntervalo != null)
                this.AlCambiarIntervalo(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarTipoDeFuncion.
        /// </summary>
        private void eventoAlCambiarTipoDeFuncion(EventArgs e)
        {
            if (this.AlCambiarTipoDeFuncion != null)
                this.AlCambiarTipoDeFuncion(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarValores.
        /// </summary>
        private void eventoAlCambiarValores(EventArgs e)
        {
            if (this.AlCambiarValores != null)
                this.AlCambiarValores(this, e);
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
    }
}