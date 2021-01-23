using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Npgsql;

namespace Administración.Modelo
{
    public class Piloto : Base_de_datos.Modelo
	{
        private System.DateTime _FechaDeAlta;
        private string _Matricula;
        private string _Nombre;
        private string _Observaciones;
        private string _PassHash;

        /// <summary>
        /// Se desencadena cuando la propiedad Nombre cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarNombre;

        /// <summary>
        /// Se desencadena cuando la propiedad Matricula cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarMatricula;

        /// <summary>
        /// Se desencadena cuando la propiedad Observaciones cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarObservaciones;

        /// <summary>
        /// Se desencadena cuando la propiedad PassHash cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarPassHash;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FechaDeAlta"></param>
        /// <param name="Nombre"></param>
        /// <param name="Matricula"></param>
        /// <param name="Observaciones"></param>
        public Piloto(string Id, DateTime FechaDeAlta, string Nombre, string Matricula, string Observaciones)
        {
            this._Id = Id;
            this._FechaDeAlta = FechaDeAlta;
            this._Nombre = Nombre;
            this._Matricula = Matricula;
            this._Observaciones = Observaciones;
        }

        public Piloto()
        {
        }

        /// <summary>
        /// Obtiene o establece el nombre del piloto.
        /// </summary>
        public string Nombre
        {
            get
            {
                return this._Nombre;
            }
            set
            {
                if (value != this._Nombre)
                {
                    this._Nombre = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarNombre(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene o establece la matrícula del piloto.
        /// </summary>
        public string Matricula
        {
            get
            {
                return this._Matricula;
            }
            set
            {
                if (value != this._Matricula)
                {
                    this._Matricula = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarMatricula(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene o establece las observaciones del piloto.
        /// </summary>
        public string Observaciones
        {
            get
            {
                return this._Observaciones;
            }
            set
            {
                if (value != this._Observaciones)
                {
                    this._Observaciones = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarObservaciones(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Establece el hash del password del piloto.
        /// </summary>
        public string PassHash
        {
            set
            {
                if (value != this._PassHash)
                {
                    this._PassHash = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarPassHash(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene la fecha de alta del piloto.
        /// </summary>
        public DateTime FechaDeAlta
        {
            get
            {
                return this._FechaDeAlta;
            }
        }

        protected override string NombreDeLaTabla
        {
            get
            {
                return "pilotos";
            }
        }

        protected override Dictionary<string, NpgsqlParameter> ParametrosParaConsulta
        {
            get
            {
                Dictionary<string, NpgsqlParameter> parametros = new Dictionary<string, NpgsqlParameter>();
                parametros.Add("idpiloto", new NpgsqlParameter(":idpiloto", this.Id));
                parametros.Add("matricula", new NpgsqlParameter(":matricula", this.Matricula));
                parametros.Add("nombre", new NpgsqlParameter(":nombre", this.Nombre));
                parametros.Add("passhash", new NpgsqlParameter(":passhash", this._PassHash));
                parametros.Add("observaciones", new NpgsqlParameter(":observaciones", this.Observaciones));
                parametros.Add("fechadealta", new NpgsqlParameter(":fechadealta", this.FechaDeAlta));
                throw new System.NotImplementedException();
                //parametros.Add("pilotoscol", new NpgsqlParameter(":pilotoscol", null));
                //return parametros;
            }
        }

        /// <summary>
        /// Obtiene la cadena que se usará para realizar los SELECT´s en la base de datos.
        /// </summary>
        /// <remarks>La cadena debe incluir una representación del WHERE, ORDER BY y LIMIT de la siguiente manera (sí, sin espacios)-> WHERE true:where::orderBy::limit:</remarks>
        public static string StringDeSelect
        {
            get
            {
                throw new System.NotImplementedException();
                //return "SELECT tabla.campo1, tabla.campo2, tabla.campo3, ... FROM tabla WHERE true:where::orderBy::limit:;";
            }
        }

        /// <summary>
        /// Convierte los datos recibidos en objetos de esta clase. La función Base_de_datos.Modelo.Obtener<T>() depende de esta función.
        /// </summary>
        /// <returns>Array de objetos con los datos convertidos en instancias de esta clase.</returns>
        /// <param name="lista">Lista con datos a convertir. Los índices deben coincidir con los del resultado de la consulta del StringDeSelect.</param>
        public Piloto[] ConvertirResultadoEnInstancias(List<Dictionary<string, object>> lista, Base_de_datos.BaseDeDatos conexionDB)
        {
            List<Piloto> resultado = new List<Piloto>();
            foreach (Dictionary<string, object> fila in lista)
            {
                // Aquí se mandan llamar los objetos dependientes 
                // ClaseDeobjetoDependiente[] argumentoParaElConstructor = 
                // Modelo.Obtener<ClaseDeobjetoDependiente>(conexionDB, "idClaseDeobjetoDependiente = :uuid", "Nombre, etc.", string.Empty);

                Piloto objeto = new Piloto(fila["idpiloto"].ToString(), Convert.ToDateTime(fila["fechadealta"]),
                    fila["nombre"].ToString(), fila["matricula"].ToString(), fila["observaciones"].ToString());
                resultado.Add(objeto);
            }

            return resultado.ToArray();
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarMatricula.
        /// </summary>
        protected virtual void eventoAlCambiarMatricula(EventArgs e)
        {
            if (this.AlCambiarMatricula != null)
                this.AlCambiarMatricula(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarNombre.
        /// </summary>
        protected virtual void eventoAlCambiarNombre(System.EventArgs e)
        {
            if (this.AlCambiarNombre != null)
                this.AlCambiarNombre(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarObservaciones.
        /// </summary>
        protected virtual void eventoAlCambiarObservaciones(System.EventArgs e)
        {
            if (this.AlCambiarObservaciones != null)
                this.AlCambiarObservaciones(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarPassHash.
        /// </summary>
        protected virtual void eventoAlCambiarPassHash(System.EventArgs e)
        {
            if (this.AlCambiarPassHash != null)
                this.AlCambiarPassHash(this, e);
        }

        protected override string NombreDelPrimaryKeyDeLaTabla
        {
            get
            {
                return "idpiloto";
            }
        }
    }
}
