using UnityEngine;
using System.Collections.Generic;

namespace Interfaz.Utilities
{
    public class StaticComboBox : ComboBoxBase
    {
        #region Campos públicos

        public GUIStyle Estilo;

        #endregion


        #region Campos privados

        /// <summary>
        /// Última posición global obtenida del mesh.
        /// </summary>
        private Vector3 _UltimaPosObtenida = Vector3.up;
        private Vector3 _TopLeft = Vector3.zero;

        #endregion


        #region Propiedades

        private string texto;
        /// <summary>
        /// Obtiene el texto asociado al control.
        /// </summary>
        public string Texto
        {
            get
            {
                return this.texto;
            }
        }

        /// <summary>
        /// Posicion global del Top/Left del área.
        /// </summary>
        private Vector3 TopLeft
        {
            get
            {
                if (this._UltimaPosObtenida != this.transform.position)
                {
                    this._UltimaPosObtenida = this.transform.position;
                    this._TopLeft =
                        this.renderer.bounds.center - new Vector3(this.renderer.bounds.size.x / 2, this.renderer.bounds.size.y / -2, 0);
                }

                return _TopLeft;
            }
        }

        #endregion


        #region Eventos de Unity

        protected override void Awake()
        {
            base.Awake();
        }

        #region Eventos dependientes del Awake
        #endregion

        private void OnGUI()
        {
            this.DibujarCajaEditable();
        }

        private void OnMouseDown()
        {
            this.SeMuestraElListadoDeItems = !this.SeMuestraElListadoDeItems;
        }

        private void OnMouseEnter()
        {
            this._mouseSobreElControl = true;
        }

        private void OnMouseExit()
        {
            this._mouseSobreElControl = false;
        }

        #endregion


        #region Métodos de la clase

        private void DibujarCajaEditable()
        {
            Vector3 posicion = this.TopLeft;
            Vector3 tam = Camera.main.WorldToScreenPoint(new Vector3(this.renderer.bounds.size.x + posicion.x, this.renderer.bounds.size.y + posicion.y));
            posicion = Camera.main.WorldToScreenPoint(this.TopLeft);

            float anchoPx = tam.x - posicion.x;
            float altoPx = tam.y - posicion.y;

            GUI.Label(new Rect(posicion.x, Screen.height - posicion.y, anchoPx, altoPx), this.Texto, this.Estilo);
        }

        /// <summary>
        /// Cambia el texto en el objeto que representa el item seleccionado.
        /// </summary>
        /// <param name="texto">Nuevo texto que se quiere asignar.</param>
        protected override void CambiarTextoDelItemSeleccionado(string texto)
        {
            this.texto = texto;
        }

        /// <summary>
        /// Obtiene el texto del objeto que representa el item seleccionado.
        /// </summary>
        protected override string ObtenerTextoDelItemSeleccionado()
        {
            return this.Texto;
        }

        #endregion
    }
}