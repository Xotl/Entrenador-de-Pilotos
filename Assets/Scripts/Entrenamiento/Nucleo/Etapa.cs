using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class SolucionEnEtapaEventArgs : EventArgs
    {
        private Entrenamiento.Nucleo.Solucion _Solucion;

        /// <param name="solucion">Solucion involucrada.</param>
        public SolucionEnEtapaEventArgs(Entrenamiento.Nucleo.Solucion solucion)
        {
            this._Solucion = solucion;
        }

        /// <summary>
        /// Obtiene la Solucion involucrada en el evento.
        /// </summary>
        public Solucion Solucion
        {
            get
            {
                return this._Solucion;
            }
        }
    }
    
    public class Etapa : Base_de_datos.Modelo
    {
        #region Campos privados
        
        /// <summary>
        /// Necesario para mantener el estado de la máquina de estados.
        /// </summary>
        private IEnumerator<Solucion> _IEnumeratorDeMaquina;

        #endregion


        #region Propiedades

        private Solucion solucionActual;
        /// <summary>
        /// Obtiene la solución actual en la simulación.
        /// </summary>
        public Solucion SolucionActual
        {
            get
            {
                return this.solucionActual;
            }
        }

        private bool _EnSimulacion;
        /// <summary>
        /// Obtiene o establece un valor que indica si la solución se está ejecutando como una sesión de entrenamiento.
        /// </summary>
        public bool EnSimulacion
        {
            get
            {
                return this._EnSimulacion;
            }
            set
            {
                if (value != this._EnSimulacion)
                {
                    this._EnSimulacion = value;
                    if (this._EnSimulacion)
                    {// Activar modo de simulación
                        //if (this.Soluciones.Length <= 0)
                        //    throw new InvalidOperationException("No se pudo inicar el modo de simulación porque no hay soluciones asignadas.");

                        this._IEnumeratorDeMaquina = null;// Reinicio la máquina de estados a su posición inicial
                        this._Finalizada = false;
                        this.eventoAlIniciarSimulacion(new EventArgs());
                        this.AvanzarALaSiguienteSolucion();// Inicia la primera simulación
                    }
                    else
                    {// Detener modo de simulación
                        foreach (Solucion s in this.Soluciones)
                        {// Detiene cualquier solución en modo simulación
                            s.EnSimulacion = false;
                        }
                        this.eventoAlDetenerSimulacion(new EventArgs());
                    }
                }
            }
        }


        private List<Solucion> _Soluciones = null;
        /// <summary>
        /// Obtiene un arreglo con las soluciones para esta solucion.
        /// </summary>
        public Solucion[] Soluciones
        {
            get
            {
                return this._Soluciones.ToArray();
            }
        }


        private List<Sintoma> _Sintomas = null;
        /// <summary>
        /// Obtiene o establece los síntomas de esta solución.
        /// </summary>
        public Sintoma[] Sintomas
        {
            get
            {
                return this._Sintomas.ToArray();
            }
            set
            {
                this._Sintomas.Clear();
                this.AgregarSintomas(value);
            }
        }


        private bool _Finalizada;
        /// <summary>
        /// Obtiene un valor que indica si esta solucion ya ha finalizado.
        /// </summary>
        public bool Finalizada
        {
            get
            {
                return this._Finalizada;
            }
        }


        private string _Nombre;
        /// <summary>
        /// Obtiene o establece el nombre de la solucion.
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

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Representación de la máquina de estados.
        /// </summary>
        /// <remarks>Aquí normalmente iría sólo un foreach con yield returns.</remarks>
        private IEnumerable<Solucion> MaquinaDeEstados()
        {
            if (this.Soluciones.Length != 0)
            {
                Solucion sFinal = this.Soluciones[this.Soluciones.Length - 1];
                foreach (Solucion s in this.Soluciones)
                {
                    this.solucionActual = s;
                    s.AlFinalizar += new EventHandler(Current_AlFinalizar);
                    s.AlDetenerSimulacion += new EventHandler(Current_AlDetenerSimulacion);
                    yield return s;
                    s.AlFinalizar -= this.Current_AlFinalizar;
                    s.AlDetenerSimulacion -= this.Current_AlDetenerSimulacion;
                    if (!sFinal.Equals(s))
                        this.eventoAlAvanzar(new EventArgs());
                }
            }

            this.solucionActual = null;
            this._IEnumeratorDeMaquina = null;
            this._Finalizada = true;
            this.EnSimulacion = false;
            this.eventoAlFinalizarEtapa(new EventArgs());
        }

        private void Current_AlDetenerSimulacion(object sender, EventArgs e)
        {// Si se detiene la simulación de una solución pero ésta no finalizó entonces se detiene la simulación de la solucion.
            Solucion solucion = (Solucion)sender;
            if (!solucion.Finalizada)
                this.EnSimulacion = false;
        }

        private void Current_AlFinalizar(object sender, EventArgs e)
        {// En cuanto finalice una solución avanza a la siguiente.
            this.AvanzarALaSiguienteSolucion();
        }

        /// <summary>
        /// Avanza a la siguiente solucion.
        /// </summary>
        private void AvanzarALaSiguienteSolucion()
        {
            if (this._IEnumeratorDeMaquina == null)
                this._IEnumeratorDeMaquina = this.MaquinaDeEstados().GetEnumerator();

            if (this._IEnumeratorDeMaquina.MoveNext() && this.solucionActual != null)
                this.solucionActual.EnSimulacion = true;
        }

        /// <summary>
        /// Agrega un grupo de síntomas a la Etapa.
        /// </summary>
        /// <param name="sintomas"></param>
        public void AgregarSintomas(Sintoma[] sintomas)
        {
            this._Sintomas.AddRange(sintomas);
            this._SeHaModificado = true;
            this.eventoAlAgregarSintoma(new EventArgs());
        }

        /// <summary>
        /// Agrega un nuevo síntoma a la Etapa.
        /// </summary>
        public void AgregarSintoma(Sintoma sintoma)
        {
            this._Sintomas.Add(sintoma);
            this._SeHaModificado = true;
            this.eventoAlAgregarSintoma(new EventArgs());
        }

        /// <summary>
        /// Agrega una nueva solución a la solucion.
        /// </summary>
        public void AgregarSolucion(Solucion solucion)
        {
            this._Soluciones.Add(solucion);
            this._SeHaModificado = true;
            this.eventoAlAgregarSolucion(new SolucionEnEtapaEventArgs(solucion));
        }

        /// <summary>
        /// Remueve un síntoma de la solucion.
        /// </summary>
        /// <param name="indice">Índice donde se encuentra el síntoma a remover.</param>
        public void QuitarSintoma(int indice)
        {
            this._Sintomas.RemoveAt(indice);
            this._SeHaModificado = true;
            this.eventoAlQuitarSintoma(new EventArgs());
        }

        /// <summary>
        /// Remueve un síntoma de la solucion.
        /// </summary>
        /// <param name="sintomas">Sintoma a remover.</param>
        public void QuitarSintoma(Sintoma sintoma)
        {
            this._Sintomas.Remove(sintoma);
            this._SeHaModificado = true;
            this.eventoAlQuitarSintoma(new EventArgs());
        }

        /// <summary>
        /// Remueve una solución de la solucion.
        /// </summary>
        /// <param name="indice">Índice donde se encuentra la solución a remover.</param>
        public void QuitarSolucion(int indice)
        {
            Solucion solucion = this._Soluciones[indice];
            this._Soluciones.RemoveAt(indice);
            this._SeHaModificado = true;
            this.eventoAlQuitarSolucion(new SolucionEnEtapaEventArgs(solucion));
        }

        /// <summary>
        /// Remueve una solución de la solucion.
        /// </summary>
        /// <param name="solucion">Solución a remover.</param>
        public void QuitarSolucion(Solucion solucion)
        {
            this._Soluciones.Remove(solucion);
            this._SeHaModificado = true;
            this.eventoAlQuitarSolucion(new SolucionEnEtapaEventArgs(solucion));
        }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(this.Nombre) ? "Etapa" : this.Nombre) + "[" + this.Soluciones.Length + "]";
        }

        #endregion


        #region PostgreSQL


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

        #endregion


        #region Definición de Eventos

        /// <summary>
        /// Se desencadena cuando la propiedad EnSimulacion adquiere el valor TRUE.
        /// </summary>
        public event EventHandler AlIniciarSimulacion;

        /// <summary>
        /// Se desencadena cuando la propiedad EnSimulacion adquiere el valor FALSE.
        /// </summary>
        public event EventHandler AlDetenerSimulacion;

        /// <summary>
        /// Se desencadena cuando la propiedad Nombre cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarNombre;

        /// <summary>
        /// Se desencadena cuando la propiedad Finalizada de esta solucion cambia a TRUE.
        /// </summary>
        public event EventHandler AlFinalizarEtapa;

        /// <summary>
        /// Se desencadena cuando se agrega una nueva solución a esta solucion.
        /// </summary>
        public event System.EventHandler<SolucionEnEtapaEventArgs> AlAgregarSolucion;

        /// <summary>
        /// Se desencadena cuando se agrega un nuevo sintomas a esta solucion.
        /// </summary>
        public event EventHandler AlAgregarSintoma;

        /// <summary>
        /// Se desencadena al quitar una solución de esta solucion.
        /// </summary>
        public event System.EventHandler<Entrenamiento.Nucleo.SolucionEnEtapaEventArgs> AlQuitarSolucion;

        /// <summary>
        /// Se desencadena al quitar un sintomas de esta solucion.
        /// </summary>
        public event EventHandler AlQuitarSintoma;

        /// <summary>
        /// Se desencadena cuando se avanza a una nueva solución. El evento no se desencadena cuando se ha llegado al final.
        /// </summary>
        public event EventHandler AlAvanzar;


        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlFinalizarEtapa.
        /// </summary>
        private void eventoAlFinalizarEtapa(EventArgs e)
        {
            if (this.AlFinalizarEtapa != null)
                this.AlFinalizarEtapa(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAgregarSolucion.
        /// </summary>
        private void eventoAlAgregarSolucion(SolucionEnEtapaEventArgs e)
        {
            if (this.AlAgregarSolucion != null)
                this.AlAgregarSolucion(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAgregarSintoma.
        /// </summary>
        private void eventoAlAgregarSintoma(EventArgs e)
        {
            if (this.AlAgregarSintoma != null)
                this.AlAgregarSintoma(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlQuitarSolucion.
        /// </summary>
        private void eventoAlQuitarSolucion(Entrenamiento.Nucleo.SolucionEnEtapaEventArgs e)
        {
            if (this.AlQuitarSolucion != null)
                this.AlQuitarSolucion(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlQuitarSintoma.
        /// </summary>
        private void eventoAlQuitarSintoma(EventArgs e)
        {
            if (this.AlQuitarSintoma != null)
                this.AlQuitarSintoma(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAvanzar.
        /// </summary>
        private void eventoAlAvanzar(EventArgs e)
        {
            if (this.AlAvanzar != null)
                this.AlAvanzar(this, e);
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
        /// Aquí se mandan llamar todos los delegados asignados del evento AlIniciarSimulacion.
        /// </summary>
        private void eventoAlIniciarSimulacion(EventArgs e)
        {
            if (this.AlIniciarSimulacion != null)
                this.AlIniciarSimulacion(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlDetenerSimulacion.
        /// </summary>
        private void eventoAlDetenerSimulacion(EventArgs e)
        {
            if (this.AlDetenerSimulacion != null)
                this.AlDetenerSimulacion(this, e);
        }

        #endregion


        #region Constructores

        public Etapa()
        {
            this._init(string.Empty, null, null);
        }

        public Etapa(string Nombre)
        {
            this._init(Nombre, null, null);
        }

        public Etapa(string Nombre, Sintoma[] sintomas, Solucion[] soluciones)
        {
            this._init(Nombre, sintomas, soluciones);
        }

        private void _init(string Nombre, Sintoma[] sintomas, Solucion[] soluciones)
        {
            this._Soluciones = new List<Solucion>();
            this._Sintomas = new List<Sintoma>();

            if (sintomas != null)
                this._Sintomas.AddRange(sintomas);

            if (soluciones != null)
                this._Soluciones.AddRange(soluciones);
        }

        #endregion
    }
}