using System;
using System.Collections.Generic;

namespace Entrenamiento.Nucleo
{
    public class EtapaEnEscenarioEventArgs : EventArgs
    {
        private Etapa _Etapa;

        /// <param name="etapa">Etapa involucrada.</param>
        public EtapaEnEscenarioEventArgs(Etapa etapa)
        {
            this._Etapa = etapa;
        }

        /// <summary>
        /// Obtiene la Etapa Involucrada en el evento.
        /// </summary>
        public Etapa Etapa
        {
            get
            {
                return this._Etapa;
            }
        }
    }
    
    public class Escenario : Comunicacion_JSON.Modelo
    {
        #region Campos privados

        /// <summary>
        /// Necesario para mantener el estado de la máquina de estados.
        /// </summary>
        private IEnumerator<Etapa> _IEnumeratorDeMaquina;

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
                if (this.EtapaActual == null)
                    return null;

                return this.EtapaActual.SolucionActual;
            }
        }

        private Etapa etapaActual;
        /// <summary>
        /// Obtiene la etapa actual de la simulación.
        /// </summary>
        public Etapa EtapaActual
        {
            get
            {
                return this.etapaActual;
            }
        }

        private bool _EnSimulacion;
        /// <summary>
        /// Obtiene o establece un valor que indica si el escenario se está ejecutando como una sesión de entrenamiento.
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
                        if (this.Etapas.Length <= 0)
                            throw new InvalidOperationException("No se pudo inicar el modo de simulación porque no hay etapas asignadas.");

                        this._IEnumeratorDeMaquina = null;// Reinicio la máquina de estados a su posición inicial
                        this._Finalizado = false;
                        this.eventoAlIniciarSimulacion(new EventArgs());
                        this.AvanzarALaSiguienteEtapa();
                    }
                    else
                    {// Detener modo de simulación
                        foreach (Etapa e in this.Etapas)
                        {// Detiene cualquier solucion en modo simulación
                            e.EnSimulacion = false;
                        }
                        this.eventoAlDetenerSimulacion(new EventArgs());
                    }
                }
            }
        }


        private string _Nombre;
        /// <summary>
        /// Obtiene o establece el nombre del escenario.
        /// </summary>
        public string Nombre
        {
            get
            {
                return this._Nombre;
            }
            set
            {
                value = value.Trim();
                if (value != this._Nombre)
                {
                    this._Nombre = value;
                    this._SeHaModificado = true;
                    this.eventoAlCambiarNombre(new EventArgs());
                }
            }
        }


        private bool _Finalizado;
        /// <summary>
        /// Obtiene un valor indicando si el escenario ya se ha finalizado o no.
        /// </summary>
        public bool Finalizado
        {
            get
            {
                return this._Finalizado;
            }
        }


        private List<Etapa> _Etapas = null;
        /// <summary>
        /// Obtiene un arreglo de las etapas de este escenario.
        /// </summary>
        public Etapa[] Etapas
        {
            get
            {
                return this._Etapas.ToArray();
            }
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Agrega una nueva solucion a este escenario.
        /// </summary>
        /// <param name="solucion">solucion que se desea agregar.</param>
        public void AgregarEtapa(Etapa etapa)
        {
            this._Etapas.Add(etapa);
            this._SeHaModificado = true;
            this.eventoAlAgregarEtapa(new EtapaEnEscenarioEventArgs(etapa));
        }

        /// <summary>
        /// Remueve una solucion de este escenario.
        /// </summary>
        /// <param name="indice">Índice donde de solucion que se desea remover.</param>
        public void QuitarEtapa(int indice)
        {
            Etapa etapa = this._Etapas[indice];
            this._Etapas.RemoveAt(indice);
            this._SeHaModificado = true;
            this.eventoAlQuitarEtapa(new EtapaEnEscenarioEventArgs(etapa));
        }

        /// <summary>
        /// Remueve una solucion de este escenario.
        /// </summary>
        /// <param name="solucion">solucion que se desea remover.</param>
        public void QuitarEtapa(Etapa etapa)
        {
            this._Etapas.Remove(etapa);
            this._SeHaModificado = true;
            this.eventoAlQuitarEtapa(new EtapaEnEscenarioEventArgs(etapa));
        }

        /// <summary>
        /// Representa la máquina de estados de este escenario.
        /// </summary>
        private IEnumerable<Etapa> MaquinaDeEstados()
        {
            Etapa eFinal = this.Etapas[this.Etapas.Length - 1];
            foreach (Etapa e in this.Etapas)
            {
                this.etapaActual = e;
                e.AlFinalizarEtapa += this.Current_AlFinalizarEtapa;
                e.AlDetenerSimulacion += this.Current_AlDetenerSimulacion;
                e.AlIniciarSimulacion += this.e_AlIniciarSimulacion;
                yield return e;
                e.AlFinalizarEtapa -= this.Current_AlFinalizarEtapa;
                e.AlDetenerSimulacion -= this.Current_AlDetenerSimulacion;
                e.AlIniciarSimulacion -= this.e_AlIniciarSimulacion;
                if(!eFinal.Equals(e))
                    this.eventoAlAvanzar(new EtapaEnEscenarioEventArgs(e));
            }

            this.etapaActual = null;
            this._IEnumeratorDeMaquina = null;
            this._Finalizado = true;
            this.eventoAlFinalizarEscenario(new EventArgs());
        }

        private void e_AlIniciarSimulacion(object sender, EventArgs e)
        {
            this.eventoAlIniciarEtapa(new EtapaEnEscenarioEventArgs((Etapa)sender));
        }

        private void Current_AlDetenerSimulacion(object sender, EventArgs e)
        {
            Etapa etapa = (Etapa)sender;
            if (!etapa.Finalizada)
                this.EnSimulacion = false; 
        }

        private void Current_AlFinalizarEtapa(object sender, EventArgs e)
        {
            this.eventoAlFinalizarEtapa(new EtapaEnEscenarioEventArgs((Etapa)sender));
            this.AvanzarALaSiguienteEtapa();
        }

        /// <summary>
        /// Avanza a la siguiente solucion para este escenario.
        /// </summary>
        private void AvanzarALaSiguienteEtapa()
        {
            if (this._IEnumeratorDeMaquina == null)
                this._IEnumeratorDeMaquina = this.MaquinaDeEstados().GetEnumerator();

            if (this._IEnumeratorDeMaquina.MoveNext() && this.etapaActual != null)
                this.etapaActual.EnSimulacion = this.EnSimulacion;
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


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando una nueva etapa ha iniciado su simulación.
        /// </summary>
        public event EventHandler<EtapaEnEscenarioEventArgs> AlIniciarEtapa;

        /// <summary>
        /// Se produce cuando la etapa en curso ha finalizado su simulación.
        /// </summary>
        public event EventHandler<EtapaEnEscenarioEventArgs> AlFinalizarEtapa;

        /// <summary>
        /// Se desencadena al finalizar el escenario.
        /// </summary>
        public event EventHandler AlFinalizarEscenario;

        /// <summary>
        /// Se desencadena cuando se agrega una nueva solucion a este escenario.
        /// </summary>
        public event System.EventHandler<EtapaEnEscenarioEventArgs> AlAgregarEtapa;

        /// <summary>
        /// Se desencadena cuando se quita una solucion a este escenario.
        /// </summary>
        public event System.EventHandler<Entrenamiento.Nucleo.EtapaEnEscenarioEventArgs> AlQuitarEtapa;

        /// <summary>
        /// Se desencadena cuando se avanza a una nueva Etapa. El evento no se desencadena cuando se ha llegado al final.
        /// </summary>
        public event EventHandler<EtapaEnEscenarioEventArgs> AlAvanzar;

        /// <summary>
        /// Se desencadena cuando se modifica el nombre del escenario.
        /// </summary>
        public event EventHandler AlCambiarNombre;

        /// <summary>
        /// Se desencadena cuando la propiedad EnSimulacion adquiere el valor TRUE.
        /// </summary>
        public event EventHandler AlIniciarSimulacion;

        /// <summary>
        /// Se desencadena cuando la propiedad EnSimulacion adquiere el valor FALSE.
        /// </summary>
        public event EventHandler AlDetenerSimulacion;

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlFinalizarEtapa.
        /// </summary>
        private void eventoAlFinalizarEtapa(EtapaEnEscenarioEventArgs e)
        {
            if (this.AlFinalizarEtapa != null)
                this.AlFinalizarEtapa(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlIniciarEtapa.
        /// </summary>
        private void eventoAlIniciarEtapa(EtapaEnEscenarioEventArgs e)
        {
            if (this.AlIniciarEtapa != null)
                this.AlIniciarEtapa(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAgregarEtapa.
        /// </summary>
        private void eventoAlAgregarEtapa(EtapaEnEscenarioEventArgs e)
        {
            if (this.AlAgregarEtapa != null)
                this.AlAgregarEtapa(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlQuitarEtapa.
        /// </summary>
        private void eventoAlQuitarEtapa(EtapaEnEscenarioEventArgs e)
        {
            if (this.AlQuitarEtapa != null)
                this.AlQuitarEtapa(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarNombre.
        /// </summary>
        private void eventoAlCambiarNombre(EventArgs e)
        {
            if (this.AlCambiarNombre != null)
                this.AlCambiarNombre(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlFinalizarEscenario.
        /// </summary>
        private void eventoAlFinalizarEscenario(EventArgs e)
        {
            if (this.AlFinalizarEscenario != null)
                this.AlFinalizarEscenario(this, e);
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
        /// Aquí se mandan llamar todos los delegados asignados del evento AlAvanzar.
        /// </summary>
        private void eventoAlAvanzar(EtapaEnEscenarioEventArgs e)
        {
            if (this.AlAvanzar != null)
                this.AlAvanzar(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlIniciarSimulacion.
        /// </summary>
        private void eventoAlIniciarSimulacion(EventArgs e)
        {
            if (this.AlIniciarSimulacion != null)
                this.AlIniciarSimulacion(this, e);
        }

        #endregion


        #region Constructores

        public Escenario()
        {
            this._initialize(string.Empty, string.Empty, null);
        }

        private void _initialize(string id, string Nombre, Etapa[] Etapas)
        {
            this._Etapas = new List<Etapa>();
            this._Id = id;
            this._Nombre = Nombre;

            if (Etapas != null)
                this._Etapas.AddRange(Etapas);
        }

        #endregion


        #region Modelo JsonFx

        /// <summary>
        /// Convierte el objeto en un string con formato JSON.
        /// </summary>
        /// <returns>Representación JSON de este objeto.</returns>
        public override string toJSON()
        {
            System.Text.StringBuilder json = new System.Text.StringBuilder();
            json.Append("{");

            if (!string.IsNullOrEmpty(this.id))
                json.Append("\"id\":\"" + this.id + "\",");

            json.Append("\"Nombre\":\"" + this.Nombre + "\",\"Etapas\":[");


            int counEtapas = this.Etapas.Length;
            foreach (Etapa etapa in this.Etapas)
            {
                json.Append("{" +
                    "\"Nombre\":\"" + etapa.Nombre + "\",\"Sintomas\":[");

                int countSintomas = etapa.Sintomas.Length;
                foreach (Sintoma sintoma in etapa.Sintomas)
                {
                    json.Append("{\"InstrumentoAfectado\":" + (int)sintoma.InstrumentoAfectado + ","
                        + "\"debug_InstrumentoAfectado\":\"" + sintoma.InstrumentoAfectado + "\","
                        + "\"Intervalo\":" + sintoma.Intervalo + ","
                        + "\"TipoDeFuncion\":" + (int)sintoma.TipoDeFuncion + ","
                        + "\"Valores\":" + sintoma.Valores.toJSON());

                    json.Append("}" + (--countSintomas == 0 ? "" : ","));
                }

                json.Append("],\"Soluciones\":[");

                int countSoluciones = etapa.Soluciones.Length;
                foreach (Solucion solucion in etapa.Soluciones)
                {

                    json.Append("{"
                        + "\"ElOrdenImporta\":" + solucion.ElOrdenImporta.ToString().ToLower() + ","
                        + "\"EstadoDeInterruptoresDeseado\":[");

                    int countPar = solucion.EstadoDeInterruptoresDeseado.Length;
                    foreach (ParDeDatosInterruptorEstado par in solucion.EstadoDeInterruptoresDeseado)
                    {
                        json.Append("{"
                            + "\"Interruptor\":" + (int)par.Interruptor + ","
                            + "\"Estado\":" + (int)par.Estado
                            + "}" + (--countPar == 0 ? "" : ","));
                    }

                    json.Append("]}" + (--countSoluciones == 0 ? "" : ","));
                }

                json.Append("]}" + (--counEtapas == 0 ? "" : ","));
            }
            json.Append("]}");

            return json.ToString();
        }

        /// <summary>
        /// Convierte un objeto JSON devuelto por la librería JsonFX en array de este tipo.
        /// </summary>
        /// <param name="jsonFx">Objeto devuelto por el método Read() de una instancia de JsonFx.JsonReader.</param>
        /// <returns>Array con los objeto construidos.</returns>
        public static Escenario[] ConvertirJsonEnArray(object jsonFx)
        {
            Escenario[] arrayFinal = null;
            if (jsonFx.GetType().IsArray)
            {
                object[] arrayJson = (object[])jsonFx;
                arrayFinal = new Escenario[arrayJson.Length];

                for (int i = 0; i < arrayFinal.Length; i++)
                {
                    arrayFinal[i] = ConvertirDiccionarioJsonEnObjeto(arrayJson[i] as Dictionary<string, object>);
                }
            }
            else
            {
                arrayFinal = new Escenario[1];
                arrayFinal[0] = ConvertirDiccionarioJsonEnObjeto(jsonFx as Dictionary<string, object>);
            }

            return arrayFinal;
        }

        /// <summary>
        /// Convierte un objeto diccionario obtenido a través de JsonFX en un objeto de este tipo.
        /// </summary>
        /// <param name="json">Diccionario que representa un objeto Json.</param>
        /// <returns>Escenario contruido que representa el diccionario Json.</returns>
        private static Escenario ConvertirDiccionarioJsonEnObjeto(Dictionary<string, object> json)
        {
            Escenario escenario = new Escenario();
            escenario.Nombre = json["Nombre"] as string;
            escenario.id = json["id"] as string;

            // ***************** Obtención de etapas *****************
            object[] arrayObjEtapas = (object[])json["Etapas"];
            Etapa etapa;
            Dictionary<string, object> auxDict;
            int count = 0;
            for (int i = 0; i < arrayObjEtapas.Length; i++)
            {
                json = arrayObjEtapas[i] as Dictionary<string, object>;
                etapa = new Etapa();
                etapa.Nombre = json["Nombre"] as string;

                
                // Obtención de síntomas
                count = ((object[])json["Sintomas"]).Length;
                Sintoma sintoma;
                for (int j = 0; j < count; j++)
                {
                    auxDict = ((object[])json["Sintomas"])[j] as Dictionary<string, object>;
                    NombresDeInstrumentos instrumento = (NombresDeInstrumentos)Convert.ToInt32(auxDict["InstrumentoAfectado"]);
                        //(NombresDeInstrumentos)Enum.Parse(typeof(NombresDeInstrumentos), auxDict["InstrumentoAfectado"] as string);
                    sintoma = new Sintoma(instrumento);
                    sintoma.Intervalo = Convert.ToInt32(auxDict["Intervalo"]);
                    sintoma.TipoDeFuncion = (TipoDeFuncionDeSintoma)auxDict["TipoDeFuncion"];


                    float[] valores = new float[((Array)auxDict["Valores"]).Length];
                    for (int k = 0; k < valores.Length; k++)
                    {
                        valores[k] = Convert.ToSingle(((Array)auxDict["Valores"]).GetValue(k));
                    }
                    
                    sintoma.Valores = valores;
                    etapa.AgregarSintoma(sintoma);
                }


                // Obtención de soluciones
                count = ((object[])json["Soluciones"]).Length;
                Solucion solucion;
                ParDeDatosInterruptorEstado auxPar =  new ParDeDatosInterruptorEstado();
                for (int j = 0; j < count; j++)
                {
                    auxDict = ((object[])json["Soluciones"])[j] as Dictionary<string, object>;
                    solucion = new Solucion(null);
                    solucion.ElOrdenImporta = (bool)auxDict["ElOrdenImporta"];

                    int cantParEstados = ((object[])auxDict["EstadoDeInterruptoresDeseado"]).Length;
                    for (int k = 0; k < cantParEstados; k++)
                    {
                        auxDict = ((object[])json["Soluciones"])[j] as Dictionary<string, object>;
                        auxDict = ((object[])auxDict["EstadoDeInterruptoresDeseado"])[k] as Dictionary<string, object>;

                        auxPar.Interruptor = (NombresDeInterruptores)Convert.ToInt32(auxDict["Interruptor"]);
                        auxPar.Estado = (EstadosDeInterruptores)Convert.ToInt32(auxDict["Estado"]);
                        solucion.AgregarEstadoDeInterruptoresDeseado(auxPar);
                    }

                    etapa.AgregarSolucion(solucion);
                }
                escenario.AgregarEtapa(etapa);
            }
            // *************** Fin obtención de etapas ***************
            
            return escenario;
        }

        #endregion
    }
}