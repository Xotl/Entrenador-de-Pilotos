using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public abstract class Instrumento : Base_de_datos.Modelo
    {
        private Entrenamiento.Nucleo.NombresDeInstrumentos _Nombre;
        private ValoresDeInstrumento _Valores;
        private ValoresDeInstrumento _ValoresMaximos;// Límites superiores del instrumento
        private ValoresDeInstrumento _ValoresMinimos;// Límites inferiores del instrumento
        private TiposDeIntrumentos _Tipo;

        /// <summary>
        /// Se desencadena cuando uno de sus valores ha cambiado.
        /// </summary>
        public event EventHandler AlCambiarValor;

        public Instrumento(Entrenamiento.Nucleo.NombresDeInstrumentos Nombre, TiposDeIntrumentos Tipo)
        {
            this._Nombre = Nombre;
            this._Tipo = Tipo;
            this._ValoresMaximos = this.valoresMaximos();
            this._ValoresMinimos = this.valoresMinimos();
            this.Valores = Instrumentacion.ObtenerValoresVaciosDeInstrumento(this._Nombre);
            
            this.AlCambiarValor += new EventHandler(this.Instrumento_AlCambiarValor);
        }

        private void Instrumento_AlCambiarValor(object sender, EventArgs e)
        {
            this.valoresEnAlertaCtrl = this.seEncuentraEnAlerta(this.Valores);

            if (this.valoresEnAlertaCtrl)// Si ya está en Alerta entonces no puede estar en Advertencia
                this.valoresEnAdvertenciaCtrl = false;
            else
                this.valoresEnAdvertenciaCtrl = this.seEncuentraEnAdvertencia(this.Valores);
        }

        /// <summary>
        /// Obtiene límites superiores del instrumento.
        /// </summary>
        public ValoresDeInstrumento ValoresMaximos
        {
            get
            {
                return this._ValoresMaximos;
            }
        }

        /// <summary>
        /// Obtiene límites inferiores del instrumento.
        /// </summary>
        public ValoresDeInstrumento ValoresMinimos
        {
            get
            {
                return this._ValoresMinimos;
            }
        }

        /// <summary>
        /// Obtiene o establece los valores que muestra el instrumento.
        /// </summary>
        public ValoresDeInstrumento Valores
        {
            get
            {
                return this._Valores;
            }
            set
            {
                if (value != null)
                {// Validación de límites superiores e inferiores.
                    int i = 0;
                    while (i < value.Cantidad)
                    {
                        if (value[i] < this._ValoresMinimos[i])
                            value[i] = this._ValoresMinimos[i];// Límite inferior
                        else if (value[i] > this._ValoresMaximos[i])
                            value[i] = this._ValoresMaximos[i];// Límite superior
                        i++;
                    }
                }

                if (value != this._Valores)
                {
                    if (this._Valores != null)
                    {// Como la referencia se perderá hay que limpiar sus delegados.
                        this._Valores.AlCambiarUnValor -= this.value_AlCambiarValores;
                        this._Valores.AlCambiarLaCantidad -= this.value_AlCambiarValores;
                    }
                    
                    this._Valores = value;
                    if (this._Valores != null)
                    {
                        this._Valores.AlCambiarUnValor += this.value_AlCambiarValores;
                        this._Valores.AlCambiarLaCantidad += this.value_AlCambiarValores;
                    }
                    this.eventoAlCambiarValor(new EventArgs());
                }
            }
        }

        private void value_AlCambiarValores(object sender, EventArgs e)
        {
            this.eventoAlCambiarValor(new EventArgs());
        }

        /// <summary>
        /// Obtiene el nombre del instrumento.
        /// </summary>
        public NombresDeInstrumentos Nombre
        {
            get
            {
                return this._Nombre;
            }
        }

        /// <summary>
        /// Obtiene el tipo de instrumento.
        /// </summary>
        public TiposDeIntrumentos Tipo
        {
            get
            {
                return this._Tipo;
            }
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarValor.
        /// </summary>
        private void eventoAlCambiarValor(EventArgs e)
        {
            if (this.AlCambiarValor != null)
                this.AlCambiarValor(this, e);
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
        /// Se desencadena cuando los Valores entran al rango de Advertencia.
        /// </summary>
        public event EventHandler AlEntrarEnAdvertencia;

        /// <summary>
        /// Se desencadena cuando los Valores entran al rango de Alerta.
        /// </summary>
        public event EventHandler AlEntrarEnAlerta;

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlEntrarEnAdvertencia.
        /// </summary>
        private void eventoAlEntrarEnAdvertencia(EventArgs e)
        {
            if (this.AlEntrarEnAdvertencia != null)
                this.AlEntrarEnAdvertencia(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlEntrarEnAlerta.
        /// </summary>
        private void eventoAlEntrarEnAlerta(EventArgs e)
        {
            if (this.AlEntrarEnAlerta != null)
                this.AlEntrarEnAlerta(this, e);
        }

        /// <summary>
        /// Obtiene un valor que indica si el instrumento se encuentra en el rango de advertencia.
        /// </summary>
        public bool ValoresEnAdvertencia
        {
            get
            {
                return this.valoresEnAdvertenciaCtrl;
            }
        }

        private bool _valoresEnAdvertenciaCtrl = false;

        /// <summary>
        /// Establece si el instrumento se encuentra en el rango de advertencia.
        /// </summary>
        private bool valoresEnAdvertenciaCtrl
        {
            get
            {
                return this._valoresEnAdvertenciaCtrl;
            }
            set
            {
                if (this._valoresEnAdvertenciaCtrl != value)
                {
                    this._valoresEnAdvertenciaCtrl = value;
                    if (this._valoresEnAdvertenciaCtrl)
                    {// Entró al rango de advertencia
                        this.eventoAlEntrarEnAdvertencia(new EventArgs());
                    }
                }
            }
        }

        private bool _valoresEnAlertaCtrl = false;

        /// <summary>
        /// Establece si el instrumento se encuentra en el rango de alerta.
        /// </summary>
        private bool valoresEnAlertaCtrl
        {
            get
            {
                return this._valoresEnAlertaCtrl;
            }
            set
            {
                if (this._valoresEnAlertaCtrl != value)
                {
                    this._valoresEnAlertaCtrl = value;
                    if (this._valoresEnAlertaCtrl)
                    {// Entró al rango de alerta
                        this.eventoAlEntrarEnAlerta(new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene un valor que indica si el instrumento se encuentra en el rango de alerta.
        /// </summary>
        public bool ValoresEnAlerta
        {
            get
            {
                return this.valoresEnAlertaCtrl;
            }
        }

        /// <summary>
        /// Evalúa si el instrumento se encuentra en el rango de advertencia basado en los valores recibidos.
        /// </summary>
        /// <param name="valores">Valores con los que se hará la evaluación</param>
        /// <returns>TRUE si se encuentra en el rango de advertencia, de lo contrario FALSE.</returns>
        protected abstract bool seEncuentraEnAdvertencia(ValoresDeInstrumento valores);

        /// <summary>
        /// Evalúa si el instrumento se encuentra en el rango de alerta basado en los valores recibidos.
        /// </summary>
        /// <param name="valores">Valores con los que se hará la evaluación</param>
        protected abstract bool seEncuentraEnAlerta(Entrenamiento.Nucleo.ValoresDeInstrumento valores);

        /// <summary>
        /// Obtiene los valores máximos del instrumento.
        /// </summary>
        /// <returns>Valores con los límites superiores del instrumento.</returns>
        protected abstract ValoresDeInstrumento valoresMaximos();

        /// <summary>
        /// Obtiene los valores mínimos del instrumento.
        /// </summary>
        /// <returns>Valores con los límites inferiores del instrumento.</returns>
        protected abstract ValoresDeInstrumento valoresMinimos();
    }
}