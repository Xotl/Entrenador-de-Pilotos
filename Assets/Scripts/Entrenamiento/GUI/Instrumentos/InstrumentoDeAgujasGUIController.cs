using UnityEngine;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    public class InstrumentoDeAgujasGUIController : InstrumentoGUIController
    {
        public enum Tipo_GUI
        {
            Cada_aguja_es_un_valor,
            Varias_agujas_un_valor,
        }

        public Transform[] Agujas;
        public Vector3[] RotacionPorUnidad;

        public Tipo_GUI GUI_Type = Tipo_GUI.Cada_aguja_es_un_valor;
        private Quaternion[] posInicial;

        #region Eventos Unity

        private void Awake()
        {
            this.posInicial = new Quaternion[this.Agujas.Length];
            for (int i = 0; i < this.Agujas.Length; i++)
            {
                if (this.Agujas[i] != null)
                    this.posInicial[i] = this.Agujas[i].localRotation;
            }

            this.AlCambiarValor += InstrumentoDobleAgujaController_AlCambiarValor;
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        #endregion


        #region Métodos de la clase

        private void InstrumentoDobleAgujaController_AlCambiarValor(object sender, System.EventArgs e)
        {
            this.ActualizarAgujas(this.Instrumento.Valores);
        }

        private void ActualizarAgujas(ValoresDeInstrumento valores)
        {
            if (this.GUI_Type == Tipo_GUI.Cada_aguja_es_un_valor)
            {
                for (int i = 0; i < this.Agujas.Length; i++)
                {
                    if (this.Agujas[i] != null)
                    {
                        this.Agujas[i].localRotation = this.posInicial[i] * Quaternion.Euler(this.RotacionPorUnidad[i] * valores[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.Agujas.Length; i++)
                {
                    if (this.Agujas[i] != null)
                    {
                        this.Agujas[i].localRotation = this.posInicial[i] * Quaternion.Euler(this.RotacionPorUnidad[i] * valores[0]);
                    }
                }
            }
        }

        #endregion
    }
}
