using UnityEngine;
using System.Collections.Generic;

namespace Interfaz.Utilities
{
    public class ListControl : MonoBehaviour
    {
        private const float ESPACIADO_DE_iTEMS = -0.5f;

        #region Campos públicos

        /// <summary>
        /// Prefab que se instanciará al agregar items.
        /// </summary>
        public ListControlItem ItemPrefab;

        #endregion


        #region Campos privados

        /// <summary>
        /// Scroll donde se muestran los items de la lista.
        /// </summary>
        private ScrollControl _scroll;

        /// <summary>
        /// Objeto que representa el título de la lista.
        /// </summary>
        private TextMesh TituloMesh;

        //private DiccionarioBidireccional<ListControlItem, object> _relItemsObjetos;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene la cantidad actual de items en el control.
        /// </summary>
        public int ItemsCount
        {
            get
            {
                return this._items.Count;
            }
        }

        private List<ListControlItem> _SelectedItems;
        /// <summary>
        /// Obtiene los items seleccionados actualmente. Nota: Estos no necesariamente están en el orden en el que aparecen en la Lista.
        /// </summary>
        public ListControlItem[] SelectedItems
        {
            get
            {
                return this._SelectedItems.ToArray();
            }
        }

        /// <summary>
        /// Obtiene el número de items seleccionados actualmente en la lista.
        /// </summary>
        public int SelectedItemsCount
        {
            get
            {
                return this._SelectedItems.Count;
            }
        }

        private List<ListControlItem> _items;
        /// <summary>
        /// Obtiene los Items que contiene la lista actualmente.
        /// </summary>
        public ListControlItem[] Items
        {
            get
            {
                return this._items.ToArray();
            }
        }

        /// <summary>
        /// Obtiene o establece el título de la lista.
        /// </summary>
        public string Titulo
        {
            get
            {
                return this.TituloMesh.text;
            }
            set
            {
                if (this.TituloMesh.text != value)
                {
                    this.TituloMesh.text = value;
                    this.eventoAlCambiarTitulo(System.EventArgs.Empty);
                }
            }

        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando un item acaba de ser seleccionado.
        /// </summary>
        public event System.EventHandler<ListItemEventArgs> ItemSeleccionado;

        /// <summary>
        /// Se produce cuando un item acaba de ser deseleccionado.
        /// </summary>
        public event System.EventHandler<ListItemEventArgs> ItemDeseleccionado;

        /// <summary>
        /// Se produce cuando la propiedad SelectedItems cambia.
        /// </summary>
        public event System.EventHandler SelectedItemsChanged;

        /// <summary>
        /// Se produce cuando la propiedad Titulo cambia de valor.
        /// </summary>
        public event System.EventHandler AlCambiarTitulo;

        /// <summary>
        /// Se produce cuando uno o más items se agregan a la lista.
        /// </summary>
        public event System.EventHandler<ListItemEventArgs> AlAgregarItem;

        /// <summary>
        /// Se produce cuando uno o más items se quitan de la lista.
        /// </summary>
        public event System.EventHandler<ListItemQuitadoEventArgs> AlQuitarItem;


        private void eventoItemSeleccionado(ListItemEventArgs e)
        {
            if (this.ItemSeleccionado != null)
                this.ItemSeleccionado(this, e);
        }

        private void eventoItemDeseleccionado(ListItemEventArgs e)
        {
            if (this.ItemDeseleccionado != null)
                this.ItemDeseleccionado(this, e);
        }

        private void eventoSelectedItemsChanged(System.EventArgs e)
        {
            if (this.SelectedItemsChanged != null)
                this.SelectedItemsChanged(this, e);
        }

        private void eventoAlCambiarTitulo(System.EventArgs e)
        {
            if (this.AlCambiarTitulo != null)
                this.AlCambiarTitulo(this, e);
        }

        private void eventoAlAgregarItem(ListItemEventArgs e)
        {
            if (this.AlAgregarItem != null)
                this.AlAgregarItem(this, e);
        }

        private void eventoAlQuitarItem(ListItemQuitadoEventArgs e)
        {
            if (this.AlQuitarItem != null)
                this.AlQuitarItem(this, e);
        }

        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            // Obtención de objetos
            this.TituloMesh = this.transform.Find("Titulo").GetComponent<TextMesh>();
            this._scroll = this.transform.Find("ScrollControl").GetComponent<ScrollControl>();

            // Inicialización
            this._items = new List<ListControlItem>();
            //this._relItemsObjetos = new DiccionarioBidireccional<ListControlItem, object>();
            this._SelectedItems = new List<ListControlItem>();
        }

