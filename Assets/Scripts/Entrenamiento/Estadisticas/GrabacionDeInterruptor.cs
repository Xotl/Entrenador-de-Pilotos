using System;
using System.Collections.Generic;

namespace Entrenamiento.Estadisticas
{
    public class GrabacionDeInterruptor : Base_de_datos.Modelo
    {
        #region Constructores

        /// <param name="interruptor">Interruptor en el que los eventos se van a visualizar.</param>
        public GrabacionDeInterruptor(Entrenamiento.Nucleo.Interruptor interruptor)
        {
            this._initialize(interruptor, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interruptor">Interruptor en el que los eventos se van a visualizar.</param>
        /// <param name="eventosDeInterruptor">Eventos del interruptor.</param>
        public GrabacionDeInterruptor(Entrenamiento.Nucleo.Interruptor interruptor, EventoDeInterruptor[] eventosDeInterruptor)
        {
            this._initialize(interruptor, eventosDeInterruptor);
        }

        private void _initialize(Entrenamiento.Nucleo.Interruptor interruptor, EventoDeInterruptor[] eventosDeInterruptor)
        {
            this._Interruptor = interruptor;
            this._Eventos = new List<EventoDeInterruptor>();
            this.eventosIndexados = new Dictionary<uint, Nucleo.EstadosDeInterruptores>();

            if (eventosDeInterruptor != null)
            {
                this._Eventos.AddRange(eventosDeInterruptor);
                foreach (EventoDeInterruptor ev in eventosDeInterruptor)
                {
                    this.eventosIndexados.Add(ev.Delta, ev.EstadoDelInterruptor);
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se desencadena cuando se agrega un nuevo evento.
        /// </summary>
        public event EventHandler AlAgregarEvento;

        /// <summary>
        /// Se desencadena cuando se remueve un evento.
        /// </summary>
        public event EventHandler AlQuitarEvento;


        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAgregarEvento.
        /// </summary>
        private void eventoAlAgregarEvento(EventArgs e)
        {
            if (this.AlAgregarEvento != null)
                this.AlAgregarEvento(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlQuitarEvento.
        /// </summary>
        private void eventoAlQuitarEvento(EventArgs e)
        {
            if (this.AlQuitarEvento != null)
                this.AlQuitarEvento(this, e);
        }

        #endregion


        #region Propiedades

        private List<Entrenamiento.Estadisticas.EventoDeInterruptor> _Eventos;
        /// <summary>
        /// Obtiene todos los eventos que habrán de reproducirse en el interruptor.
        /// </summary>
        public Entrenamiento.Estadisticas.EventoDeInterruptor[] Eventos
        {
            get
            {
                return this._Eventos.ToArray();
            }
        }

        private Entrenamiento.Nucleo.Interruptor _Interruptor;
        /// <summary>
        /// Obtiene el interruptor en el que los eventos se van a visualizar.
        /// </summary>
        public Entrenamiento.Nucleo.Interruptor Interruptor
        {
            get
            {
                return this._Interruptor;
            }
        }

        private Dictionary<uint, Nucleo.EstadosDeInterruptores> eventosIndexados;
        /// <summary>
        /// Obtiene todos los eventos que habrán de reproducirse en el interruptor indexados por su Delta.
        /// </summary>
        public Dictionary<uint, Nucleo.EstadosDeInterruptores> EventosIndexados
        {
            get
            {
                return this.eventosIndexados;
            }
        }

        #endregion
        

        #region Métodos de la clase

        /// <summary>
        /// Agrega un nuevo evento de grabación.
        /// </summary>
        /// <param name="evento">Evento que se desea agregar.</param>
        public void AgregarEvento(EventoDeInterruptor evento)
        {
            this.eventosIndexados.Add(evento.Delta, evento.EstadoDelInterruptor);
            this._Eventos.Add(evento);
            this._SeHaModificado = true;
            this.eventoAlAgregarEvento(new EventArgs());
        }

        /// <summary>
        /// Quita un evento de grabación.
        /// </summary>
        /// <param name="indice">Índice del evento que se desea remover.</param>
        public void QuitarEvento(int indice)
        {
            this.eventosIndexados.Remove(this._Eventos[indice].Delta);
            this._Eventos.RemoveAt(indice);
            this._SeHaModificado = true;
            this.eventoAlQuitarEvento(new EventArgs());
        }

        /// <summary>
        /// Quita un evento de grabación.
        /// </summary>
        /// <param name="evento">Evento que se desea remover.</param>
        public void QuitarEvento(Entrenamiento.Estadisticas.EventoDeInterruptor evento)
        {
            this.eventosIndexados.Remove(evento.Delta);
            this._Eventos.Remove(evento);
            this._SeHaModificado = true;
            this.eventoAlQuitarEvento(new EventArgs());
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
    }
}
