using System;
using System.Collections.Generic;
using Administración.Modelo;
using Entrenamiento.Nucleo;

namespace Entrenamiento.Estadisticas
{
    public class SesionDeEntrenamiento : Comunicacion_JSON.Modelo
    {
        #region Constructores

        internal SesionDeEntrenamiento(Piloto piloto, DateTime fecha, GrabacionDeInstrumento[] grabacionDeInstrumentacion,
            GrabacionDeInterruptor[] grabacionDeInterruptores, Hito[] hitos, Escenario escenario, ModelosDeHelicoptero modelo,
            uint TiempoDeFinalizacion)
        {
            this._initialize(piloto, fecha, grabacionDeInstrumentacion, grabacionDeInterruptores, hitos, escenario,
                modelo, TiempoDeFinalizacion);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="grabadora">Grabación de la sesión.</param>
        public SesionDeEntrenamiento(GrabadoraDeSesion grabadora, ModelosDeHelicoptero modelo)
        {
            this._initialize(null, DateTime.Now, grabadora.GrabacionDeInstrumentos, grabadora.GrabacionDeInterruptores,
                grabadora.Hitos, grabadora.Escenario, modelo, grabadora.TiempoDeFinalizacion);
        }

        /// <param name="piloto">Piloto que realiza esta sesión de entrenamiento.</param>
        /// <param name="fecha">Fecha de cuando se realiza ésta sesión de entrenamiento.</param>
        /// <param name="grabacionDeInstrumentacion">Grabación de la instrumentación de la sesión de entrenamiento.</param>
        /// <param name="grabacionDeInterruptores">Grabación de los interruptores de la sesión de entrenamiento.</param>
        /// <param name="hitos">Hitos que ocurrieron durante la sesión de entrenamiento.</param>
        /// <param name="idDelEscenario">ID del escenario en la que se desarrolló la sesión de entrenamiento.</param>
        /// <param name="modelo">Modelo de la aeronave en el que se realizó esta sesión.</param>
        private void _initialize(Piloto piloto, DateTime fecha, GrabacionDeInstrumento[] grabacionDeInstrumentacion,
            GrabacionDeInterruptor[] grabacionDeInterruptores, Hito[] hitos, Escenario escenario, ModelosDeHelicoptero modelo,
            uint TiempoDeFinalizacion)
        {
            this._Entrenador = new Entrenador(string.Empty);
            this.escenario = escenario;
            this._Piloto = piloto;
            this._Fecha = fecha;
            this.modeloDeAeronave = modelo;
            this.tiempoDeFinalizacion = TiempoDeFinalizacion;

            this._GrabacionDeInstrumentacion = grabacionDeInstrumentacion;
            this._GrabacionDeInterruptores = grabacionDeInterruptores;
            
            this._Hitos = new List<Hito>();
            if (hitos != null)
                this._Hitos.AddRange(hitos);
        }

        #endregion


        #region Campos privados
        #endregion


        #region Propiedades

        private uint tiempoDeFinalizacion = 0;
        /// <summary>
        /// Tiempo de duración en milisegundos de la simulación.
        /// </summary>
        public uint TiempoDeFinalizacion
        {
            get
            {
                return this.tiempoDeFinalizacion;
            }
        }

        private DateTime _Fecha;
        /// <summary>
        /// Obtiene la fecha en que se realizó esta sesión de entrenamiento.
        /// </summary>
        public DateTime Fecha
        {
            get
            {
                return this._Fecha;
            }
        }

        private ModelosDeHelicoptero modeloDeAeronave;
        /// <summary>
        /// Obtiene el modelo de la aeronave en el que se realizó esta sesión.
        /// </summary>
        public ModelosDeHelicoptero ModeloDeAeronave
        {
            get
            {
                return this.modeloDeAeronave;
            }
        }

        private ushort calificacionGeneral = (ushort)Calificaciones.No_calificado;
        /// <summary>
        /// Obtiene o establece la calificación general (Promedio general) de esta sesión entrenamiento.
        /// </summary>
        public ushort CalificacionGeneral
        {
            get
            {
                //decimal promedio = 0;
                //foreach (CriterioDeEvaluacion c in this.Evaluacion)
                //{
                //    promedio += c.Calificacion;
                //}

                //return Convert.ToUInt16(promedio / this.Evaluacion.Length);

                return this.calificacionGeneral;
            }
            set
            {
                if (this.calificacionGeneral != value)
                {
                    this.calificacionGeneral = value;
                    this.eventoAlCambiarAlCambiarCalificacionGeneral(EventArgs.Empty);
                }
            }
        }

        private Entrenamiento.Estadisticas.CriterioDeEvaluacion[] _Evaluacion;
        [Obsolete("Su usará una sólo la CalificacionGeneral en lugar de los criterios de evaluación", true)]
        /// <summary>
        /// Obtiene o establece los criterios de evaluación de esta sesión de entrenamiento.
        /// </summary>
        public Entrenamiento.Estadisticas.CriterioDeEvaluacion[] Evaluacion
        {
            get
            {
                return this._Evaluacion;
            }
            set
            {
                if (value == null || value.Length == 0)
                    throw new InvalidOperationException("Los criterios de evaluación no pueden ser nulos. Debe haber al menos un criterio.");

                bool sonDiferentes = false;
                if (this._Evaluacion.Length != value.Length)
                {
                    sonDiferentes = true;
                }
                else
                {// Se verifican diferencias en sus valores
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (this._Evaluacion[i].Calificacion != value[0].Calificacion || this._Evaluacion[i].Nombre != value[0].Nombre)
                        {
                            sonDiferentes = true;
                            break;
                        }
                    }
                }

                if (sonDiferentes)
                {
                    foreach (CriterioDeEvaluacion c in this._Evaluacion)
                    {// Limpia los delegados de los criterios, pues éstos ya no serán parte de este objeto.
                        c.AlCambiarCalificacion -= this.c_AlCambiarCriterio;
                        c.AlCambiarNombre -= this.c_AlCambiarCriterio;
                    }

                    this._Evaluacion = value;
                    foreach (CriterioDeEvaluacion c in this._Evaluacion)
                    {// Hay que estar al pendiente de estos nuevos criterios.
                        c.AlCambiarCalificacion += new EventHandler(this.c_AlCambiarCriterio);
                        c.AlCambiarNombre += new EventHandler(this.c_AlCambiarCriterio);
                    }
                    this._SeHaModificado = true;
                    //this.eventoAlCambiarEvaluacion(new EventArgs());
                }
            }
        }

