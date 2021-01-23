using UnityEngine;
using System;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    class Instrumento_TurbOutTemp : InstrumentoGUIController
    {
        public Transform Aguja = null;
        public Vector3 RotacionPorUnidad_100_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_500_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_800_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_900_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_Mayor_a_900 = Vector3.zero;

        
        private Quaternion posInicial_0;
        private Quaternion posInicial_100;
        private Quaternion posInicial_500;
        private Quaternion posInicial_800;
        private Quaternion posInicial_900;

        #region Eventos Unity

        private void Awake()
        {
            this.posInicial_0 = this.Aguja.localRotation;
            this.posInicial_100 = this.posInicial_0;
            this.posInicial_500 = this.posInicial_100 * Quaternion.Euler(this.RotacionPorUnidad_500_o_menos * 400);
            this.posInicial_800 = this.posInicial_500 * Quaternion.Euler(this.RotacionPorUnidad_800_o_menos * 300);
            this.posInicial_900 = this.posInicial_800 * Quaternion.Euler(this.RotacionPorUnidad_900_o_menos * 100);


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
            if (valores[0] <= 100)
            {
                this.Aguja.localRotation = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_100_o_menos * valores[0]);
            }
            else if (valores[0] <= 500)
            {
                this.Aguja.localRotation = this.posInicial_100 * Quaternion.Euler(this.RotacionPorUnidad_500_o_menos * (valores[0] - 100));
            }
            else if (valores[0] <= 800)
            {
                this.Aguja.localRotation = this.posInicial_500 * Quaternion.Euler(this.RotacionPorUnidad_800_o_menos * (valores[0] - 500));
            }
            else if (valores[0] <= 900)
            {
                this.Aguja.localRotation = this.posInicial_800 * Quaternion.Euler(this.RotacionPorUnidad_900_o_menos * (valores[0] - 800));
            }
            else
            {
                this.Aguja.localRotation = this.posInicial_900 * Quaternion.Euler(this.RotacionPorUnidad_Mayor_a_900 * (valores[0] - 900));
            }
        }

        #endregion
    }
}
