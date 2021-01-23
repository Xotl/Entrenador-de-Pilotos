using UnityEngine;
using System.Collections;

namespace Interfaz.Utilities
{
    [RequireComponent(typeof(Collider))]
    public class CheckBox : MonoBehaviour
    {

        #region Campos públicos

        public Material MaterialDeCasillaMarcada;

        #endregion


        #region Campos privados

        private Material MaterialDeCasillaDesmarcada;
        private TextMesh _textMesh;
        private Transform _casilla;
        private BoxCollider _collider;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtien o establece el texto de la etiqueta del CheckBox.
        /// </summary>
        public string Texto
        {
            get
            {
                return this._textMesh.text;
            }
            set
            {
                if(this._textMesh.text != value)
                {
                    this._textMesh.text = value;
                    this.ActualizarColider();
                    this.eventoAlCambiarTexto(System.EventArgs.Empty);
                }
            }
        }

        private bool _checked = false;
        /// <summary>
        /// Obtiene o establece un valor que indica si la casilla se encuentra marcada.
        /// </summary>
        public bool Checked
        {
            get
            {
                return this._checked;
            }
            set
            {
                if (this._checked != value)
                {
                    this._checked = value;

                    if (this._checked)
                        this._casilla.renderer.material = this.MaterialDeCasillaMarcada;
                    else
                        this._casilla.renderer.material = this.MaterialDeCasillaDesmarcada;

                    this.eventoOnCheckedChange(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando el valor de la propiedad Checked cambia.
        /// </summary>
        public event System.EventHandler OnCheckedChange;
        
        /// <summary>
        /// Se produce cuando el valor de la propiedad Texto cambia.
        /// </summary>
        public event System.EventHandler AlCambiarTexto;

        private void eventoOnCheckedChange(System.EventArgs e)
        {
            if (this.OnCheckedChange != null)
                this.OnCheckedChange(this, e);
        }

        private void eventoAlCambiarTexto(System.EventArgs e)
        {
            if (this.AlCambiarTexto != null)
                this.AlCambiarTexto(this, e);
        }

        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            this._textMesh = this.GetComponent<TextMesh>();
            this._casilla = this.transform.Find("Casilla");
            this.MaterialDeCasillaDesmarcada = this._casilla.renderer.material;
            this._collider = this.GetComponent<BoxCollider>();

            this.ActualizarColider();
        }

        private void OnMouseUpAsButton()
        {
            this.Checked = !this.Checked;
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Actualiza la posición y las dimensiones del Collider.
        /// </summary>
        /// <returns></returns>
        private void ActualizarColider()
        {
            Bounds b = new Bounds(this._casilla.renderer.bounds.center, this._casilla.renderer.bounds.size);
            b.Encapsulate(this.renderer.bounds);
            b.Expand(0.3f);
            this._collider.center = this.transform.InverseTransformPoint(b.center);
            this._collider.size = b.size;
        }

        #endregion
    }
}