using System;
using System.Collections.Generic;

namespace Entrenamiento.Estadisticas
{
    public class CriterioDeEvaluacion : Base_de_datos.Modelo
	{
        private ushort _Calificacion;
        private string _Nombre;

        /// <summary>
        /// Este evento se desencadena cuando la propiedad Calificacion cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarCalificacion;

        /// <summary>
        /// Se desencadena cuando la propiedad Nombre cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarNombre;

        /// <param name="nombre">Nombre de este criterio.</param>
        public CriterioDeEvaluacion(string nombre)
        {
            this._Nombre = nombre;
        }

        /// <summary>
        /// Obtiene o establece el nombre de este criterio de evaluación.
        /// </summary>
        public string Nombre
        {
            get
            {
                return this._Nombre;
            }
            set
            {
                if (this._Nombre != value)
                {
                    this._Nombre = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarNombre(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene o establece la calificación para este criterio.
        /// </summary>
        public ushort Calificacion
        {
            get
            {
                return this._Calificacion;
            }
            set
            {
                if (this._Calificacion != value)
                {
                    if (value > 100)
                        throw new ArgumentOutOfRangeException("Asignación de calificación", "La calificación no puede ser mayor que 100.");

                    this._Calificacion = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarCalificacion(new EventArgs());
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
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarCalificacion.
        /// </summary>
        private void eventoAlCambiarCalificacion(EventArgs e)
        {
            if (this.AlCambiarCalificacion != null)
                this.AlCambiarCalificacion(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarNombre.
        /// </summary>
        private void eventoAlCambiarNombre(EventArgs e)
        {
            if (this.AlCambiarNombre != null)
                this.AlCambiarNombre(this, e);
        }
    }
}
