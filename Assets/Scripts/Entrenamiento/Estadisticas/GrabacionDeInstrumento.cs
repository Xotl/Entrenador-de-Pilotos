using System;
using System.Collections.Generic;

namespace Entrenamiento.Estadisticas
{
    public class GrabacionDeInstrumento : Base_de_datos.Modelo
    {
        #region Constructores

        /// <param name="instrumento">Instrumento en el que los eventos se van a visualizar.</param>
        public GrabacionDeInstrumento(Entrenamiento.Nucleo.Instrumento instrumento)
        {
            this._initialize(instrumento, null);
        }

        /// <param name="instrumento">Instrumento en el que los eventos se van a visualizar.</param>
        public GrabacionDeInstrumento(Entrenamiento.Nucleo.Instrumento instrumento, EventoDeInstrumento[] eventos)
        {
            this._initialize(instrumento, eventos);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instrumento">Instrumento en el que los eventos se van a visualizar.</param>
        /// <param name="eventos"></param>
        private void _initialize(Entrenamiento.Nucleo.Instrumento instrumento, EventoDeInstrumento[] eventos)
        {
            this.eventosIndexados = new Dictionary<uint, Nucleo.ValoresDeInstrumento>();
            this._Eventos = new List<EventoDeInstrumento>();
            this._Instrumento = instrumento;

            if (eventos != null)
            {
                this._Eventos.AddRange(eventos);
                foreach (EventoDeInstrumento ev in eventos)
                {
                    if (this.eventosIndexados.ContainsKey(ev.Delta))
                        this.eventosIndexados[ev.Delta] = ev.ValoresDelInstrumento;
                    else
                        this.eventosIndexados.Add(ev.Delta, ev.ValoresDelInstrumento);
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

        private Entrenamiento.Nucleo.Instrumento _Instrumento;
        /// <summary>
        /// Obtiene el instrumento en el que los eventos se van a visualizar.
        /// </summary>
        public Entrenamiento.Nucleo.Instrumento Instrumento
        {
            get
            {
                return this._Instrumento;
            }
        }

        private List<Entrenamiento.Estadisticas.EventoDeInstrumento> _Eventos;
        /// <summary>
        /// Obtiene todos los eventos que habrán de reproducirse en el instrumento.
        /// </summary>
        public Entrenamiento.Estadisticas.EventoDeInstrumento[] Eventos
        {
            get
            {
                return this._Eventos.ToArray();
            }
        }

        private Dictionary<uint, Nucleo.ValoresDeInstrumento> eventosIndexados;
        /// <summary>
        /// Obtiene todos los eventos que habrán de reproducirse en el instrumento indexados por su Delta.
        /// </summary>
        public Dictionary<uint, Nucleo.ValoresDeInstrumento> EventosIndexados
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
        public void AgregarEvento(EventoDeInstrumento evento)
        {
            if (this.eventosIndexados.ContainsKey(evento.Delta))
                this.eventosIndexados[evento.Delta] = evento.ValoresDelInstrumento;
            else
                this.eventosIndexados.Add(evento.Delta, evento.ValoresDelInstrumento);

            this._Eventos.Add(evento);
            this._SeHaModificado = true;
            this.eventoAlAgregarEvento(new EventArgs());
        }

        /// <summary>
        /// Quita un evento de grabación.
        /// </summary>
        /// <param name="evento">Evento que se desea remover.</param>
        public void QuitarEvento(Entrenamiento.Estadisticas.EventoDeInstrumento evento)
        {
            this.eventosIndexados.Remove(evento.Delta);
            this._Eventos.Remove(evento);
            this._SeHaModificado = true;
            this.eventoAlQuitarEvento(new EventArgs());
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
