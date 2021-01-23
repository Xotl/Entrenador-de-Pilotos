using UnityEngine;
using System;
using System.Collections;

namespace Interfaz.Utilities
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CajaDeTexto : MonoBehaviour
    {
        /// <summary>
        /// Camara donde se graficará la caja de texto. Si no hay ninguna se usará la principal.
        /// </summary>
        public Camera CamaraDeVisualizacion;

        public string TextoInicial = "Escribe aquí";
        public GUIStyle Estilo;
        /// <summary>
        /// Tamaño máximo de caracteres.
        /// </summary>
        public int MaxLength = 25;
        private Camera _Camara;

        /// <summary>
        /// Última posición global obtenida del mesh.
        /// </summary>
        private Vector3 _UltimaPosObtenida = Vector3.up;
        private Vector3 _TopLeft = Vector3.zero;
        
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


        public GUISkin Skin;
        private string _Texto = string.Empty;

        /// <summary>
        /// Se desencadena cuando la propiedad Texto cambia.
        /// </summary>
        public event EventHandler AlCambiarTexto;

        /// <summary>
        /// Obtiene o establece el texto del control.
        /// </summary>
        public string Texto
        {
            get
            {
                return this._Texto;
            }
            set
            {
                if (this._Texto != value)
                {
                    this._Texto = value;
                    this.eventoAlCambiarTexto(new EventArgs());
                }
            }
        }

        private void Awake()
        {
            this._Texto = this.TextoInicial;

            // Define la cámara a utilizar
            this._Camara = this.CamaraDeVisualizacion;
            if (this._Camara == null)
                this._Camara = Camera.main;
        }

        private void OnGUI()
        {
            GUI.skin = this.Skin;
            this.DibujarCajaEditable();
        }

        private void DibujarCajaEditable()
        {
            Vector3 posicion = this.TopLeft;
            Vector3 tam = this._Camara.WorldToScreenPoint(new Vector3(this.renderer.bounds.size.x + posicion.x, this.renderer.bounds.size.y + posicion.y));
            posicion = this._Camara.WorldToScreenPoint(this.TopLeft);

            float anchoPx = tam.x - posicion.x;
            float altoPx = tam.y - posicion.y;

            this.Texto = GUI.TextArea(new Rect(posicion.x, Screen.height - posicion.y, anchoPx, altoPx), this.Texto, this.MaxLength, this.Estilo);
        }

        private void OnMouseEnter()
        {
            throw new System.NotImplementedException();
        }

        private void eventoAlCambiarTexto(EventArgs e)
        {
            if (this.AlCambiarTexto != null)
                this.AlCambiarTexto(this, e);
        }
    }
}