using UnityEngine;
using System;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    class VerticalSpeedGUIController : InstrumentoGUIController
    {
        public Transform Aguja = null;

        public Vector3 RotacionPorUnidad_m3500_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_m3000_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_m2000_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_m1000_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_m500_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_0_o_menos = Vector3.zero;

        public Vector3 RotacionPorUnidad_500_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_1000_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_2000_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_3000_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_3500_o_menos = Vector3.zero;
        public Vector3 RotacionPorUnidad_Mayor_a_3500 = Vector3.zero;


        private Quaternion posInicial_m3500;
        private Quaternion posInicial_m3000;
        private Quaternion posInicial_m2000;
        private Quaternion posInicial_m1000;
        private Quaternion posInicial_m500;
        private Quaternion posInicial_0;
        private Quaternion posInicial_500;
        private Quaternion posInicial_1000;
        private Quaternion posInicial_2000;
        private Quaternion posInicial_3000;
        private Quaternion posInicial_3500;

        #region Eventos Unity

        private void Awake()
        {
            this.posInicial_0 = this.Aguja.localRotation;

            this.posInicial_500 = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_500_o_menos * 500);
            this.posInicial_1000 = this.posInicial_500 * Quaternion.Euler(this.RotacionPorUnidad_1000_o_menos * 500);
            this.posInicial_2000 = this.posInicial_1000 * Quaternion.Euler(this.RotacionPorUnidad_2000_o_menos * 1000);
            this.posInicial_3000 = this.posInicial_2000 * Quaternion.Euler(this.RotacionPorUnidad_3000_o_menos * 1000);
            this.posInicial_3500 = this.posInicial_3000 * Quaternion.Euler(this.RotacionPorUnidad_3500_o_menos * 500);

            this.posInicial_m500 = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_0_o_menos * -500);
            this.posInicial_m1000 = this.posInicial_m500 * Quaternion.Euler(this.RotacionPorUnidad_m500_o_menos * -500);
            this.posInicial_m2000 = this.posInicial_m1000 * Quaternion.Euler(this.RotacionPorUnidad_m1000_o_menos * -1000);
            this.posInicial_m3000 = this.posInicial_m2000 * Quaternion.Euler(this.RotacionPorUnidad_m2000_o_menos * -1000);
            this.posInicial_m3500 = this.posInicial_m3000 * Quaternion.Euler(this.RotacionPorUnidad_m3000_o_menos * -500);


            this.AlCambiarValor += Instrumento_TurbOutTemp_AlCambiarValor;
        }

        #endregion


        #region Métodos de la clase

        private void Instrumento_TurbOutTemp_AlCambiarValor(object sender, EventArgs e)
        {
            this.ActualizarAgujas(this.Instrumento.Valores);
        }

        private void ActualizarAgujas(ValoresDeInstrumento valores)
        {
            if (valores[0] <= -3500)
            {
                this.Aguja.localRotation = this.posInicial_m3500 * Quaternion.Euler(this.RotacionPorUnidad_m3500_o_menos * (valores[0] + 3500));
            }
            else if (valores[0] <= -3000)
            {
                this.Aguja.localRotation = this.posInicial_m3000 * Quaternion.Euler(this.RotacionPorUnidad_m3000_o_menos * (valores[0] + 3000));
            }
            else if (valores[0] <= -2000)
            {
                this.Aguja.localRotation = this.posInicial_m2000 * Quaternion.Euler(this.RotacionPorUnidad_m2000_o_menos * (valores[0] + 2000));
            }
            else if (valores[0] <= -1000)
            {
                this.Aguja.localRotation = this.posInicial_m1000 * Quaternion.Euler(this.RotacionPorUnidad_m1000_o_menos * (valores[0] + 1000));
            }
            else if (valores[0] <= -500)
            {
                this.Aguja.localRotation = this.posInicial_m500 * Quaternion.Euler(this.RotacionPorUnidad_m500_o_menos * (valores[0] + 500));
            }
            else if (valores[0] <= 0)
            {
                this.Aguja.localRotation = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_0_o_menos * valores[0]);
            }




            else if (valores[0] <= 500)
            {
                this.Aguja.localRotation = this.posInicial_0 * Quaternion.Euler(this.RotacionPorUnidad_500_o_menos * valores[0]);
            }
            else if (valores[0] <= 1000)
            {
                this.Aguja.localRotation = this.posInicial_500 * Quaternion.Euler(this.RotacionPorUnidad_1000_o_menos * (valores[0] - 500));
            }
            else if (valores[0] <= 2000)
            {
                this.Aguja.localRotation = this.posInicial_1000 * Quaternion.Euler(this.RotacionPorUnidad_2000_o_menos * (valores[0] - 1000));
            }
            else if (valores[0] <= 3000)
            {
                this.Aguja.localRotation = this.posInicial_2000 * Quaternion.Euler(this.RotacionPorUnidad_3000_o_menos * (valores[0] - 2000));
            }
            else if (valores[0] <= 3500)
            {
                this.Aguja.localRotation = this.posInicial_3000 * Quaternion.Euler(this.RotacionPorUnidad_3500_o_menos * (valores[0] - 3000));
            }
            else
            {
                this.Aguja.localRotation = this.posInicial_3500 * Quaternion.Euler(this.RotacionPorUnidad_Mayor_a_3500 * (valores[0] - 3500));
            }
        }

        #endregion
    }
}
