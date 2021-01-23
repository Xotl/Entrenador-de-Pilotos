using UnityEngine;
using System.Collections.Generic;
using Interfaz.Utilities;

namespace Entrenamiento.GUI.EditorDeEscenarios
{
    [RequireComponent(typeof(ScrollControl))]
    class SeleccionEnVisorDeEtapas : MonoBehaviour
    {
        private Interfaz.Utilities.ScrollControl scrollControl;
        private Dictionary<GameObject, Material> diccionarioBotonesMateriales;
        private GameObject objetoSeleccionado = null;

        [SerializeField]
        private Material Material;

        private void Awake()
        {
            this.diccionarioBotonesMateriales = new Dictionary<GameObject, Material>();
            this.scrollControl = this.GetComponent<Interfaz.Utilities.ScrollControl>();

            this.scrollControl.AlAgregarElemento += this.scrollControl_AlAgregarElemento;
            this.scrollControl.AlQuitarElemento += this.scrollControl_AlQuitarElemento;
        }

        private void scrollControl_AlQuitarElemento(object sender, System.EventArgs e)
        {
            GameObject[] objetos = this.scrollControl.ElementosDelScroll;

            this.ResetMateriales();
            this.diccionarioBotonesMateriales.Clear();
            foreach (GameObject objeto in objetos)
            {
                this.diccionarioBotonesMateriales.Add(objeto, objeto.renderer.sharedMaterial);
            }

            if (this.objetoSeleccionado != null)
                this.objetoSeleccionado.renderer.sharedMaterial = this.Material;
            else
                this.objetoSeleccionado = null;// Le asigno NULL para evitar el uso del operador sobrecargado == en los GameObject
        }

        private void scrollControl_AlAgregarElemento(object sender, System.EventArgs e)
        {
            GameObject[] objetos = this.scrollControl.ElementosDelScroll;
            this.AgregarEventosABotones(objetos[objetos.Length - 1].GetComponentsInChildren<BotonController>());
        }

        private void AgregarEventosABotones(BotonController[] botones)
        {
            if (botones == null)
                return;

            foreach (BotonController boton in botones)
            {
                if (boton.name == "SolucionBtn(Clone)" || boton.name == "Sintoma")
                {
                    this.diccionarioBotonesMateriales.Add(boton.gameObject, boton.renderer.sharedMaterial);
                    boton.Click += this.boton_Click;
                }
            }
        }

        private void boton_Click(object sender, System.EventArgs e)
        {
            this.objetoSeleccionado = ((BotonController)sender).gameObject;
            
            this.ResetMateriales();
            this.objetoSeleccionado.renderer.sharedMaterial = this.Material;
        }

        /// <summary>
        /// Regresa todos los objetos del diccionario a sus materiales originales.
        /// </summary>
        private void ResetMateriales()
        {
            foreach (KeyValuePair<GameObject, Material> par in this.diccionarioBotonesMateriales)
            {
                par.Key.renderer.sharedMaterial = par.Value;
            }
        }
    }
}