        private void Start()
        {

        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Similar al SetActive de GameObject. Activa o desactiva este objeto, eso incluye a sus hijos y objetos dependientes.
        /// </summary>
        /// <param name="value">TRUE para activar, de lo contrario FALSE.</param>
        public void SetActive(bool value)
        {
            this._scroll.SetActive(value);
            this.gameObject.SetActive(value);
        }

        /// <summary>
        /// Quita todos los items del control.
        /// </summary>
        public void Clear()
        {
            ListControlItem[] items = this.Items;
            object[] valores = new object[items.Length];
            for (int i = 0; i < valores.Length;i++ )
                valores[i] = items[i].Valor;

            this._SelectedItems.Clear();
            this.eventoSelectedItemsChanged(System.EventArgs.Empty);

            this._items.Clear();
            this._scroll.Limpiar();

            foreach (object valor in valores)
                this.eventoAlQuitarItem(new ListItemQuitadoEventArgs(valor));
        }

        /// <summary>
        /// Posiciona un nuevo Item al final de la lista.
        /// </summary>
        /// <param name="item"></param>
        private void AgregarObjetoItemAlFinal(ListControlItem item)
        {
            item.ActivarAjusteDeSeleccionAlScroll(this._scroll);
            item.AlCambiarSeleccion += this.ListItem_AlCambiarSeleccion;
            this._scroll.AgregarElemento(item.gameObject, new Vector2(0, ESPACIADO_DE_iTEMS * (this.ItemsCount - 1)));
            this._scroll.RecalcularBounds();
        }
        

        /// <summary>
        /// Reposiciona todos los items de la lista según su índice.
        /// </summary>
        private void ActualizarPosicionesDeItems()
        {
            ListControlItem[] items = this.Items;
            Vector3 aux;

            for (int i = 0; i < items.Length; i++)
            {
                aux = items[i].transform.localPosition;
                items[i].transform.localPosition = new Vector3(
                    aux.x,
                    ESPACIADO_DE_iTEMS * i,
                    aux.z
                );
            }

            this._scroll.RecalcularBounds();
        }

        /// <summary>
        /// Agrega un nuevo item a la lista.
        /// </summary>
        /// <param name="item">Item a agregar.</param>
        public void AgregarItem(object item)
        {
            ListControlItem objeto = Instantiate(this.ItemPrefab) as ListControlItem;
            objeto.Valor = item;
            this._items.Add(objeto);
            this.AgregarObjetoItemAlFinal(objeto);
            this.eventoAlAgregarItem(new ListItemEventArgs(objeto));
        }

        /// <summary>
        /// Agrega un grupo de items a la lista.
        /// </summary>
        /// <param name="item">Items a agregar.</param>
        public void AgregarItems(object[] items)
        {
            ListControlItem[] listItems = new ListControlItem[items.Length];
            for (int i = 0; i < listItems.Length; i++)
            {
                listItems[i] = Instantiate(this.ItemPrefab) as ListControlItem;
                listItems[i].Valor = items[i];
            }

            this._items.AddRange(listItems);

            for (int i = 0; i < listItems.Length; i++)
            {
                this.AgregarObjetoItemAlFinal(listItems[i]);
                this.eventoAlAgregarItem(new ListItemEventArgs(listItems[i]));
            }
        }

        /// <summary>
        /// Quita el Item que se encuentra en el índice especificado.
        /// </summary>
        /// <param name="indice">Índice del item a quitar.</param>
        public void QuitarItem(int indice)
        {
            this.QuitarItem(this._items[indice]);
        }

        /// <summary>
        /// Quita un Item de la lista.
        /// </summary>
        /// <param name="item">Item a quitar.</param>
        /// <returns>TRUE si el item fue quitado, de lo contrario FALSE.</returns>
        public bool QuitarItem(ListControlItem item)
        {
            if (this._items.Remove(item))
            {
                object valor = item.Valor;
                if(this._SelectedItems.Remove(item))
                    this.eventoSelectedItemsChanged(System.EventArgs.Empty);

                this._scroll.EliminarElemento(item.gameObject);
                this.ActualizarPosicionesDeItems();
                this.eventoAlQuitarItem(new ListItemQuitadoEventArgs(valor));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Quita un grupo de Items de la lista.
        /// </summary>
        /// <param name="item">Items a quitar.</param>
        public void QuitarItems(ListControlItem[] items)
        {
            object valor;
            foreach (ListControlItem item in items)
            {
                valor = item.Valor;
                this._items.Remove(item);
                if (this._SelectedItems.Remove(item))
                    this.eventoSelectedItemsChanged(System.EventArgs.Empty);

                this._scroll.EliminarElemento(item.gameObject);
                this.eventoAlQuitarItem(new ListItemQuitadoEventArgs(valor));
            }
            this.ActualizarPosicionesDeItems();
        }

        #endregion


        #region Eventos de la interfaz

        private void ListItem_AlCambiarSeleccion(object sender, System.EventArgs e)
        {
            ListControlItem item = (ListControlItem)sender;
            if (item.Seleccionado)
            {
                this._SelectedItems.Add(item);
                this.eventoItemSeleccionado(new ListItemEventArgs(item));
            }
            else
            {
                this._SelectedItems.Remove(item);
                this.eventoItemDeseleccionado(new ListItemEventArgs(item));
            }

            this.eventoSelectedItemsChanged(e);
        }

        #endregion
    }

    public class ListItemQuitadoEventArgs : System.EventArgs
    {
        private object valor;
        /// <summary>
        /// Obtiene el valor que tenía el Item que fue quitado.
        /// </summary>
        public object Valor
        {
            get
            {
                return this.valor;
            }
        }

        public ListItemQuitadoEventArgs(object valor)
        {
            this.valor = valor;
        }

    }

    public class ListItemEventArgs : System.EventArgs
    {
        private ListControlItem item;
        /// <summary>
        /// Obtiene el Item involucrado en en evento.
        /// </summary>
        public ListControlItem Item
        {
            get
            {
                return this.item;
            }
        }

        public ListItemEventArgs(ListControlItem item)
        {
            this.item = item;
        }

    }
}