using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Administración.Modelo
{
    public class Entrenador : Comunicacion_JSON.Modelo
	{
        public Entrenador(string id)
        {
            this.id = id;
        }

        private string _Matricula = string.Empty;
        private string _Nombre = string.Empty;
        private string _PassHash = string.Empty;

        /// <summary>
        /// Se desencadena cuando la propiedad Nombre cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarNombre;

        /// <summary>
        /// Se desencadena cuando la propiedad Matricula cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarMatricula;

        /// <summary>
        /// Se desencadena cuando la propiedad PassHash cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarPassHash;

        /// <summary>
        /// Obtiene o establece el nombre del entrenador.
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
                    this.eventoAlCambiarNombre(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Obtiene o establece la matrícula del entrenador.
        /// </summary>
        public string Matricula
        {
            get
            {
                return this._Matricula;
            }
            set
            {
                if (this._Matricula != value)
                {
                    this._Matricula = value;
                    this.eventoAlCambiarMatricula(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Establece el hash del password del entrenador.
        /// </summary>
        public string PassHash
        {
            set
            {
                if (this._PassHash != value)
                {
                    this._PassHash = value;
                    this.eventoAlCambiarPassHash(EventArgs.Empty);
                }
            }
        }

        //protected override string NombreDeLaTabla
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //protected override Dictionary<string, Npgsql.NpgsqlParameter> ParametrosParaConsulta
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //protected override string NombreDelPrimaryKeyDeLaTabla
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarMatricula.
        /// </summary>
        private void eventoAlCambiarMatricula(EventArgs e)
        {
            if (this.AlCambiarMatricula != null)
                this.AlCambiarMatricula(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarNombre.
        /// </summary>
        private void eventoAlCambiarNombre(System.EventArgs e)
        {
            if (this.AlCambiarNombre != null)
                this.AlCambiarNombre(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarPassHash.
        /// </summary>
        private void eventoAlCambiarPassHash(System.EventArgs e)
        {
            if (this.AlCambiarPassHash != null)
                this.AlCambiarPassHash(this, e);
        }
    }
}
