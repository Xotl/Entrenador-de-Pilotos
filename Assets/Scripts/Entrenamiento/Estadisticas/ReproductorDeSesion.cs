using System;
using System.Collections.Generic;
using Entrenamiento.Nucleo;

namespace Entrenamiento.Estadisticas
{
    public class ReproductorDeSesion
    {
        /// <summary>
        /// Clase que representa el conjunto de grabación de instrumentos e interruptores.
        /// </summary>
        private class ConjuntoDeGrabacion
        {
            public ConjuntoDeGrabacion(NombreDeInstrumento_ValoresDeInstrumento[] NombreDeInstrumento_ValoresDeInstrumento,
                NombreDeInterruptor_EstadosDeInterruptor[] NombreDeInterruptor_EstadosDeInterruptor)
            {
                this.NombreDeInstrumento_ValoresDeInstrumento = NombreDeInstrumento_ValoresDeInstrumento;
                this.NombreDeInterruptor_EstadosDeInterruptor = NombreDeInterruptor_EstadosDeInterruptor;
            }
            public NombreDeInstrumento_ValoresDeInstrumento[] NombreDeInstrumento_ValoresDeInstrumento;
            public NombreDeInterruptor_EstadosDeInterruptor[] NombreDeInterruptor_EstadosDeInterruptor;
        }


        public ReproductorDeSesion()
        {
            this.dictInterruptores = new Dictionary<NombresDeInterruptores, Interruptor>();
            this.dictInstrumentos = new Dictionary<NombresDeInstrumentos, Instrumento>();
        }

        #region Campos privados

        private Dictionary<uint, ConjuntoDeGrabacion> grabacionIndexada;

        #endregion


        #region Propiedades

        private SesionDeEntrenamiento sesionDeEntrenamiento;
        /// <summary>
        /// Obtiene o establece la sesión de entrenamiento a reproducir.
        /// </summary>
        public SesionDeEntrenamiento SesionDeEntrenamiento
        {
            get
            {
                return this.sesionDeEntrenamiento;
            }
            set
            {
                if (this.sesionDeEntrenamiento != value)
                {
                    this.sesionDeEntrenamiento = value;
                    this.grabacionIndexada = this.CalcularLineaDeTiempo(
                        this.sesionDeEntrenamiento.GrabacionDeInstrumentosIndexada,
                        this.sesionDeEntrenamiento.GrabacionDeInterruptoresIndexada,
                        this.TiempoFinal
                    );
                    this.TiempoActual = 0;
                    this.eventoAlCambiarSesionDeEntrenamiento(EventArgs.Empty);
                }
            }
        }


        private Dictionary<NombresDeInterruptores, Interruptor> dictInterruptores;
        /// <summary>
        /// Obtiene o establece los interruptores lógicos que afectará el reproductor.
        /// </summary>
        public Interruptor[] Interruptores
        {
            get
            {
                Interruptor[] interruptores = new Interruptor[this.dictInterruptores.Count];
                this.dictInterruptores.Values.CopyTo(interruptores, 0);
                return interruptores;
            }
            set
            {
                this.dictInterruptores.Clear();
                foreach (Interruptor interruptor in value)
                {
                    this.dictInterruptores.Add(interruptor.Nombre, interruptor);
                }
            }
        }


        private Dictionary<NombresDeInstrumentos, Instrumento> dictInstrumentos;
        /// <summary>
        /// Obtiene o establece los instrumentos lógicos que afectará el reproductor.
        /// </summary>
        public Instrumento[] Instrumentos
        {
            get
            {
                Instrumento[] instrumentos = new Instrumento[this.dictInstrumentos.Count];
                this.dictInstrumentos.Values.CopyTo(instrumentos, 0);
                return instrumentos;
            }
            set
            {
                this.dictInstrumentos.Clear();
                foreach (Instrumento instrumento in value)
                {
                    this.dictInstrumentos.Add(instrumento.Nombre, instrumento);
                }
            }
        }


        private uint tiempoActual = 0;
        /// <summary>
        /// Obtiene o establece en milisegundos el tiempo actual de la reproducción.
        /// </summary>
        public uint TiempoActual
        {
            get
            {
                return this.tiempoActual;
            }
            set
            {
                if (value > this.TiempoFinal)
                    value = this.TiempoFinal;

                if (this.tiempoActual != value)
                {
                    this.ReajusteDeTiempo(this.tiempoActual, value);
                    this.tiempoActual = value;
                    this.eventoAlCambiarTiempoActual(EventArgs.Empty);
                }
            }
        }


