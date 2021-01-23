using UnityEngine;
using System.Collections;

namespace Interfaz.Genericos
{
    [RequireComponent(typeof(Collider))]
    public class MenuRadialItem : MonoBehaviour
    {
        #region Campos privados

        private TextMesh textMesh;

        #endregion


        #region Propiedades

        private bool mouseOver = false;
        /// <summary>
        /// Obtiene un valor indicando si el cursor del mouse se encuentra sobre este item.
        /// </summary>
        public bool MouseOver
        {
            get
            {
                return this.mouseOver;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto del item.
        /// </summary>
        public string Texto
        {
            get
            {
                return this.textMesh.text;
            }
            set
            {
                if (this.textMesh.text != value)
                {
                    this.textMesh.text = value;
                    this.eventoAlCambiarTexto(System.EventArgs.Empty);
                }
            }
        }

        private object valor;
        /// <summary>
        /// Obtiene o establece el valor asociado a este item.
        /// </summary>
        public object Valor
        {
            get
            {
                return this.valor;
            }
            set
            {
                if (this.valor != value)
                {
                    this.valor = value;
                    this.eventoAlCambiarValor(System.EventArgs.Empty);
                    this.Texto = this.valor.ToString();
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando la propiedad Valor cambia.
        /// </summary>
        public event System.EventHandler AlCambiarValor;

        /// <summary>
        /// Se produce cuando la propiedad Texto cambia.
        /// </summary>
        public event System.EventHandler AlCambiarTexto;

        /// <summary>
        /// Se produce cuando la propiedad MouseOver cambia.
        /// </summary>
        public event System.EventHandler AlCambiarMouseOver;

        private void eventoAlCambiarValor(System.EventArgs e)
        {
            if (this.AlCambiarValor != null)
                this.AlCambiarValor(this, e);
        }

        private void eventoAlCambiarTexto(System.EventArgs e)
        {
            if (this.AlCambiarTexto != null)
                this.AlCambiarTexto(this, e);
        }

        private void eventoAlCambiarMouseOver(System.EventArgs e)
        {
            if (this.AlCambiarMouseOver != null)
                this.AlCambiarMouseOver(this, e);
        }

        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            // Obtención de objetos
            this.textMesh = this.transform.Find("Texto").GetComponent<TextMesh>();
        }

        private void OnMouseEnter()
        {
            this.mouseOver = true;
            this.eventoAlCambiarMouseOver(System.EventArgs.Empty);
        }

        private void OnMouseExit()
        {
            this.mouseOver = false;
            this.eventoAlCambiarMouseOver(System.EventArgs.Empty);
        }

        private void OnEnable()
        {
            this.mouseOver = false;
        }

        #endregion
    }
}