        private Administración.Modelo.Piloto _Piloto;
        /// <summary>
        /// Obtiene los datos del piloto que realizó esta sesión de entrenamiento.
        /// </summary>
        public Administración.Modelo.Piloto Piloto
        {
            get
            {
                return this._Piloto;
            }
        }

        private Administración.Modelo.Entrenador _Entrenador;
        /// <summary>
        /// Obtiene el entrenador que realizó la evaluación de esta sesión de entrenamiento.
        /// </summary>
        public Administración.Modelo.Entrenador Entrenador
        {
            get
            {
                return this._Entrenador;
            }
            //set
            //{
            //    if (this.Entrenador.id != value.id)
            //    {
            //        this._Entrenador = value;
            //        this._SeHaModificado = true;
            //        this.eventoAlCambiarEntrenador(new EventArgs());
            //    }
            //}
        }

        private string _Observaciones;
        /// <summary>
        /// Obtiene o establece las observaciones realizadas por el entrenador.
        /// </summary>
        public string Observaciones
        {
            get
            {
                return this._Observaciones;
            }
            set
            {
                if (this._Observaciones != value)
                {
                    this._Observaciones = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarObservaciones(new EventArgs());
                }
            }
        }

        private GrabacionDeInstrumento[] _GrabacionDeInstrumentacion;
        /// <summary>
        /// Obtiene la grabación de la instrumentación de esta sesión de entrenamiento.
        /// </summary>
        public GrabacionDeInstrumento[] GrabacionDeInstrumentacion
        {
            get
            {
                return this._GrabacionDeInstrumentacion;
            }
        }