        /// <summary>
        /// Obtiene en milisegundos el tiempo final de esta reproducción.
        /// </summary>
        public uint TiempoFinal
        {
            get
            {
                return this.sesionDeEntrenamiento.TiempoDeFinalizacion;
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando la propiedad TiempoActual cambia.
        /// </summary>
        public event EventHandler AlCambiarTiempoActual;

        /// <summary>
        /// Se produce cuando la propiedad SesionDeEntrenamiento cambia.
        /// </summary>
        public event EventHandler AlCambiarSesionDeEntrenamiento;


        private void eventoAlCambiarTiempoActual(EventArgs e)
        {
            if (this.AlCambiarTiempoActual != null)
                this.AlCambiarTiempoActual(this, e);
        }

        private void eventoAlCambiarSesionDeEntrenamiento(EventArgs e)
        {
            if (this.AlCambiarSesionDeEntrenamiento != null)
                this.AlCambiarSesionDeEntrenamiento(this, e);
        }

        #endregion


        #region Métodos de la clase
        
        /// <summary>
        /// Calcula la línea del tiempo basada en los eventos de los interruptores e instrumentación y los indexa en una sola grabación.
        /// </summary>
        /// <param name="grabacionDeInstrumentosIndexada">Grabación de la instrumentación indexada por su delta.</param>
        /// <param name="grabacionDeInterruptoresIndexada">Grabación de los interruptores indexada por su delta.</param>
        /// <param name="tiempoDeFinalizacion">Tiempo en milisegundos de la finalización de la sesión.</param>
        /// <returns>Grabación indexada por su delta y que contiene los estados de los instrumentos e interruptores.</returns>
        private Dictionary<uint, ConjuntoDeGrabacion> CalcularLineaDeTiempo(
            Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]> grabacionDeInstrumentosIndexada,
            Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]> grabacionDeInterruptoresIndexada, 
            uint tiempoDeFinalizacion)
        {
            Dictionary<NombresDeInstrumentos, ValoresDeInstrumento> estadoInstrumentos = 
                new Dictionary<NombresDeInstrumentos,ValoresDeInstrumento>();
            Dictionary<NombresDeInterruptores, EstadosDeInterruptores> estadoInterruptores =
                new Dictionary<NombresDeInterruptores, EstadosDeInterruptores>();
            Dictionary<uint, ConjuntoDeGrabacion> grabacionIndexada = new Dictionary<uint, ConjuntoDeGrabacion>();

            foreach (Instrumento instrumento in this.Instrumentos)
            {// Se asignan los estados iniciales de los Instrumentos
                estadoInstrumentos.Add(instrumento.Nombre, instrumento.Valores);
            }
            
            foreach (Interruptor interruptor in this.Interruptores)
            {// Se asignan los estados iniciales de los Interruptores
                estadoInterruptores.Add(interruptor.Nombre, interruptor.EstadoActual);
            }


            // Me aseguro aseguro que siempre exista algo en delta 0
            this.AplicarCambiosAInstrumentosEInterruptores(0, grabacionDeInstrumentosIndexada, estadoInstrumentos,
                    grabacionDeInterruptoresIndexada, estadoInterruptores);
            grabacionIndexada.Add(0, this.ObtenerConjuntoDeGrabacion(estadoInstrumentos, estadoInterruptores));

            
            for (uint delta = 1; delta <= tiempoDeFinalizacion; delta++)
            {// Recorro toda la línea del tiempo en busca de cambios que guardar

                if (this.AplicarCambiosAInstrumentosEInterruptores(delta,grabacionDeInstrumentosIndexada,estadoInstrumentos,
                    grabacionDeInterruptoresIndexada,estadoInterruptores))
                {// Guardamos sólo si hay cambios de algún tipo
                    grabacionIndexada.Add(delta, this.ObtenerConjuntoDeGrabacion(estadoInstrumentos, estadoInterruptores));
                }
            }

            return grabacionIndexada;
        }

