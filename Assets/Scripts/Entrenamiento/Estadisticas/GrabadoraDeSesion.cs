using System;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using UnityEngine;

namespace Entrenamiento.Estadisticas
{
    /// <summary>
    /// Estructura que representa un conjunto con el nombre del interruptor y su estado.
    /// </summary>
    public struct NombreDeInterruptor_EstadosDeInterruptor : Comunicacion_JSON.IToJsonFx
    {
        public NombresDeInterruptores NombresDeInterruptores;
        public EstadosDeInterruptores EstadosDeInterruptores;

        public NombreDeInterruptor_EstadosDeInterruptor(NombresDeInterruptores NombresDeInterruptores, EstadosDeInterruptores EstadosDeInterruptores)
        {
            this.NombresDeInterruptores = NombresDeInterruptores;
            this.EstadosDeInterruptores = EstadosDeInterruptores;
        }

        public string toJSON()
        {
            return "{" +
                "\"NombresDeInterruptores\":" + (int)this.NombresDeInterruptores + ", " +
                "\"EstadosDeInterruptores\":" + (int)this.EstadosDeInterruptores +
                "}";
        }
    }

    /// <summary>
    /// Estructura que representa un conjunto con el nombre del Instrumento y sus Valores.
    /// </summary>
    public struct NombreDeInstrumento_ValoresDeInstrumento : Comunicacion_JSON.IToJsonFx
    {
        public NombresDeInstrumentos NombresDeInstrumentos;
        public ValoresDeInstrumento ValoresDeInstrumento;

        public NombreDeInstrumento_ValoresDeInstrumento(NombresDeInstrumentos NombresDeInstrumentos, ValoresDeInstrumento ValoresDeInstrumento)
        {
            this.NombresDeInstrumentos = NombresDeInstrumentos;
            this.ValoresDeInstrumento = ValoresDeInstrumento;
        }

        public string toJSON()
        {
            return "{" +
                "\"NombresDeInstrumentos\":" + (int)this.NombresDeInstrumentos + ", " +
                "\"ValoresDeInstrumento\":" + this.ValoresDeInstrumento.toJSON() +
                "}";
        }
    }


    public class GrabadoraDeSesion : Comunicacion_JSON.Modelo
    {
        #region Constructores