        private GrabacionDeInterruptor[] _GrabacionDeInterruptores;
        /// <summary>
        /// Obtiene la grabación de los intrumentos de esta sesión de entrenamiento.
        /// </summary>
        public GrabacionDeInterruptor[] GrabacionDeInterruptores
        {
            get
            {
                return this._GrabacionDeInterruptores;
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
                if (this._SeHaModificado || this.grabacionDeInstrumentosIndexada == null)
                {
                    this.grabacionDeInstrumentosIndexada =
                        this.ObtenerGrabacionDeInstrumentosIndexada(this.GrabacionDeInstrumentacion);
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
                if (this._SeHaModificado || this.grabacionDeInterruptoresIndexada == null)
                {
                    this.grabacionDeInterruptoresIndexada =
                        this.ObtenerGrabacionDeInterruptoresIndexada(this.GrabacionDeInterruptores);
                }

                return this.grabacionDeInterruptoresIndexada;
            }
        }

        private List<Hito> _Hitos;
        /// <summary>
        /// Obtiene los hitos de esta sesión de entrenamiento.
        /// </summary>
        public Hito[] Hitos
        {
            get
            {
                return this._Hitos.ToArray();
            }
        }

        private Escenario escenario;
        /// <summary>
        /// Obtiene el escenario en el que se desarrolló esta sesión de entrenamiento.
        /// </summary>
        public Escenario Escenario
        {
            get
            {
                return this.escenario;
            }
        }

        #endregion


        #region Definición de eventos de la clase

        //[Obsolete("Su usará sólo la CalificacionGeneral en lugar de los criterios de evaluación")]
        ///// <summary>
        ///// Se desencadena cuando algún criterio de evaluación se modifica, o bien, si se agregan o quitan criterios.
        ///// </summary>
        //public event EventHandler AlCambiarEvaluacion;

        /// <summary>
        /// Se desencadena cuando la propiedad Observaciones cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarObservaciones;

        /// <summary>
        /// Se desencadena cuando la propiedad Entrenador cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarEntrenador;

        /// <summary>
        /// Se produce cuando el valor de la propiedad CalificacionGeneral cambia.
        /// </summary>
        public event EventHandler AlCambiarCalificacionGeneral;

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarCalificacionGeneral.
        /// </summary>
        private void eventoAlCambiarAlCambiarCalificacionGeneral(EventArgs e)
        {
            if (this.AlCambiarCalificacionGeneral != null)
                this.AlCambiarCalificacionGeneral(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarObservaciones.
        /// </summary>
        private void eventoAlCambiarObservaciones(EventArgs e)
        {
            if (this.AlCambiarObservaciones != null)
                this.AlCambiarObservaciones(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarEntrenador.
        /// </summary>
        private void eventoAlCambiarEntrenador(EventArgs e)
        {
            if (this.AlCambiarEntrenador != null)
                this.AlCambiarEntrenador(this, e);
        }

        #endregion


        #region PostgreSQL

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


        private void c_AlCambiarCriterio(object sender, EventArgs e)
        {
            this._SeHaModificado = true;
            //this.eventoAlCambiarEvaluacion(e);
        }

        #endregion


        #region Modelo JsonFx

        public override string toJSON()
        {
            int count;
            int countAux;
            System.Text.StringBuilder json = new System.Text.StringBuilder();
            json.Append("{");

            if (!string.IsNullOrEmpty(this.id))
                json.Append("\"id\":\"" + this.id + "\",");

            if (this.Escenario != null)
                json.Append("\"escenario_id\":\"" + this.Escenario.id + "\",");

            json.Append("\"Entrenador_id\":\"" + this.Entrenador.id + "\",");
            json.Append("\"Observaciones\":\"" + this.Observaciones + "\",");
            json.Append("\"CalificacionGeneral\":" + (int)this.CalificacionGeneral + ",");
            json.Append("\"ModeloDeAeronave\":" + (int)this.ModeloDeAeronave + ",");
            json.Append("\"TiempoDeFinalizacion\":" + this.TiempoDeFinalizacion + ",");


            // ************************* GrabacionDeInterruptores *************************
            json.Append("\"GrabacionDeInterruptores\":[");
            count = this._GrabacionDeInterruptores.Length;
            foreach (GrabacionDeInterruptor gInterruptor in this._GrabacionDeInterruptores)
            {
                json.Append("{\"Interruptor\":{\"Nombre\":" + (int)gInterruptor.Interruptor.Nombre +
                    ", \"EstadosPermitidos\":[");

                countAux = gInterruptor.Interruptor.EstadosPermitidos.Length;
                foreach (EstadosDeInterruptores estado in gInterruptor.Interruptor.EstadosPermitidos)
                {
                    json.Append((int)estado + (--countAux == 0 ? "" : ","));
                }

                json.Append("]}, \"Eventos\":[");
                EventoDeInterruptor[] eventos = gInterruptor.Eventos;
                countAux = eventos.Length;
                foreach (EventoDeInterruptor ev in eventos)
                {
                    json.Append("{\"EstadoDelInterruptor\":" + (int)ev.EstadoDelInterruptor +
                        ", \"Delta\":" + ev.Delta + "}" + (--countAux == 0 ? "" : ","));
                }
                json.Append("]}" + (--count == 0 ? "" : ","));
            }
            json.Append("],");// Fin de Interruptores
            // *********************** Fin GrabacionDeInterruptores ***********************


            // ************************* GrabacionDeInstrumentacion *************************
            json.Append("\"GrabacionDeInstrumentacion\":[");
            count = this.GrabacionDeInstrumentacion.Length;
            foreach (GrabacionDeInstrumento gInstrumento in this.GrabacionDeInstrumentacion)
            {
                json.Append("{\"Instrumento\":{\"Nombre\":" + (int)gInstrumento.Instrumento.Nombre +
                    ", \"Tipo\":" + (int)gInstrumento.Instrumento.Tipo +
                    "}, \"Eventos\":[");

                EventoDeInstrumento[] eventos = gInstrumento.Eventos;
                countAux = eventos.Length;
                foreach (EventoDeInstrumento ev in eventos)
                {
                    json.Append("{\"ValoresDelInstrumento\":" + ev.ValoresDelInstrumento.toJSON() +
                        ", \"Delta\":" + ev.Delta + "}" + (--countAux == 0 ? "" : ","));
                }
                json.Append("]}" + (--count == 0 ? "" : ","));
            }
            json.Append("],");// Fin de GrabacionDeInstrumentos
            // *********************** Fin GrabacionDeInstrumentacion ***********************


            // ************************* Hitos *************************
            json.Append("\"Hitos\":[");
            count = this._Hitos.Count;
            foreach (Hito hito in this._Hitos)
            {
                json.Append("{\"Nombre\":\"" + hito.Nombre + "\",\"Info\":\"" + hito.Info + "\",\"Tipo\":" + (int)hito.Tipo +
                    ",\"Delta\":" + hito.Delta + "}" + (--count == 0 ? "" : ","));
            }
            json.Append("]");// Fin de Hitos
            // *********************** Fin Hitos ***********************


            json.Append("}");// Fin de objeto Json
            return json.ToString();
        }

        /// <summary>
        /// Convierte un objeto JSON devuelto por la librería JsonFX en array de este tipo.
        /// </summary>
        /// <param name="jsonFx">Objeto devuelto por el método Read() de una instancia de "JsonFx.JsonReader".</param>
        /// <returns>Array con los objeto construidos.</returns>
        public static SesionDeEntrenamiento[] ConvertirJsonEnArray(object jsonFx)
        {
            SesionDeEntrenamiento[] arrayFinal = null;
            if (jsonFx.GetType().IsArray)
            {
                object[] arrayJson = (object[])jsonFx;
                arrayFinal = new SesionDeEntrenamiento[arrayJson.Length];

                for (int i = 0; i < arrayFinal.Length; i++)
                {
                    arrayFinal[i] = ConvertirDiccionarioJsonEnObjeto(arrayJson[i] as Dictionary<string, object>);
                }
            }
            else
            {
                arrayFinal = new SesionDeEntrenamiento[1];
                arrayFinal[0] = ConvertirDiccionarioJsonEnObjeto(jsonFx as Dictionary<string, object>);
            }

            return arrayFinal;
        }


        /// <summary>
        /// Convierte un objeto diccionario obtenido a través de JsonFX en un objeto de este tipo.
        /// </summary>
        /// <param name="json">Diccionario que representa un objeto Json.</param>
        /// <returns>Escenario contruido que representa el diccionario Json.</returns>
        private static SesionDeEntrenamiento ConvertirDiccionarioJsonEnObjeto(Dictionary<string, object> json)
        {
            Dictionary<string, object> dict;
            Dictionary<string, object>[] dictArray;
            Array array;
            Array auxArray;

            DateTime fecha = Convert.ToDateTime(json["Fecha"]);
            ModelosDeHelicoptero modelo = (ModelosDeHelicoptero)Convert.ToInt32(json["ModeloDeAeronave"]);
            uint TiempoDeFinalizacion = Convert.ToUInt32(json["TiempoDeFinalizacion"]);

            
            // ***************** Obtención de grabación de interruptores *****************
            dictArray = json["GrabacionDeInterruptores"] as Dictionary<string, object>[];
            GrabacionDeInterruptor[] grabacionDeInterruptores = new GrabacionDeInterruptor[dictArray.Length];
            NombresDeInterruptores nombreDeInterruptor;
            EstadosDeInterruptores[] estadosDeInterruptores;
            Interruptor interruptor;
            EventoDeInterruptor[] eventosDeInterruptor;
            for (int i = 0; i < grabacionDeInterruptores.Length; i++)
            {
                //******************* Obtención del interruptor *******************
                dict = (Dictionary<string, object>)dictArray[i]["Interruptor"];
                nombreDeInterruptor = (NombresDeInterruptores)Convert.ToInt32(dict["Nombre"]);


                array = (Array)dict["EstadosPermitidos"];
                estadosDeInterruptores = new EstadosDeInterruptores[array.Length];
                for (int j = 0; j < estadosDeInterruptores.Length; j++)
                {
                    estadosDeInterruptores[j] = (EstadosDeInterruptores)Convert.ToInt32(array.GetValue(j));
                }

                interruptor = new Interruptor(nombreDeInterruptor, estadosDeInterruptores);
                //***************** Fin Obtención del interruptor *****************


                //******************* Obtención de eventos de Interruptor *******************
                array = (Array)dictArray[i]["Eventos"];
                eventosDeInterruptor = new EventoDeInterruptor[array.Length];
                for (int j = 0; j < eventosDeInterruptor.Length; j++)
                {
                    dict = (Dictionary<string, object>)array.GetValue(j);
                    eventosDeInterruptor[j] = new EventoDeInterruptor(
                        (EstadosDeInterruptores)Convert.ToInt32(dict["EstadoDelInterruptor"]),
                        Convert.ToUInt32(dict["Delta"])
                    );
                }
                //***************** Fin Obtención de eventos de Interruptor *****************

                grabacionDeInterruptores[i] = new GrabacionDeInterruptor(interruptor, eventosDeInterruptor);

            }
            // *************** Fin Obtención de grabación de interruptores ***************


            // ***************** Obtención de grabación de instrumentos *****************
            dictArray = (Dictionary<string, object>[])json["GrabacionDeInstrumentacion"];
            GrabacionDeInstrumento[] grabacionDeInstrumentacion = new GrabacionDeInstrumento[dictArray.Length];
            Instrumento instrumento;
            ValoresDeInstrumento valoresDeInstrumento;
            EventoDeInstrumento[] eventosDeInstrumentacion;
            for (int i = 0; i < grabacionDeInstrumentacion.Length; i++)
            {
                //******************* Obtención del instrumento *******************
                dict = (Dictionary<string, object>)dictArray[i]["Instrumento"];
                instrumento = Instrumentacion.InstanciarInstrumentoPorNombre((NombresDeInstrumentos)Convert.ToInt32(dict["Nombre"]), modelo);
                //***************** Fin Obtención del instrumento *****************

                //******************* Obtención de eventos de instrumentos *******************
                array = (Array)dictArray[i]["Eventos"];
                eventosDeInstrumentacion = new EventoDeInstrumento[array.Length];
                for (int j = 0; j < eventosDeInstrumentacion.Length; j++)
                {
                    dict = (Dictionary<string, object>)array.GetValue(j);
                    auxArray = (Array)dict["ValoresDelInstrumento"];
                    valoresDeInstrumento = new ValoresDeInstrumento();
                    valoresDeInstrumento.Cantidad = auxArray.Length;
                    for (int k = 0; k < valoresDeInstrumento.Cantidad; k++)
                    {
                        valoresDeInstrumento[k] = Convert.ToSingle(auxArray.GetValue(k));
                    }
                    
                    eventosDeInstrumentacion[j] = new EventoDeInstrumento(valoresDeInstrumento, Convert.ToUInt32(dict["Delta"]));
                }
                //***************** Fin Obtención de eventos de instrumentos *****************

                grabacionDeInstrumentacion[i] = new GrabacionDeInstrumento(instrumento, eventosDeInstrumentacion);
            }
            // *************** Fin Obtención de grabación de instrumentos ***************


            // ***************** Obtención de Hitos *****************
            dictArray = json["Hitos"] as Dictionary<string, object>[];
            Hito[] hitos = new Hito[dictArray.Length];
            for (int i = 0; i < hitos.Length; i++)
            {
                hitos[i] = new Hito(
                    dictArray[i]["Nombre"] as string,
                    (TiposDeHitos)Convert.ToInt32(dictArray[i]["Tipo"]),
                    Convert.ToUInt32(dictArray[i]["Delta"])
                );

                hitos[i].Info = dictArray[i]["Info"] as string;
            }
            // *************** Fin Obtención de Hitos ***************
            
            Escenario escenario = null;
            string observaciones = string.Empty;

            SesionDeEntrenamiento sesion = new SesionDeEntrenamiento(null, fecha, grabacionDeInstrumentacion,
                grabacionDeInterruptores, hitos, escenario, modelo, TiempoDeFinalizacion);
            sesion.id = json["id"] as string;
            sesion.Observaciones = json["Observaciones"] as string;
            sesion.CalificacionGeneral = Convert.ToUInt16(json["CalificacionGeneral"]);
            sesion._Entrenador = new Entrenador(json["Entrenador_id"] as string);
            
            dict = (Dictionary<string, object>)json["Usuario"];
            if (dict.ContainsKey("observaciones"))
                observaciones = (string)dict["observaciones"];
            
            sesion._Piloto = new Piloto((string)dict["id"], (DateTime)dict["createdAt"], (string)dict["nombre"],
                (string)dict["username"], observaciones);

            return sesion;
        }

        #endregion
    }
}
