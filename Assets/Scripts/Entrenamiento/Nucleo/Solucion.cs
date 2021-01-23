using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class Solucion : Base_de_datos.Modelo
    {
        #region Constructores

        /// <param name="Interruptores">Lista de todos los interruptores involucrados, o bien, todos los interruptores existentes.</param>
        public Solucion(Interruptor[] Interruptores)
        {
            this.Initialize(Interruptores, null, null);
        }

        /// <param name="EstadoDeseado">Estado final para dar por terminada la solución, o bien, la secuencia a realizar para finalizarla.</param>
        /// <param name="Interruptores">Lista de todos los interruptores involucrados, o bien, todos los interruptores existentes.</param>
        public Solucion(Interruptor[] Interruptores, ParDeDatosInterruptorEstado[] EstadoDeseado)
        {
            this.Initialize(Interruptores, EstadoDeseado, null);
        }

        /// <param name="EstadoInicial">Estado inicial que deben tener los interruptores antes de inicar la solución. NULL si no es requerido.</param>
        /// <param name="EstadoDeseado">Estado final para dar por terminada la solución, o bien, la secuencia a realizar para finalizarla.</param>
        /// <param name="Interruptores">Lista de todos los interruptores involucrados, o bien, todos los interruptores existentes.</param>
        public Solucion(Interruptor[] Interruptores, ParDeDatosInterruptorEstado[] EstadoDeseado, System.Collections.Generic.Dictionary<Interruptor, EstadosDeInterruptores> EstadoInicial)
        {
            this.Initialize(Interruptores, EstadoDeseado, EstadoInicial);
        }

        /// <param name="EstadoInicial">Estado inicial que deben tener los interruptores antes de inicar la solución. NULL si no es requerido.</param>
        /// <param name="EstadoDeseado">Estado final para dar por terminada la solución, o bien, la secuencia a realizar para finalizarla.</param>
        /// <param name="Interruptores">Lista de todos los interruptores involucrados, o bien, todos los interruptores existentes.</param>
        private void Initialize(Interruptor[] Interruptores, ParDeDatosInterruptorEstado[] EstadoDeseado, System.Collections.Generic.Dictionary<Interruptor, EstadosDeInterruptores> EstadoInicial)
        {
            //this._EstadoDeInterruptoresInicial = EstadoInicial;
            this._EstadoDeInterruptoresDeseado = new List<ParDeDatosInterruptorEstado>();

            if (EstadoDeseado != null)
            {
                this._EstadoDeInterruptoresDeseado.AddRange(EstadoDeseado);
            }

            this._InterruptoresDeLaAeronave = new Dictionary<NombresDeInterruptores, Interruptor>();
            this.InterruptoresDeLaAeronave = Interruptores;

            //foreach (Interruptor i in Interruptores)
            //{
            //    this._InterruptoresDeLaAeronave.Add(i.Nombre, i);
            //}
        }

        #endregion


        #region Campos privados

        private System.Collections.Generic.List<Entrenamiento.Nucleo.ParDeDatosInterruptorEstado> _EstadoDeInterruptoresDeseado = null;
        private System.Collections.Generic.Dictionary<Interruptor, EstadosDeInterruptores> _EstadoDeInterruptoresInicial;

        /// <summary>
        /// IEnumerator necesario para mantener el estado actual de la máquina de estados.
        /// </summary>
        private IEnumerator<ParDeDatosInterruptorEstado> _IEnumeratorDeMaquina;

        /// <summary>
        /// Representación de todos los interruptores de la aeronave.
        /// </summary>
        private Dictionary<NombresDeInterruptores, Entrenamiento.Nucleo.Interruptor> _InterruptoresDeLaAeronave = null;

        #endregion


        #region Propiedades

        private bool _EnSimulacion = false;
        /// <summary>
        /// Obtiene o establece un valor que indica si la solución se está ejecutando como una sesión de entrenamiento (a la escucha de interruptores).
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
                        if (this.EstadoDeInterruptoresDeseado.Length <= 0)
                            throw new InvalidOperationException("No se pudo inicar el modo de simulación porque no hay estados de interruptor asignados.");

                        this.AlFinalizar += new EventHandler(this.Solucion_AlFinalizar);
                        this.AlIniciarSimulacion += this.Solucion_AlIniciarSimulacion;
                        this._IEnumeratorDeMaquina = null;// Reinicio la máquina de estados a su posición inicial
                        this.EstarAlPendienteDeLosInterruptores();
                        this._Finalizada = false;
                        this.eventoAlIniciarSimulacion(new EventArgs());
                    }
                    else
                    {// Detener modo de simulación
                        this.DejarDeEstarAlPendienteDeLosInterruptores();
                        this.AlFinalizar -= this.Solucion_AlFinalizar;
                        this.eventoAlDetenerSimulacion(new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene o establece los interruptores de la aeronave que se usarán para el modo simulación.
        /// </summary>
        public Interruptor[] InterruptoresDeLaAeronave
        {
            get
            {
                Interruptor[] array = new Interruptor[this._InterruptoresDeLaAeronave.Count];
                this._InterruptoresDeLaAeronave.Values.CopyTo(array, 0);
                return array;
            }
            set
            {
                if (this.EnSimulacion)
                {
                    throw new InvalidOperationException("No se pueden cambiar los interruptores de la aeronave durante la simulación.");
                }

                this._InterruptoresDeLaAeronave.Clear();
                if (value != null)
                {
                    foreach (Interruptor i in value)
                    {
                        this._InterruptoresDeLaAeronave.Add(i.Nombre, i);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene el estado que deben tener todos los interruptores para finalizar la solución, o bien, la secuencia que debe relizarse para finalizarla.
        /// </summary>
        public ParDeDatosInterruptorEstado[] EstadoDeInterruptoresDeseado
        {
            get
            {
                return this._EstadoDeInterruptoresDeseado.ToArray();
            }
        }

        /// <summary>
        /// Obtiene el número total de estados deseados de esta solución.
        /// </summary>
        public int EstadoDeseadoCount
        {
            get
            {
                return this._EstadoDeInterruptoresDeseado.Count;
            }
        }

        private bool _ElOrdenImporta;
        /// <summary>
        /// Obtiene o establece un valor indicando si el orden importa o no en esta solución. Nota: Cambiar éste valor hará que la propiedad EstadoDeInterruptoresDeseado sea reiniciada.
        /// </summary>
        public bool ElOrdenImporta
        {
            get
            {
                return this._ElOrdenImporta;
            }
            set
            {
                if (value != this._ElOrdenImporta)
                {
                    //this._EstadoDeInterruptoresDeseado.Clear();
                    this._ElOrdenImporta = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarSuOrdenacion(new EventArgs());
                    this.Clear();
                }
            }
        }

        private bool _Finalizada;
        /// <summary>
        /// Obtiene un valor indicando si ya ha finalizado esta solución (se ha completado).
        /// </summary>
        public bool Finalizada
        {
            get
            {
                return this._Finalizada;
            }
        }

        /// <summary>
        /// Obtiene o establece el estado que deben tener todos los interruptores al inicar esta solución.
        /// </summary>
        public System.Collections.Generic.Dictionary<Interruptor, EstadosDeInterruptores> EstadoDeInterruptoresInicial
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        #endregion

        
        #region Definición de eventos

        /// <summary>
        /// Se desencadena cuando la propiedad EnSimulacion adquiere el valor TRUE.
        /// </summary>
        public event EventHandler AlIniciarSimulacion;

        /// <summary>
        /// Se desencadena cuando la propiedad EnSimulacion adquiere el valor FALSE.
        /// </summary>
        public event EventHandler AlDetenerSimulacion;

        /// <summary>
        /// Se desencadena cuando la propiedad Finalizado cambia a TRUE.
        /// </summary>
        public event EventHandler AlFinalizar;

        /// <summary>
        /// Se desencadena después de haber analizado el nuevo estado de un interruptor.
        /// </summary>
        public event System.EventHandler<NuevoEstadoEnSolucionEventArgs> AlAnalizarNuevoEstadoDelInterruptor;

        /// <summary>
        /// Se desencadena cuando se avanza a un nuevo estado de interruptor (sólo aplica en soluciones ordenadas). El evento no se desencadena cuando se ha llegado al final.
        /// </summary>
        public event EventHandler AlAvanzar;

        /// <summary>
        /// Se desencadena cuando la propiedad ElOrdenImporta cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarSuOrdenacion;

        /// <summary>
        /// Se desencadena cuando se agrega un nuevo estado de interruptor a la solución.
        /// </summary>
        public event EventHandler<ParEstadoInterruptorEventArgs> AlAgregarEstadoDeInterruptor;

        /// <summary>
        /// Se desencadena cuando se quita un estado de interruptor de esta solución.
        /// </summary>
        public event EventHandler<ParEstadoInterruptorEventArgs> AlQuitarEstadoDeInterruptor;



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

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAgregarEstadoDeInterruptor.
        /// </summary>
        private void eventoAlAgregarEstadoDeInterruptor(ParEstadoInterruptorEventArgs e)
        {
            if (this.AlAgregarEstadoDeInterruptor != null)
                this.AlAgregarEstadoDeInterruptor(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlQuitarEstadoDeInterruptor.
        /// </summary>
        private void eventoAlQuitarEstadoDeInterruptor(ParEstadoInterruptorEventArgs e)
        {
            if (this.AlQuitarEstadoDeInterruptor != null)
                this.AlQuitarEstadoDeInterruptor(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlFinalizar.
        /// </summary>
        private void eventoAlFinalizar(EventArgs e)
        {
            if (this.AlFinalizar != null)
                this.AlFinalizar(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAnalizarNuevoEstadoDelInterruptor.
        /// </summary>
        private void eventoAlAnalizarNuevoEstadoDelInterruptor(NuevoEstadoEnSolucionEventArgs e)
        {
            if (this.AlAnalizarNuevoEstadoDelInterruptor != null)
                this.AlAnalizarNuevoEstadoDelInterruptor(this, e);
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
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarSuOrdenacion.
        /// </summary>
        private void eventoAlCambiarSuOrdenacion(EventArgs e)
        {
            if (this.AlCambiarSuOrdenacion != null)
                this.AlCambiarSuOrdenacion(this, e);
        }

        #endregion


        #region Métodos de la clase

        private void Solucion_AlIniciarSimulacion(object sender, EventArgs e)
        {
            if (this.EstadoCorrectoInterruptoresEnSolucionDesordenada())
            {// Ha finalizado la solución desordenada
                this._Finalizada = true;
                this.eventoAlFinalizar(new EventArgs());
                this.EnSimulacion = false;
            }
        }

        /// <summary>
        /// Añade las referencias necesarias al EventHandler de los interruptores asocioados a esta solución para estar al pendiente de lo que ocurre con ellos.
        /// </summary>
        private void EstarAlPendienteDeLosInterruptores()
        {
            foreach (Interruptor i in this._InterruptoresDeLaAeronave.Values)
            {
                i.AlCambiarSuEstado += new EventHandler(this.AnalizarCambioDeEstadoDeInterruptor);
            }
        }

        private void Solucion_AlFinalizar(object sender, EventArgs e)
        {
            this.EnSimulacion = false;
        }

        /// <summary>
        /// Limpia el EvenHandler de todos los interruptores de funciones de esta instancia.
        /// </summary>
        private void DejarDeEstarAlPendienteDeLosInterruptores()
        {
            foreach (Interruptor i in this._InterruptoresDeLaAeronave.Values)
            {
                i.AlCambiarSuEstado -= this.AnalizarCambioDeEstadoDeInterruptor;
            }
        }

        /// <summary>
        /// Maquina de estados para la realización de las soluciones ordenadas.
        /// </summary>
        /// <returns>El interruptor en el que se encuentra actualmente.</returns>
        private System.Collections.Generic.IEnumerable<ParDeDatosInterruptorEstado> MaquinaDeEstado()
        {
            foreach (ParDeDatosInterruptorEstado p in this.EstadoDeInterruptoresDeseado)
            {
                yield return p;
            }

            this._IEnumeratorDeMaquina = null;
            this._Finalizada = true;
            this.eventoAlFinalizar(new EventArgs());
            this.EnSimulacion = false;
        }

        /// <summary>
        /// Avanza al siguiente estado de la máquina de estados. Nota: Desconocido se realiza una validación aquí.
        /// </summary>
        /// <param name="interruptor">Interruptor que se evaluará para determinar si avanzará o no.</param>
        private void AvanzarAlSiguienteEstado(Interruptor interruptor)
        {
            bool existeUnCurrent = true;

            if (this._IEnumeratorDeMaquina == null)
            {// Primera validación de la solución
                this._IEnumeratorDeMaquina = this.MaquinaDeEstado().GetEnumerator();
                existeUnCurrent = this._IEnumeratorDeMaquina.MoveNext();
            }

            if (existeUnCurrent && this.ValidacionParaAvanzarAlSiguienteEstado(this._IEnumeratorDeMaquina.Current, interruptor) && this._IEnumeratorDeMaquina.MoveNext())
            {// Si entra aquí, entonces avanzó al siguiente estado
                this.eventoAlAvanzar(new EventArgs());
            }
        }

        /// <summary>
        /// Realiza la validación del interruptor con el estado esperado por la máquina de estado para determinar si puede avanzar o no.
        /// </summary>
        /// <returns>TRUE si debe avanzar al siguiente estado, de lo contrario FALSE.</returns>
        private bool ValidacionParaAvanzarAlSiguienteEstado(ParDeDatosInterruptorEstado actual, Interruptor interruptor)
        {
            if (actual.Interruptor/*.Nombre*/ == interruptor.Nombre)
            {// Interruptor esperado
                if (actual.Estado == interruptor.EstadoActual)
                {// Estado de interruptor esperado
                    this.eventoAlAnalizarNuevoEstadoDelInterruptor(new NuevoEstadoEnSolucionEventArgs(true, true));
                    return true;// Debe avanzar al siguiente estado
                }
                else
                {// Estado de interruptor NO esperado
                    this.eventoAlAnalizarNuevoEstadoDelInterruptor(new NuevoEstadoEnSolucionEventArgs(true, false));
                }
            }
            else
            {// Interruptor NO esperado
                this.eventoAlAnalizarNuevoEstadoDelInterruptor(new NuevoEstadoEnSolucionEventArgs(false, false));
            }

            return false;
        }

        /// <summary>
        /// Se analiza si el interruptor deseado cambió al estado esperado y se realizan las acciones correspondientes, que pueden ser avanzar al siguiente estado, o bien, finalizar la solución.
        /// </summary>
        /// <remarks>Este es una función que se debe asignar al eventoAlAnalizarNuevoEstadoDelInterruptor AlcambiarEstado de cada interruptor.</remarks>
        private void AnalizarCambioDeEstadoDeInterruptor(Object sender, EventArgs e)
        {
            Interruptor interruptor = sender as Interruptor;
            if (interruptor == null)
                return;

            if (this.ElOrdenImporta)
            {// Solución ordenada
                this.AvanzarAlSiguienteEstado(interruptor);
            }
            else
            {// Solución desordenada
                bool interruptorEncontrado = false;
                bool todosLosinterruptoresEnPosicionEsperada = true;

                foreach (ParDeDatosInterruptorEstado par in this._EstadoDeInterruptoresDeseado)
                {
                    if (par.Interruptor/*.Nombre*/ == interruptor.Nombre)
                    {// Es un interruptor que sí debió cambiar de estado
                        interruptorEncontrado = true;

                        if (par.Estado == interruptor.EstadoActual)
                        {// Estado correcto
                            this.eventoAlAnalizarNuevoEstadoDelInterruptor(new NuevoEstadoEnSolucionEventArgs(true, true));
                        }
                        else
                        {// Estado incorrecto
                            this.eventoAlAnalizarNuevoEstadoDelInterruptor(new NuevoEstadoEnSolucionEventArgs(true, false));
                            return;
                        }
                    }
                    else if (par.Estado != this._InterruptoresDeLaAeronave[par.Interruptor].EstadoActual)
                    {
                        todosLosinterruptoresEnPosicionEsperada = false;
                    }
                }

                if (!interruptorEncontrado)
                {// Movieron un interruptor que ni siquiera tenían que tocar
                    this.eventoAlAnalizarNuevoEstadoDelInterruptor(new NuevoEstadoEnSolucionEventArgs(false, false));
                    return;
                }

                if (todosLosinterruptoresEnPosicionEsperada)
                {// Ha finalizado la solución desordenada
                    this._Finalizada = true;
                    this.eventoAlFinalizar(new EventArgs());
                    this.EnSimulacion = false;
                }
            }
        }


        /// <summary>
        /// Evalúa si el estado de los interruptores del panel cumple con el estado de los interruptores deseado en una solución desordenada.
        /// </summary>
        /// <returns>Devuelve TRUE si todos los interruptores se encuentran en la posición deseada, de lo contrario FALSE. También devuelve FALSE si la solución es ordenada.</returns>
        private bool EstadoCorrectoInterruptoresEnSolucionDesordenada()
        {
            if (this.ElOrdenImporta)
                return false;

            foreach (ParDeDatosInterruptorEstado par in this._EstadoDeInterruptoresDeseado)
            {
                if (par.Estado != this._InterruptoresDeLaAeronave[par.Interruptor].EstadoActual)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Agrega un nuevo estado de interruptor deseado a esta solución.
        /// </summary>
        /// <param name="parDeDatosInterruptorEstado">Estado de interruptor que se agregará.</param>
        public void AgregarEstadoDeInterruptoresDeseado(Entrenamiento.Nucleo.ParDeDatosInterruptorEstado parDeDatosInterruptorEstado)
        {
            if (!this.ElOrdenImporta)
            {// Buscamos y quitamos el otro relacionado al Interruptor para que sólo exista uno.
                foreach (ParDeDatosInterruptorEstado par in this._EstadoDeInterruptoresDeseado)
                {
                    if (par.Interruptor/*.Nombre*/ == parDeDatosInterruptorEstado.Interruptor/*.Nombre*/)
                    {
                        this.QuitarEstadoDeInterruptoresDeseado(par);
                        break;
                    }
                }
            }
            this._EstadoDeInterruptoresDeseado.Add(parDeDatosInterruptorEstado);
            this._SeHaModificado = true;
            this.eventoAlAgregarEstadoDeInterruptor(new ParEstadoInterruptorEventArgs(parDeDatosInterruptorEstado));
        }

        /// <summary>
        /// Remueve un estado de interruptor deseado de esta solución.
        /// </summary>
        /// <param name="parDeDatosInterruptorEstado">Estado de interruptor que se removerá.</param>
        public void QuitarEstadoDeInterruptoresDeseado(Entrenamiento.Nucleo.ParDeDatosInterruptorEstado parDeDatosInterruptorEstado)
        {
            this._EstadoDeInterruptoresDeseado.Remove(parDeDatosInterruptorEstado);
            this._SeHaModificado = true;
            this.eventoAlQuitarEstadoDeInterruptor(new ParEstadoInterruptorEventArgs(parDeDatosInterruptorEstado));
        }

        /// <summary>
        /// Quita todos los estados de insterruptor de esta solución.
        /// </summary>
        public void Clear()
        {
            while (this._EstadoDeInterruptoresDeseado.Count > 0)
            {
                this.QuitarEstadoDeInterruptoresDeseado(0);
            }
        }

        /// <summary>
        /// Remueve un estado de interruptor deseado de esta solución.
        /// </summary>
        /// <param name="indice">Índice del estado de interruptor que se removerá.</param>
        public void QuitarEstadoDeInterruptoresDeseado(int indice)
        {
            ParEstadoInterruptorEventArgs ev = new ParEstadoInterruptorEventArgs(this._EstadoDeInterruptoresDeseado[indice]);
            this._EstadoDeInterruptoresDeseado.RemoveAt(indice);
            this._SeHaModificado = true;
            this.eventoAlQuitarEstadoDeInterruptor(ev);
        }

        #endregion


        #region Implementación PostgreSQL

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
    }
}