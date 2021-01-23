using UnityEngine;
using System.Collections;

namespace Interfaz.Utilities
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ItemController : MonoBehaviour
    {
        public const float ESPACIADO_TOP_BOTTOM = 0.1f;

        #region Campos públicos

        public Material BackgroundMaterial;

        #endregion


        #region Campos privados

        private TextMesh _textMesh;
        private BoxCollider colliderDelItem;
        private Transform ItemBackground;
        private ScrollControl scrollControl = null;

        #endregion


        #region Propiedades

        private object valor = null;
        /// <summary>
        /// Obtiene o establece el valor del item.
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
                }
            }
        }

        /// <summary>
        /// Obtiene o establece un valor que indica si el color de fondo del Item se muestra.
        /// </summary>
        public bool MostrarFondo
        {
            get
            {
                return this.ItemBackground.renderer.enabled;
            }

            set
            {
                this.ItemBackground.renderer.enabled = value;
            }
        }

        private float _EspaciadoEntreItems = ESPACIADO_TOP_BOTTOM;
        /// <summary>
        /// Espacio en unidades Unity que se quiere entre cada Item. Este espacio será aplicado tanto arriba como abajo del Item.
        /// </summary>
        private float EspaciadoEntreItems
        {
            get
            {
                return this._EspaciadoEntreItems;
            }
            set
            {
                if (value < 0)
                    value = 0;

                if (this._EspaciadoEntreItems != value)
                {
                    this._EspaciadoEntreItems = value * 2;
                    this.ReajustarDimensionesDelItem();
                }
            }
        }

        private Bounds itemBounds;
        /// <summary>
        /// Obtiene las dimensiones de este Item. Nota: Las dimensiones del Item no abarcan las dimensiones del Texto.
        /// </summary>
        public Bounds ItemBounds
        {
            get
            {
                return this.itemBounds;
            }
        }

        /// <summary>
        /// Obtiene o establece el tamaño de la letra del texto. Debe ser mayor que 0. Nota: Esto no afecta a las dimensiones del Item.
        /// </summary>
        public int FontSize
        {
            get
            {
                return this._textMesh.fontSize;
            }
            set
            {
                if (value > 0)
                    this._textMesh.fontSize = value;
            }
        }

        /// <summary>
        /// Obtiene o establece el texto asociado al item. 
        /// </summary>
        public string Texto
        {
            get
            {
                return this._textMesh.text;
            }
            set
            {
                if (this._textMesh.text != value)
                {
                    this._textMesh.text = value;
                    this.ReajustarDimensionesDelItem();
                    this.eventoAlCambiarTexto(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando ha cambiado la propiedad Texto.
        /// </summary>
        public event System.EventHandler AlCambiarTexto;

        /// <summary>
        /// Se produce cuando la propiedad Valor cambia.
        /// </summary>
        public event System.EventHandler AlCambiarValor;

        /// <summary>
        /// Se produce cuando se hace click sobre el item.
        /// </summary>
        public event System.EventHandler Click;

        /// <summary>
        /// Se produce cuando se el cursor del mouse entra al control.
        /// </summary>
        public event System.EventHandler MouseEnter;

        /// <summary>
        /// Se produce cuando se el cursor del mouse sale del control.
        /// </summary>
        public event System.EventHandler MouseExit;


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

        private void eventoClick(System.EventArgs e)
        {
            if (this.Click != null)
                this.Click(this, e);
        }

        private void eventoMouseEnter(System.EventArgs e)
        {
            if (this.MouseEnter != null)
                this.MouseEnter(this, e);
        }

        private void eventoMouseExit(System.EventArgs e)
        {
            if (this.MouseExit != null)
                this.MouseExit(this, e);
        }

        #endregion


        #region Eventos de Unity

        protected virtual void Awake()
        {
            // Obtención de objetos
            this._textMesh = this.GetComponent<TextMesh>();
            this.colliderDelItem = this.collider as BoxCollider;
            this.ItemBackground = this.transform.Find("ItemBackground");


            //Inicialización
            this.ItemBackground.renderer.material = this.BackgroundMaterial;
            this.ItemBackground.renderer.enabled = false;
        }

        private void OnMouseExit()
        {
            this.eventoMouseExit(System.EventArgs.Empty);
        }

        private void OnMouseEnter()
        {
            this.eventoMouseEnter(System.EventArgs.Empty);
        }

        private void OnMouseUpAsButton()
        {
            this.eventoClick(System.EventArgs.Empty);
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Activa o desactiva el modo en el que el área del mouseOver del Item se ajusta al ancho del scroll. Si se asigna NULL se usará el ancho del texto.
        /// </summary>
        /// <param name="scrollControl">Scroll del ComboBox.</param>
        public void ActivarAjusteDeSeleccionAlScroll(ScrollControl scrollControl)
        {
            this.scrollControl = scrollControl;
            this.ReajustarDimensionesDelItem();
        }

        /// <summary>
        /// Reajusta las dimensiones de las dependencias del Item.
        /// </summary>
        private void ReajustarDimensionesDelItem()
        {
            this.ActualizarDimensionesDelItem();
            this.ActualizarDimensionesDelCollider(this.ItemBounds);
            this.ActualizarDimensionesDelBackground(this.ItemBounds);
        }

        /// <summary>
        /// Actualiza las dimensiones del espacio que ocupa este item basado en el contenido de Texto o de su Scroll asociado.
        /// </summary>
        private void ActualizarDimensionesDelItem()
        {
            float anchoItem = (this.scrollControl == null) ?
                this.renderer.bounds.size.x : this.scrollControl.renderer.bounds.size.x - this.scrollControl.Padding * 2;

            Vector3 size = new Vector3(
                    anchoItem,
                    this.renderer.bounds.size.y + ESPACIADO_TOP_BOTTOM,
                    0
                );

            Vector3 centro = new Vector3(
                    this.renderer.bounds.center.x + (anchoItem - this.renderer.bounds.size.x) / 2,
                    this.renderer.bounds.center.y,
                    this.renderer.bounds.center.z
                );

            this.itemBounds = new Bounds(centro, size);
        }

        /// <summary>
        /// Actualiza las dimensiones del ItemBackground.
        /// </summary>
        private void ActualizarDimensionesDelBackground(Bounds bounds)
        {
            this.ItemBackground.position = bounds.center;
            this.ItemBackground.localScale = bounds.size;
        }

        /// <summary>
        /// Actualiza las dimensiones del Collider.
        /// </summary>
        private void ActualizarDimensionesDelCollider(Bounds bounds)
        {
            this.colliderDelItem.center = this.transform.InverseTransformPoint(bounds.center);
            this.colliderDelItem.size = bounds.size;
        }

        #endregion
    }
}