        /// <summary>
        /// Le aplica los cambios a los diccionarios de interruptores e instrumentación a partir de grabaciones indexadas y según el delta dado.
        /// </summary>
        /// <param name="delta">Delta de las grabaciones deseado.</param>
        /// <param name="grabacionDeInstrumentosIndexada">Grabación de Instrumentos indexada de donde se tomarán los valores.</param>
        /// <param name="estadoInstrumentos">Conjunto en donde se aplicarán los cambios para los Instrumentos.</param>
        /// <param name="grabacionDeInterruptoresIndexada">Grabación de Interruptores indexada de donde se tomarán los estados.</param>
        /// <param name="estadoInterruptores">Conjunto en donde se aplicarán los cambios para los Interruptores.</param>
        /// <returns>TRUE si existen cambios en el delta dado, de lo contrario FALSE.</returns>
        private bool AplicarCambiosAInstrumentosEInterruptores(uint delta,
            Dictionary<uint, NombreDeInstrumento_ValoresDeInstrumento[]> grabacionDeInstrumentosIndexada,
            Dictionary<NombresDeInstrumentos, ValoresDeInstrumento> estadoInstrumentos,
            Dictionary<uint, NombreDeInterruptor_EstadosDeInterruptor[]> grabacionDeInterruptoresIndexada,
            Dictionary<NombresDeInterruptores, EstadosDeInterruptores> estadoInterruptores)
        {
            bool cambiosExistentes = false;

            if (grabacionDeInstrumentosIndexada.ContainsKey(delta))
            {// Aplica cambios en la Instrumentación
                cambiosExistentes = true;

                foreach (NombreDeInstrumento_ValoresDeInstrumento item in grabacionDeInstrumentosIndexada[delta])
                {// Se actualiza el valor de los instrumentos
                    estadoInstrumentos[item.NombresDeInstrumentos] = item.ValoresDeInstrumento;
                }
            }

            if (grabacionDeInterruptoresIndexada.ContainsKey(delta))
            {// Aplica cambios en los Interruptores
                cambiosExistentes = true;

                foreach (NombreDeInterruptor_EstadosDeInterruptor item in grabacionDeInterruptoresIndexada[delta])
                {// Se actualiza el valor de los interruptores
                    estadoInterruptores[item.NombresDeInterruptores] = item.EstadosDeInterruptores;
                }
            }

            return cambiosExistentes;
        }

        /// <summary>
        /// Obtiene el conjunto de grabación a partir de los estados de interruptores e instrumentos.
        /// </summary>
        /// <param name="estadoInstrumentos">Objeto con la representación de los estados de los instrumentos.</param>
        /// <param name="estadoInterruptores">Objeto con la representación de los estados de los interruptores.</param>
        /// <returns></returns>
        private ConjuntoDeGrabacion ObtenerConjuntoDeGrabacion(Dictionary<NombresDeInstrumentos, ValoresDeInstrumento> estadoInstrumentos,
            Dictionary<NombresDeInterruptores, EstadosDeInterruptores> estadoInterruptores)
        {
            ConjuntoDeGrabacion conjuntoDeGrabacion = new ConjuntoDeGrabacion(
                        new NombreDeInstrumento_ValoresDeInstrumento[estadoInstrumentos.Count],
                        new NombreDeInterruptor_EstadosDeInterruptor[estadoInterruptores.Count]
                    );

            int indice = 0;
            foreach (KeyValuePair<NombresDeInstrumentos, ValoresDeInstrumento> item in estadoInstrumentos)
            {
                conjuntoDeGrabacion.NombreDeInstrumento_ValoresDeInstrumento[indice++] =
                    new NombreDeInstrumento_ValoresDeInstrumento(item.Key, item.Value);
            }

            indice = 0;
            foreach (KeyValuePair<NombresDeInterruptores, EstadosDeInterruptores> item in estadoInterruptores)
            {
                conjuntoDeGrabacion.NombreDeInterruptor_EstadosDeInterruptor[indice++] =
                    new NombreDeInterruptor_EstadosDeInterruptor(item.Key, item.Value);
            }

            return conjuntoDeGrabacion;
        }


        /// <summary>
        /// Actualiza los valores de los interruptores e instrumentación según un conjunto de grabación dado.
        /// </summary>
        /// <param name="conjuntoDeGrabacion">Conjunto con los datos que se aplicarán.</param>
        private void ActualizarValoresDeLosPaneles(ConjuntoDeGrabacion conjuntoDeGrabacion)
        {
            foreach (NombreDeInstrumento_ValoresDeInstrumento item in conjuntoDeGrabacion.NombreDeInstrumento_ValoresDeInstrumento)
            {
                if (this.dictInstrumentos.ContainsKey(item.NombresDeInstrumentos))
                    this.dictInstrumentos[item.NombresDeInstrumentos].Valores = item.ValoresDeInstrumento;
            }

            foreach (NombreDeInterruptor_EstadosDeInterruptor item in conjuntoDeGrabacion.NombreDeInterruptor_EstadosDeInterruptor)
            {
                if (this.dictInterruptores.ContainsKey(item.NombresDeInterruptores))
                    this.dictInterruptores[item.NombresDeInterruptores].EstadoActual = item.EstadosDeInterruptores;
            }
        }

        /// <summary>
        /// Actualiza los valores de los instrumentos y la posición de los interruptores según el tiempo deseado.
        /// </summary>
        /// <param name="tiempoAnterior"></param>
        /// <param name="tiempoDeseado"></param>
        private void ReajusteDeTiempo(uint tiempoAnterior, uint tiempoDeseado)
        {
            while (!this.grabacionIndexada.ContainsKey(tiempoDeseado) && tiempoDeseado > 0)
            {// Busca el delta indexado más próximo
                tiempoDeseado--;
            }

            this.ActualizarValoresDeLosPaneles(this.grabacionIndexada[tiempoDeseado]);
        }

        #endregion
    }
}