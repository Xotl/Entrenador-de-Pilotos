using System;
using System.Collections.Generic;

namespace Entrenamiento.Estadisticas
{
    public class Hito : Base_de_datos.Modelo
	{
        /// <summary>
        /// </summary>
        private Entrenamiento.Estadisticas.TiposDeHitos _Tipo;
        /// <summary>
        /// </summary>
        private uint _Delta;
        /// <summary>
        /// </summary>
        private string _Nombre;
        private string _Info;

        /// <summary>
        /// Se desencadena cuando la propiedad Delta cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarDelta;

        /// <summary>
        /// Se desencadena cuando la propiedad Info cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarInfo;

        /// <param name="nombre">Nombre del instrumento o interruptor involucrado.</param>
        /// <param name="tipo">Manejador de hito.</param>
        /// <param name="delta">Cantidad de milisegundos que han transcurrido desde que inició la sesión de entrenamiento.</param>
        public Hito(string nombre, TiposDeHitos tipo, uint delta)
        {
            this._Nombre = nombre;
            this._Tipo = tipo;
            this._Delta = delta;
        }

        /// <summary>
        /// Obtiene el tipo de hito que sucedió.
        /// </summary>
        public TiposDeHitos Tipo
        {
            get
            {
                return this._Tipo;
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

        /// <summary>
        /// Obtiene el nombre del instrumento o interruptor involucrado.
        /// </summary>
        public string Nombre
        {
            get
            {
                return this._Nombre;
            }
        }

        /// <summary>
        /// Obtiene o establece información de interés para este hito.
        /// </summary>
        public string Info
        {
            get
            {
                return this._Info;
            }
            set
            {
                if (this._Info != value)
                {
                    this._Info = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarInfo(new EventArgs());
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

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarInfo.
        /// </summary>
        private void eventoAlCambiarInfo(EventArgs e)
        {
            if (this.AlCambiarInfo != null)
                this.AlCambiarInfo(this, e);
        }
    }
}
