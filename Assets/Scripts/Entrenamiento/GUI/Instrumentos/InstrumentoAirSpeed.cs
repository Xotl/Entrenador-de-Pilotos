using UnityEngine;
using System;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    class InstrumentoAirSpeed : InstrumentoGUIController
    {
        public Transform Aguja = null;
        public Vector3 RotacionPorUnidad_20_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_40_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_60_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_80_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_100_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_120_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_140_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_Mayor_a_140 = Vector3.zero;


        private Quaternion posInicial_0;
        private Quaternion posInicial_20;
        private Quaternion posInicial_40;
        private Quaternion posInicial_60;
        private Quaternion posInicial_80;
        private Quaternion posInicial_100;
        private Quaternion posInicial_120;
        private Quaternion posInicial_140;

        #region Eventos Unity

        private void Awake()
        {
            this.posInicial_0 = this.Aguja.localRotation;
            this.posInicial_20 = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_20_o_menos * 20);
            this.posInicial_40 = this.posInicial_20 * Quaternion.Euler(this.RotacionPorUnidad_40_o_menos * 20);
            this.posInicial_60 = this.posInicial_40 * Quaternion.Euler(this.RotacionPorUnidad_60_o_menos * 20);
            this.posInicial_80 = this.posInicial_60 * Quaternion.Euler(this.RotacionPorUnidad_80_o_menos * 20);
            this.posInicial_100 = this.posInicial_80 * Quaternion.Euler(this.RotacionPorUnidad_100_o_menos * 20);
            this.posInicial_120 = this.posInicial_100 * Quaternion.Euler(this.RotacionPorUnidad_120_o_menos * 20);
            this.posInicial_140 = this.posInicial_120 * Quaternion.Euler(this.RotacionPorUnidad_140_o_menos * 20);


            this.AlCambiarValor += Instrumento_TurbOutTemp_AlCambiarValor;
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        #endregion


        #region Métodos de la clase

        private void Instrumento_TurbOutTemp_AlCambiarValor(object sender, EventArgs e)
        {
            this.ActualizarAgujas(this.Instrumento.Valores);
        }

        private void ActualizarAgujas(ValoresDeInstrumento valores)
        {
            if (valores[0] <= 20)
            {
                this.Aguja.localRotation = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_20_o_menos * valores[0]);
            }
            else if (valores[0] <= 40)
            {
                this.Aguja.localRotation = this.posInicial_20 * Quaternion.Euler(this.RotacionPorUnidad_40_o_menos * (valores[0] - 20));
            }
            else if (valores[0] <= 60)
            {
                this.Aguja.localRotation = this.posInicial_40 * Quaternion.Euler(this.RotacionPorUnidad_60_o_menos * (valores[0] - 40));
            }
            else if (valores[0] <= 80)
            {
                this.Aguja.localRotation = this.posInicial_60 * Quaternion.Euler(this.RotacionPorUnidad_80_o_menos * (valores[0] - 60));
            }
            else if (valores[0] <= 100)
            {
                this.Aguja.localRotation = this.posInicial_80 * Quaternion.Euler(this.RotacionPorUnidad_100_o_menos * (valores[0] - 80));
            }
            else if (valores[0] <= 120)
            {
                this.Aguja.localRotation = this.posInicial_100 * Quaternion.Euler(this.RotacionPorUnidad_120_o_menos * (valores[0] - 100));
            }
            else if (valores[0] <= 140)
            {
                this.Aguja.localRotation = this.posInicial_120 * Quaternion.Euler(this.RotacionPorUnidad_140_o_menos * (valores[0] - 120));
            }
            else
            {
                this.Aguja.localRotation = this.posInicial_140 * Quaternion.Euler(this.RotacionPorUnidad_Mayor_a_140 * (valores[0] - 140));
            }
        }

        #endregion
    }
}
