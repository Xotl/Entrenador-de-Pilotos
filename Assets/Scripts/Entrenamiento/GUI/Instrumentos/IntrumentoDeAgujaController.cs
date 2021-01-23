using UnityEngine;
using System;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    public class IntrumentoDeAgujaController : InstrumentoGUIController
    {
        #region Campos públicos

        /// <summary>
        /// Unidad que representa (Metros, pies, millas por hora, etc).
        /// </summary>
        public string Unidad;

        /// <summary>
        /// El/Los objeto(s) que se va(n) a girar.
        /// </summary>
        public Transform[] Agujas;

        /// <summary>
        /// El valor que cada vuelta representa. El orden corresponde al mismo que el de Agujas.
        /// </summary>
        public float[] ValorPorVuelta;

        /// <summary>
        /// Vector que indica hacia donde debe girarse (en coordenadas locales).
        /// </summary>
        public Vector3 DireccionDeRotacion;

        #endregion


        #region Campos privados

        /// <summary>
        /// Aqui se almacena la cantidad de grados que debe girar cada aguja por unidad.
        /// </summary>
        private float[] GradosPorUnidad;

        /// <summary>
        /// Obtiene la rotación de las agujas cuando el valor es 0. Nota: Este valor es obtenido a partir de la rotación inicial del objeto.
        /// </summary>
        private Quaternion[] rotacionOriginal;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene o establece el valor actual del indicador de aguja.
        /// </summary>
        public float ValorDeAguja
        {
            get
            {
                return this.Instrumento.Valores[0];
            }
            set
            {
                this.Instrumento.Valores[0] = value;
            }
        }

        #endregion


        private void Awake()
        {
            if (this.Agujas.Length > this.ValorPorVuelta.Length)
            {// Hay agujas a las que no se les ha dado un valor por vuelta
                throw new InvalidOperationException("Hay valores por vuelta no especificados para la cantidad de agujas que hay.");
            }

            this.AlCambiarValor += this.Intrumento_AlCambiarValor;

            this.rotacionOriginal = new Quaternion[this.Agujas.Length];
            this.GradosPorUnidad = new float[this.Agujas.Length];
            for (int indice = 0; indice < this.Agujas.Length; indice++)
            {// Se calcula cuantos grados debe girar cada aguja por cada unidad (1)
                this.GradosPorUnidad[indice] = 360 / this.ValorPorVuelta[indice];
                this.rotacionOriginal[indice] = this.Agujas[indice].localRotation;
            }

        }

        private void GirarAgujas()
        {
            for (int Indice = 0; Indice < this.Agujas.Length; Indice++)
            {
                this.Agujas[Indice].localRotation = this.rotacionOriginal[Indice] *
                    Quaternion.AngleAxis(this.ValorDeAguja * this.GradosPorUnidad[Indice], this.DireccionDeRotacion);
            }
        }

        private void Intrumento_AlCambiarValor(object sender, EventArgs e)
        {
            this.GirarAgujas();
        }
    }
}