        /// <summary>
        /// 
        /// </summary>
        /// <param name="escenario">Escenario que se va a grabar.</param>
        public GrabadoraDeSesion(Escenario escenario)
        {
            this._initialize(escenario);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="escenario">Escenario que se va a grabar.</param>
        /// <param name="interruptores">Interruptores que se grabarán.</param>
        /// <param name="instrumentos">Instrumentos que se grabarán.</param>
        public GrabadoraDeSesion(Escenario escenario, Interruptor[] interruptores, Instrumento[] instrumentos)
        {
            this._initialize(escenario);
            
            this.Interruptores = interruptores;
            this.Instrumentos = instrumentos;
        }

        /// <summary>
        /// Inicializa la clase GrabadoraDeSesion.
        /// </summary>
        /// <param name="escenario">Escenario que se va a grabar.</param>
        private void _initialize(Escenario escenario)
        {
            this.grabacionDeInstrumentosIndexada = new Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]>();
            this.diccionarioGrabacionDeInstrumentos = new Dictionary<NombresDeInstrumentos, GrabacionDeInstrumento>();
            this.diccionarioGrabacionDeInterruptores = new Dictionary<NombresDeInterruptores, GrabacionDeInterruptor>();
            this.hitos = new List<Hito>();

            this.Escenario = escenario;
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando el valor de la propiedad GrabacionIniciada cambia.
        /// </summary>
        public event EventHandler AlCambiarGrabacionIniciada;

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarGrabacionIniciada.
        /// </summary>
        private void eventoAlCambiarGrabacionIniciada(EventArgs e)
        {
            if (this.AlCambiarGrabacionIniciada != null)
                this.AlCambiarGrabacionIniciada(this, e);
        }

        #endregion


        #region Campos privados

        /// <summary>
        /// Momento exacto (desde que se empezó a ejecutar Unity) segundos del inicio de la simulación.
        /// </summary>
        private float tiempoDeInicio = -1;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<NombresDeInterruptores, GrabacionDeInterruptor> diccionarioGrabacionDeInterruptores;

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<NombresDeInstrumentos, GrabacionDeInstrumento> diccionarioGrabacionDeInstrumentos;

        #endregion


        #region Propiedades

        private bool grabacionIniciada = false;
        /// <summary>
        /// Obtiene o establece un valor indicando si se está grabando actualmente.
        /// </summary>
        public bool GrabacionIniciada
        {
            get
            {
                return this.grabacionIniciada;
            }
            set
            {
                if (this.grabacionIniciada != value)
                {
                    this.grabacionIniciada = value;

                    if (this.grabacionIniciada)
                    {// Asignamos los delegados
                        foreach (GrabacionDeInterruptor gi in this.GrabacionDeInterruptores)
                        {
                            gi.Interruptor.AlCambiarSuEstado += this.Interruptor_AlCambiarSuEstado;
                        }

                        foreach (GrabacionDeInstrumento gi in this.GrabacionDeInstrumentos)
                        {
                            gi.Instrumento.AlCambiarValor += this.Instrumento_AlCambiarValor;
                            gi.Instrumento.AlEntrarEnAdvertencia += this.Instrumento_AlEntrarEnAdvertencia;
                            gi.Instrumento.AlEntrarEnAlerta += this.Instrumento_AlEntrarEnAlerta;
                        }
                    }
                    else
                    {// Quitamos los delegados
                        foreach (GrabacionDeInterruptor gi in this.GrabacionDeInterruptores)
                        {
                            gi.Interruptor.AlCambiarSuEstado -= this.Interruptor_AlCambiarSuEstado;
                        }

                        foreach (GrabacionDeInstrumento gi in this.GrabacionDeInstrumentos)
                        {
                            gi.Instrumento.AlCambiarValor -= this.Instrumento_AlCambiarValor;
                            gi.Instrumento.AlEntrarEnAdvertencia -= this.Instrumento_AlEntrarEnAdvertencia;
                            gi.Instrumento.AlEntrarEnAlerta -= this.Instrumento_AlEntrarEnAlerta;
                        }
                    }

                    this.eventoAlCambiarGrabacionIniciada(EventArgs.Empty);
                }
            }
        }


        Interruptor[] interruptores;
        /// <summary>
        /// Obtiene o establece los interruptores de esta grabadora.
        /// </summary>
        Interruptor[] Interruptores
        {
            get
            {
                return this.interruptores;
            }
            set
            {
                if (this.interruptores != value)
                {
                    this.interruptores = value;
                    this.diccionarioGrabacionDeInterruptores.Clear();

                    if (this.interruptores != null)
                    {
                        foreach (Interruptor interruptor in this.interruptores)
                        {
                            this.diccionarioGrabacionDeInterruptores.Add(interruptor.Nombre, new GrabacionDeInterruptor(interruptor));
                        }
                    }
                }
            }
        }


        Instrumento[] instrumentos;
        /// <summary>
        /// Obtiene o establece los instrumentos de esta grabadora.
        /// </summary>
        Instrumento[] Instrumentos
        {
            get
            {
                return this.instrumentos;
            }
            set
            {
                if (this.instrumentos != value)
                {
                    this.instrumentos = value;
                    this.diccionarioGrabacionDeInstrumentos.Clear();

                    if (this.instrumentos != null)
                    {
                        foreach (Instrumento instrumento in this.instrumentos)
                        {
                            this.diccionarioGrabacionDeInstrumentos.Add(instrumento.Nombre, new GrabacionDeInstrumento(instrumento));
                        }
                    }
                }
            }
        }


        private uint tiempoDeFinalizacion = 0;
        /// <summary>
        /// Tiempo final en milisegundos de la simulación. Sólo será diferente de 0 cuando se haya finalizado el Escenario asignado.
        /// </summary>
        public uint TiempoDeFinalizacion
        {
            get
            {
                return this.tiempoDeFinalizacion;
            }
        }

        
        /// <summary>
        /// Obtiene el tiempo en milisegundos que han pasado desde que inició la simulación del escenario asignado.
        /// </summary>
        public uint deltaTime
        {
            get
            {
                if (this.tiempoDeInicio == -1)
                    return 0;

                return Convert.ToUInt32((Time.time - this.tiempoDeInicio) * 1000);// Convertimos a milisegundos. (1s = 1000 ms)
            }
        }


        private Escenario escenario;
        /// <summary>
        /// Obtiene el escenario de esta grabadora.
        /// </summary>
        public Escenario Escenario
        {
            get
            {
                return this.escenario;
            }
            set
            {
                if (this.escenario != null)
                {
                    this.escenario.AlIniciarSimulacion -= this.escenario_AlIniciarSimulacion;
                    //this.escenario.AlDetenerSimulacion -= this.escenario_AlDetenerSimulacion;
                    //this.escenario.AlFinalizarEscenario -= this.escenario_AlFinalizarEscenario;

                    //foreach (Etapa etapa in this.escenario.Etapas)
                    //{
                    //    etapa.AlFinalizarEtapa -= Etapa_AlFinalizarEtapa;
                    //    foreach (Solucion solucion in etapa.Soluciones)
                    //    {
                    //        solucion.AlAnalizarNuevoEstadoDelInterruptor -= solucion_AlAnalizarNuevoEstadoDelInterruptor;
                    //    }
                    //}
                }

                this.escenario = value;

                if (this.escenario != null)
                {
                    this.escenario.AlIniciarSimulacion += this.escenario_AlIniciarSimulacion;
                    //this.escenario.AlDetenerSimulacion += this.escenario_AlDetenerSimulacion;
                    //this.escenario.AlFinalizarEscenario += this.escenario_AlFinalizarEscenario;

                    //foreach (Etapa etapa in this.escenario.Etapas)
                    //{
                    //    etapa.AlFinalizarEtapa += Etapa_AlFinalizarEtapa;
                    //    foreach (Solucion solucion in etapa.Soluciones)
                    //    {
                    //        solucion.AlAnalizarNuevoEstadoDelInterruptor += solucion_AlAnalizarNuevoEstadoDelInterruptor;
                    //    }
                    //}
                }
            }
        }


        /// <summary>
        /// Obtiene la grabaciön de los interruptores.
        /// </summary>
        public GrabacionDeInterruptor[] GrabacionDeInterruptores
        {
            get
            {
                GrabacionDeInterruptor[] aux = new GrabacionDeInterruptor[this.diccionarioGrabacionDeInterruptores.Count];
                this.diccionarioGrabacionDeInterruptores.Values.CopyTo(aux, 0);
                return aux;
            }
        }


        /// <summary>
        /// Obtiene la grabaciön de los instrumentos.
        /// </summary>
        public GrabacionDeInstrumento[] GrabacionDeInstrumentos
        {
            get
            {
                GrabacionDeInstrumento[] aux = new GrabacionDeInstrumento[this.diccionarioGrabacionDeInstrumentos.Count];
                this.diccionarioGrabacionDeInstrumentos.Values.CopyTo(aux, 0);
                return aux;
            }
        }


        private List<Hito> hitos;
        /// <summary>
        /// Obtiene todos los hitos de esta grabación.
        /// </summary>
        public Hito[] Hitos
        {
            get
            {
                return this.hitos.ToArray();
            }
        }


        private Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]> grabacionDeInstrumentosIndexada;
        /// <summary>
        /// Obtiene la grabaciön de los instrumentos indexada por su Delta.
        /// </summary>
        public Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]> GrabacionDeInstrumentosIndexada
        {
            get
            {
                if (this._SeHaModificado)
                {
                    this.grabacionDeInstrumentosIndexada = 
                        this.ObtenerGrabacionDeInstrumentosIndexada(this.GrabacionDeInstrumentos);
                }

                return this.grabacionDeInstrumentosIndexada;
            }
        }


