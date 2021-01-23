using System;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using UnityEngine;
using Entrenamiento.GUI.Instrumentos;

namespace Entrenamiento.GUI.RealizarSesion
{
    [RequireComponent(typeof(PanelDeInstrumentosInitializer))]
    public abstract class ReglasDePanelDeInstrumentos : MonoBehaviour
    {
        #region Campos privados

        private Dictionary<NombresDeInstrumentos, Instrumento> instrumentosDict;

        #endregion


        #region Eventos Unity

        private void Awake()
        {
            instrumentosDict = new Dictionary<NombresDeInstrumentos, Instrumento>();
        }

        private void Start()
        {
            InstrumentoGUIController[] instrumentosGUI = this.GetComponentsInChildren<InstrumentoGUIController>();
            foreach (InstrumentoGUIController i in instrumentosGUI)
            {
                instrumentosDict.Add(i.Instrumento.Nombre, i.Instrumento);
            }

            this.inicializarReglas();
        }

        #endregion


        #region Métodos de la clase
        
        /// <summary>
        /// Método donde se implementan las reglas.
        /// </summary>
        abstract protected void inicializarReglas();

        /// <summary>
        /// Agrega una regla a un determinado instrumento.
        /// </summary>
        /// <param name="instrumento_de_entrada">Instrumento de donde se estarán leyendo los valores.</param>
        /// <param name="regla">Regla a ejecutar.</param>
        public void AgregarRegla(NombresDeInstrumentos instrumento_de_entrada, Action<ValoresDeInstrumento> regla)
        {
            if (this.instrumentosDict.ContainsKey(instrumento_de_entrada))
            {
                this.instrumentosDict[instrumento_de_entrada].AlCambiarValor += (object sender, EventArgs e) =>
                {
                    regla(((Instrumento)sender).Valores);
                };
            }
        }

        /// <summary>
        /// Modifica el valor de un determinado instrumento.
        /// </summary>
        /// <param name="instrumento_afectado">Instrumento al que se le modificarán los valores.</param>
        /// <param name="valores">Nuevos valores a ser asignados.</param>
        public void ModificarValor(NombresDeInstrumentos instrumento_afectado, ValoresDeInstrumento valores)
        {
            if (this.instrumentosDict.ContainsKey(instrumento_afectado))
            {
                this.instrumentosDict[instrumento_afectado].Valores = valores;
            }
        }

        #endregion


        #region Métodos estáticos

        /// <summary>
        /// Le agrega el componente de reglas a un objeto dado a partir de un modelo de aeronave.
        /// </summary>
        /// <param name="panel">Panel de instrumentos al que se le aplicarán las reglas.</param>
        /// <param name="modelo">Modelo de la aeronave correspondiente al panel.</param>
        public static void AgregarReglasAPanel(GameObject panel, ModelosDeHelicoptero modelo)
        {
            switch (modelo)
            {
                case ModelosDeHelicoptero.B206L3:
                    panel.AddComponent<ReglasDePanel_B206L3>();
                    break;

                case ModelosDeHelicoptero.B206B3:
                    panel.AddComponent<ReglasDePanel_B206B3>();
                    break;

                case ModelosDeHelicoptero.B206L4:
                    panel.AddComponent<ReglasDePanel_B206L4>();
                    break;
            }
        }

        #endregion
    }
}