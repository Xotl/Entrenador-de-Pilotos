using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Interfaz.Utilities
{
    public abstract class ComboBoxBase : MonoBehaviour
    {
        #region Campos públicos

        public ComboBoxItem ItemPrefab;

        #endregion


        #region Campos privados

        /// <summary>
        /// Botón que despliga todas las opciones de selección.
        /// </summary>
        private BotonController _botonOptions;

        /// <summary>
        /// Scroll donde se mostrarán las opciones.
        /// </summary>
        private ScrollControl _scrollControl;

        /// <summary>
        /// Lista con todos los elementos agregados.
        /// </summary>
        private List<ComboBoxItem> _listaDeElementos;

        /// <summary>
        /// Valor que indica si cursor del mouse se encuentra sobre el control.
        /// </summary>
        protected bool _mouseSobreElControl = false;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene la cantidad de Items que contiene este ComboBox.
        /// </summary>
        public int ItemsCount
        {
            get
            {
                return this._listaDeElementos.Count;
            }
        }

        private int _SelectedIndex = -1;
        /// <summary>
        /// Obtiene o establece el índice que especifica el elemento seleccionado actualmente.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this._SelectedIndex;
            }
            set
            {
                if (value < 0)
                    value = -1;

                if (this._SelectedIndex != value)
                {
                    this._SelectedIndex = value;
                    if (this._SelectedIndex == -1)
                    {
                        this._SelectedItem = null;
                        this.CambiarTextoDelItemSeleccionado(string.Empty);
                    }
                    else
                    {
                        this._SelectedItem = this._listaDeElementos[this._SelectedIndex];
                        this.CambiarTextoDelItemSeleccionado(this._SelectedItem.Texto);
                    }
                    this.eventoSelectedIndexChange(System.EventArgs.Empty);
                }
            }
        }

        private ComboBoxItem _SelectedItem = null;
        /// <summary>
        /// Obtiene o establece el elemento seleccionado actualmente en el ComboBox.
        /// </summary>
        public ComboBoxItem SelectedItem
        {
            get
            {
                return this._SelectedItem;
            }
            set
            {
                if (this._SelectedItem != value)
                {
                    if (value == null)
                    {
                        this.SelectedIndex = -1;
                        return;
                    }

                    int indice = this._listaDeElementos.IndexOf(value);
                    if (indice != -1)
                    {
                        this.SelectedIndex = indice;
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene un arreglo que representa la colección de los elementos que contiene el ComboBox.
        /// </summary>
        public ComboBoxItem[] Items
        {
            get
            {
                return this._listaDeElementos.ToArray();
            }
        }

        private bool _SeMuestraElListadoDeItems = true;
        /// <summary>
        /// Obtiene o establece un valor que indica si el listado de items se muestra actualmente o no. TRUE para mostrar la lista de items, de lo contrario FALSE.
        /// </summary>
        public bool SeMuestraElListadoDeItems
        {
            get
            {
                return this._SeMuestraElListadoDeItems;
            }
            set
            {
                if (this._SeMuestraElListadoDeItems != value)
                {
                    this._SeMuestraElListadoDeItems = value;
                    this._scrollControl.SetActive(this._SeMuestraElListadoDeItems);
                    
                    if (this._SeMuestraElListadoDeItems)
                    {
                        this._scrollControl.ScrollH = 0;
                        this._scrollControl.ScrollV = 0;
                    }

                    this.eventoAlCambiarVisualizacionDeListaDeItems(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Eventos de Unity

        protected virtual void Awake()
        {
            // Obtención de objetos
            this._botonOptions = this.transform.FindChild("Boton").GetComponent<BotonController>();
            this._scrollControl = this.transform.FindChild("ScrollControl").GetComponent<ScrollControl>();

            // Inicialización
            this._scrollControl.gameObject.SetActive(true);
            this._listaDeElementos = new List<ComboBoxItem>();

            // Eventos
            this.AlQuitarElemento += new System.EventHandler(this.ComboBox_AlQuitarElemento);
            this._botonOptions.Click += new System.EventHandler(this._botonOptions_Click);
            this._botonOptions.MouseExit += new System.EventHandler(_botonOptions_MouseExit);
            this._botonOptions.MouseEnter += new System.EventHandler(_botonOptions_MouseEnter);
            this.SelectedIndexChange += new System.EventHandler(this.ComboBox_SelectedIndexChange);
        }

        #region Eventos dependientes del Awake

        private void _botonOptions_MouseEnter(object sender, System.EventArgs e)
        {
            this._mouseSobreElControl = true;
        }

        private void _botonOptions_MouseExit(object sender, System.EventArgs e)
        {
            this._mouseSobreElControl = false;
        }

        private void ComboBox_SelectedIndexChange(object sender, System.EventArgs e)
        {
            this.SeMuestraElListadoDeItems = false;
        }

        private void ComboBox_AlQuitarElemento(object sender, System.EventArgs e)
        {
            this.ReordenarListaDeItemsEnScroll();
        }

        #endregion

        protected virtual void Start()
        {
            this.SeMuestraElListadoDeItems = false;
        }

        protected virtual void Update()
        {
            if (this.SeMuestraElListadoDeItems && Input.GetMouseButtonUp(0) && !this._mouseSobreElControl)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (!this._scrollControl.ControlAreaBounds.IntersectRay(ray))
                {// Oculta el mouse cuando se da click fuera del control
                    this.SeMuestraElListadoDeItems = false;
                }
            }
            
        }

        #endregion


        #region Definición de eventos del control

        /// <summary>
        /// Se produce cuando ha cambiado la propiedad SelectedIndex.
        /// </summary>
        public event System.EventHandler SelectedIndexChange;

        /// <summary>
        /// Se produce cuando se ha agregado un nuevo elemento a la colección de los elementos que contiene el ComboBox.
        /// </summary>
        public event System.EventHandler<ComboBoxItemEventArgs> AlAgregarElemento;

        /// <summary>
        /// Se produce cuando se ha quitado un elemento de la colección de los elementos que contiene el ComboBox.
        /// </summary>
        public event System.EventHandler AlQuitarElemento;

        /// <summary>
        /// Se produce cuando ha cambiado la propiedad SeMuestraElListadoDeItems.
        /// </summary>
        public event System.EventHandler AlCambiarVisualizacionDeListaDeItems;

        
        private void eventoSelectedIndexChange(System.EventArgs e)
        {
            if (this.SelectedIndexChange != null)
                this.SelectedIndexChange(this, e);
        }

        private void eventoAlAgregarElemento(ComboBoxItemEventArgs e)
        {
            if (this.AlAgregarElemento != null)
                this.AlAgregarElemento(this, e);
        }

        private void eventoAlQuitarElemento(System.EventArgs e)
        {
            if (this.AlQuitarElemento != null)
                this.AlQuitarElemento(this, e);
        }

        private void eventoAlCambiarVisualizacionDeListaDeItems(System.EventArgs e)
        {
            if (this.AlCambiarVisualizacionDeListaDeItems != null)
                this.AlCambiarVisualizacionDeListaDeItems(this, e);
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Obtiene el índice del primer ComboBoxItem que tenga un valor dado en su propiedad Valor.
        /// </summary>
        /// <param name="valor">Valor a buscar.</param>
        /// <returns>ïndice del objeto encontrado. Regresará -1 si no fue encontrado.</returns>
        public int IndexOf(object valor)
        {
            int indice = 0;
            while (indice < this.ItemsCount)
            {
                if (valor.Equals(this._listaDeElementos[indice].Valor))
                    break;
                indice++;
            }

            if (indice == this.ItemsCount)
                return -1;

            return indice;
        }

        /// <summary>
        /// Cambia el texto en el objeto que representa el item seleccionado.
        /// </summary>
        /// <param name="texto">Nuevo texto que se quiere asignar.</param>
        protected abstract void CambiarTextoDelItemSeleccionado(string texto);

        /// <summary>
        /// Obtiene el texto del objeto que representa el item seleccionado.
        /// </summary>
        protected abstract string ObtenerTextoDelItemSeleccionado();

        /// <summary>
        /// Actualiza y reordena la lista de items del ComboBox.
        /// </summary>
        private void ReordenarListaDeItemsEnScroll()
        {
            Vector2 pos;
            ComboBoxItem[] LosItems = this.Items;
            for (int i = 0; i < LosItems.Length; i++)
            {
                pos = new Vector2(
                    0,
                    -1 * LosItems[i].ItemBounds.size.y * i
                );
                LosItems[i].transform.localPosition = pos;
            }
            this._scrollControl.RecalcularBounds();
        }

        /// <summary>
        /// Crea una instancia de un item dado y lo agrega al final del scroll de items.
        /// </summary>
        /// <param name="item">Nuevo item que se desea instanciar y agregar.</param>
        /// <returns>Regresa el ComboBoxItem que recién se agregó.</returns>
        private ComboBoxItem InstanciarYColocarNuevoItemEnScroll(object item)
        {
            ComboBoxItem obj = Instantiate(this.ItemPrefab) as ComboBoxItem;
            this._listaDeElementos.Add(obj);
            obj.ActivarAjusteDeSeleccionAlScroll(this._scrollControl);
            obj.Texto = item.ToString();
            obj.Valor = item;
            obj.Click += new System.EventHandler(this.Item_Click);

            Vector2 pos = new Vector2(
                    0,
                    -1 * obj.ItemBounds.size.y * (this.Items.Length - 1)
                );

            bool tmp = this._scrollControl.gameObject.activeInHierarchy;
            this._scrollControl.SetActive(true);
            this._scrollControl.AgregarElemento(obj.gameObject, pos);
            this._scrollControl.SetActive(tmp);

            return obj;
        }

        /// <summary>
        /// Agrega un elemento a la lista de elementos del ComboBox.
        /// </summary>
        /// <param name="item"></param>
        public void AgregarElemento(object item)
        {
            ComboBoxItem oItem = InstanciarYColocarNuevoItemEnScroll(item);
            ComboBoxItemEventArgs args = new ComboBoxItemEventArgs(this._listaDeElementos.Count - 1, oItem);
            this.eventoAlAgregarElemento(args);
        }

        /// <summary>
        /// Quita un elemento del ComboBox en el índice especificado.
        /// </summary>
        /// <param name="indice"></param>
        public void QuitarElemento(int indice)
        {
            Destroy(_listaDeElementos[indice].gameObject);
            this._listaDeElementos.RemoveAt(indice);
            this.eventoAlQuitarElemento(System.EventArgs.Empty);
        }

        /// <summary>
        /// Quita el elemento especificado del ComboBox.
        /// </summary>
        /// <param name="item">Elemento a quitar.</param>
        /// <returns>TRUE si se encontró y quitó el elemento, de lo contrario FALSE.</returns>
        public bool QuitarElemento(ComboBoxItem item)
        {
            if (this._listaDeElementos.Remove(item))
            {
                Destroy(item.gameObject);
                this.eventoAlQuitarElemento(System.EventArgs.Empty);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Quita todos los elementos del ComboBox.
        /// </summary>
        public void Clear()
        {
            this._listaDeElementos.Clear();
            this._scrollControl.Limpiar();
            this.eventoAlQuitarElemento(System.EventArgs.Empty);
        }

        #endregion


        #region Eventos de la interfaz

        private void Item_Click(object sender, System.EventArgs e)
        {
            this.SeMuestraElListadoDeItems = false;
            this.SelectedItem = (ComboBoxItem)sender;
        }

        private void _botonOptions_Click(object sender, System.EventArgs e)
        {
            this.SeMuestraElListadoDeItems = !this.SeMuestraElListadoDeItems;
        }

        #endregion

        public class ComboBoxItemEventArgs : System.EventArgs
        {
            private int indice = -1;
            private ComboBoxItem item = null;

            public int Indice
            {
                get { return this.indice; }
            }

            public string ItemText
            {
                get { return this.item.Texto; }
            }

            public ComboBoxItem Item
            {
                get { return this.item; }
            }

            public ComboBoxItemEventArgs(int indice, ComboBoxItem item)
            {
                this.indice = indice;
                this.item = item;
            }
        }
    }
}