        private Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]> grabacionDeInterruptoresIndexada;
        /// <summary>
        /// Obtiene la grabaciön de los interruptores indexada por su Delta.
        /// </summary>
        public Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]> GrabacionDeInterruptoresIndexada
        {
            get
            {
                if (this._SeHaModificado)
                {
                    this.grabacionDeInterruptoresIndexada = 
                        this.ObtenerGrabacionDeInterruptoresIndexada(this.GrabacionDeInterruptores);
                }

                return this.grabacionDeInterruptoresIndexada;
            }
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Obtiene la grabación de los instrumentos indexada por delta.
        /// </summary>
        private Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]> ObtenerGrabacionDeInstrumentosIndexada(GrabacionDeInstrumento[] grabacion)
        {
            Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]> eventosIndexados
                = new Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]>();
            List<NombreDeInstrumento_ValoresDeInstrumento> lista = new List<NombreDeInstrumento_ValoresDeInstrumento>();
            NombreDeInstrumento_ValoresDeInstrumento aux;
            foreach (GrabacionDeInstrumento g in grabacion)
            {
                aux.NombresDeInstrumentos = g.Instrumento.Nombre;
                foreach (EventoDeInstrumento ev in g.Eventos)
                {
                    aux.ValoresDeInstrumento = ev.ValoresDelInstrumento;
                    if (!eventosIndexados.ContainsKey(ev.Delta))
                    {
                        eventosIndexados.Add(ev.Delta, new NombreDeInstrumento_ValoresDeInstrumento[0]);
                    }
                    
                    lista.Clear();// Limpiamos los anteriores
                    lista.AddRange(eventosIndexados[ev.Delta]);// Agregamos los correspondientes
                    lista.Add(aux);// Se agrega el nuevo
                    eventosIndexados[ev.Delta] = lista.ToArray();// y sustituimos
                }
            }
            return eventosIndexados;
        }

        /// <summary>
        /// Obtiene la grabación de los interruptores indexada por delta.
        /// </summary>
        private Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]> ObtenerGrabacionDeInterruptoresIndexada(GrabacionDeInterruptor[] grabacion)
        {
            Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]> eventosIndexados
                = new Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]>();
            List<NombreDeInterruptor_EstadosDeInterruptor> lista = new List<NombreDeInterruptor_EstadosDeInterruptor>();
            NombreDeInterruptor_EstadosDeInterruptor aux;
            foreach (GrabacionDeInterruptor g in grabacion)
            {
                aux.NombresDeInterruptores = g.Interruptor.Nombre;
                foreach (EventoDeInterruptor ev in g.Eventos)
                {
                    aux.EstadosDeInterruptores = ev.EstadoDelInterruptor;
                    if (!eventosIndexados.ContainsKey(ev.Delta))
                    {
                        eventosIndexados.Add(ev.Delta, new NombreDeInterruptor_EstadosDeInterruptor[0]);
                    }

                    lista.Clear();// Limpiamos los anteriores
                    lista.AddRange(eventosIndexados[ev.Delta]);// Agregamos los correspondientes
                    lista.Add(aux);// Se agrega el nuevo
                    eventosIndexados[ev.Delta] = lista.ToArray();// y sustituimos
                }
            }
            return eventosIndexados;
        }

        /// <summary>
        /// Quita todos los eventos grabados hasta ahora, es decir, que regresa la grabadora a su estado inicial.
        /// </summary>
        public void LimpiarGrabacion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Quita todos los delegados relacionados con la grabación de la sesión.
        /// </summary>
        private void QuitarLosDelegadosDelEscenario()
        {
            this.escenario.AlDetenerSimulacion -= this.escenario_AlDetenerSimulacion;
            this.escenario.AlFinalizarEscenario -= this.escenario_AlFinalizarEscenario;

            foreach (Etapa etapa in this.escenario.Etapas)
            {
                etapa.AlFinalizarEtapa -= Etapa_AlFinalizarEtapa;
                foreach (Solucion solucion in etapa.Soluciones)
                {
                    solucion.AlAnalizarNuevoEstadoDelInterruptor -= solucion_AlAnalizarNuevoEstadoDelInterruptor;
                }
            }
        }

        /// <summary>
        /// Asigna todos los delegados relacionados con la grabación de la sesión.
        /// </summary>
        private void AsignarLosDelegadosDelEscenario()
        {
            this.escenario.AlDetenerSimulacion += this.escenario_AlDetenerSimulacion;
            this.escenario.AlFinalizarEscenario += this.escenario_AlFinalizarEscenario;

            foreach (Etapa etapa in this.escenario.Etapas)
            {
                etapa.AlFinalizarEtapa += Etapa_AlFinalizarEtapa;
                foreach (Solucion solucion in etapa.Soluciones)
                {
                    solucion.AlAnalizarNuevoEstadoDelInterruptor += solucion_AlAnalizarNuevoEstadoDelInterruptor;
                }
            }
        }

        #endregion


        #region Eventos relacionados a la simulación del Escenario

        private void escenario_AlIniciarSimulacion(object sender, EventArgs e)
        {
            this.tiempoDeInicio = Time.time;// Necesario para el deltaTime.
            this.GrabacionIniciada = true;
            this.AsignarLosDelegadosDelEscenario();
        }

        private void escenario_AlDetenerSimulacion(object sender, EventArgs e)
        {
            // Quitamos todos los eventos relacionados con la simulación.
            this.QuitarLosDelegadosDelEscenario();
        }

        private void escenario_AlFinalizarEscenario(object sender, EventArgs e)
        {
            this.tiempoDeFinalizacion = this.deltaTime;
            this.GrabacionIniciada = false;
            this.QuitarLosDelegadosDelEscenario();
            this._SeHaModificado = true;
        }

        private void Etapa_AlFinalizarEtapa(object sender, EventArgs e)
        {
            Etapa etapa = (Etapa)sender;
            this.hitos.Add(new Hito(etapa.Nombre, TiposDeHitos.Fin_de_etapa, this.deltaTime));
            this._SeHaModificado = true;
        }

        private void solucion_AlAnalizarNuevoEstadoDelInterruptor(object sender, NuevoEstadoEnSolucionEventArgs e)
        {
            if (e.EraUnInterruptorValido)
            {
                if(!e.EraUnValorEsperado)
                    this.hitos.Add(new Hito(e.Descripcion, TiposDeHitos.Interruptor_esperado_pero_estado_incorrecto, this.deltaTime));
            }
            else
            {
                this.hitos.Add(new Hito(e.Descripcion, TiposDeHitos.Interruptor_no_esperado, this.deltaTime));
            }
            this._SeHaModificado = true;
        }

        #endregion


        #region Eventos de la grabación

        private void Interruptor_AlCambiarSuEstado(object sender, EventArgs e)
        {
            Interruptor i = (Interruptor)sender;
            this.diccionarioGrabacionDeInterruptores[i.Nombre].AgregarEvento(new EventoDeInterruptor(i.EstadoActual, this.deltaTime));
            this._SeHaModificado = true;
        }

        private void Instrumento_AlCambiarValor(object sender, EventArgs e)
        {
            Instrumento i = (Instrumento)sender;
            this.diccionarioGrabacionDeInstrumentos[i.Nombre].AgregarEvento(new EventoDeInstrumento((ValoresDeInstrumento)i.Valores.Clone(), this.deltaTime));
            this._SeHaModificado = true;
        }

        private void Instrumento_AlEntrarEnAlerta(object sender, EventArgs e)
        {
            Instrumento i = (Instrumento)sender;
            this.hitos.Add(new Hito(i.Nombre.ToString(), TiposDeHitos.Alerta_de_instrumento, this.deltaTime));
            this._SeHaModificado = true;
        }

        private void Instrumento_AlEntrarEnAdvertencia(object sender, EventArgs e)
        {
            Instrumento i = (Instrumento)sender;
            this.hitos.Add(new Hito(i.Nombre.ToString(), TiposDeHitos.Advertencia_de_instrumento, this.deltaTime));
            this._SeHaModificado = true;
        }

        #endregion


        #region Modelo JsonFx

        public override string toJSON()
        {
            int count;
            int countAux;
            string json = "{";

            if (!string.IsNullOrEmpty(this.id))
                json += "\"id\":\"" + this.id + "\",";

            json += "\"Interruptores\":{";
            countAux = this.diccionarioGrabacionDeInterruptores.Keys.Count;
            foreach (NombresDeInterruptores nombre in this.diccionarioGrabacionDeInterruptores.Keys)
            {
                json += "\"" + (int)nombre + "\":[";
                EventoDeInterruptor[] eventos = this.diccionarioGrabacionDeInterruptores[nombre].Eventos;
                count = eventos.Length;
                foreach (EventoDeInterruptor ev in eventos)
                {
                    json += "{\"EstadoDelInterruptor\":" + (int)ev.EstadoDelInterruptor +
                        ", \"Delta\":" + ev.Delta + "}" + (--count == 0 ? "" : ",");
                }
                json += "]" + (--countAux == 0 ? "" : ",");
            }
            json += "},";// Fin de Interruptores



            json += "\"Instrumentos\":{";
            countAux = this.diccionarioGrabacionDeInstrumentos.Keys.Count;
            foreach (NombresDeInstrumentos nombre in this.diccionarioGrabacionDeInstrumentos.Keys)
            {
                json += "\"" + (int)nombre + "\":[";
                EventoDeInstrumento[] eventos = this.diccionarioGrabacionDeInstrumentos[nombre].Eventos;
                count = eventos.Length;
                foreach (EventoDeInstrumento ev in eventos)
                {
                    json += "{\"ValoresDelInstrumento\":" + ev.ValoresDelInstrumento.toJSON() +
                        ", \"Delta\":" + ev.Delta + "}" + (--count == 0 ? "" : ",");
                }
                json += "]" + (--countAux == 0 ? "" : ",");
            }
            json += "},";// Fin de Instrumentos



            json += "\"Hitos\":[";
            count = this.hitos.Count;
            foreach (Hito hito in this.hitos)
            {
                json += "{\"Nombre\":\"" + hito.Nombre + "\",\"Info\":\"" + hito.Info + "\",\"Tipo\":" + (int)hito.Tipo +
                    ",\"Delta\":" + hito.Delta + "}" + (--count == 0 ? "" : ",");
            }
            json += "]";// Fin de Hitos


            json += "}";// Fin de objeto Json
            return json;
        }

        /// <summary>
        /// Convierte un objeto JSON devuelto por la librería JsonFX en array de este tipo.
        /// </summary>
        /// <param name="jsonFx">Objeto devuelto por el método Read() de una instancia de "JsonFx.JsonReader".</param>
        /// <returns>Array con los objeto construidos.</returns>
        public static Escenario[] ConvertirJsonEnArray(object jsonFx)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
