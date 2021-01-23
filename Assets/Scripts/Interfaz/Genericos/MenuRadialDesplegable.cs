using UnityEngine;
using System.Collections;

namespace Interfaz.Genericos
{
    [RequireComponent(typeof(Collider))]
    public class MenuRadialDesplegable : MonoBehaviour
    {
        #region Campos públicos

        public MenuRadialItem MenuItemPrefab;
        public float DistanciaEntreItems = 4f;

        /// <summary>
        /// Valores string que se quieren mostrar en el menú desplegable. Nota: Preferentemente usar la propiedad Items.
        /// </summary>
        public string[] Valores = new string[0];

        #endregion


        #region Campos Privados

        /// <summary>
        /// Item sobre el cual se encuentra el cursor del mouse.
        /// </summary>
        private MenuRadialItem seleccionActual = null;
        private MenuRadialItem[] menuItems;
        private Transform contenedorDeItems = null;

        #endregion


        #region Propiedades

        private object[] items = new object[8];
        /// <summary>
        /// Obtiene o establece los items mostrados en el menú desplegable. El máximo de items mostrados son 8.
        /// </summary>
        public object[] Items
        {
            get
            {
                return this.items;
            }
            set
            {
                if (this.items != value)
                {
                    if (value.Length == 8)
                        this.items = value;
                    else
                    {
                        int indice = 0;
                        foreach (object o in value)
                        {// Asignamos los nuevos
                            if (o != null)
                                this.items[indice++] = o;
                        }

                        while (indice < 8)
                        {// Limpiamos los restantes
                            this.items[indice++] = null;
                        }
                    }

                    this.contenedorDeItems.gameObject.SetActive(true);
                    this.ActualizarItemsDelMenu();
                    this.contenedorDeItems.gameObject.SetActive(false);
                    this.eventoAlCambiarItems(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando un item del menú radial ha sido seleccionado.
        /// </summary>
        public event System.EventHandler<MenuRadialItemEventArgs> AlSeleccionarItem;

        /// <summary>
        /// Se produce cuando la propiedad Items cambia.
        /// </summary>
        public event System.EventHandler AlCambiarItems;

        private void eventoAlSeleccionarItem(MenuRadialItemEventArgs e)
        {
            if (this.AlSeleccionarItem != null)
                this.AlSeleccionarItem(this, e);
        }

        private void eventoAlCambiarItems(System.EventArgs e)
        {
            if (this.AlCambiarItems != null)
                this.AlCambiarItems(this, e);
        }

        #endregion


        #region Eventos Unity

        private void Awake()
        {
            // Inicialización
            this.contenedorDeItems = (new GameObject()).transform;
            this.contenedorDeItems.parent = this.transform;
            this.contenedorDeItems.localPosition = Vector3.zero;
            this.contenedorDeItems.name = "ContenedorDeItems";
            this.contenedorDeItems.position = new Vector3(
                    this.contenedorDeItems.position.x,
                    this.contenedorDeItems.position.y,
                    Camera.main.transform.position.z + Camera.main.nearClipPlane + 0.2f
                );
            this.menuItems = new MenuRadialItem[8];
            this.InicializarItems();
            this.contenedorDeItems.gameObject.SetActive(false);

            this.Items = this.Valores;

        }

        private void OnMouseUp()
        {
            this.MostrarMenu(false);
            if (this.seleccionActual != null)
            {
                this.eventoAlSeleccionarItem(new MenuRadialItemEventArgs(this.seleccionActual));
                this.seleccionActual = null;
            }
        }

        private void OnMouseDown()
        {
            this.MostrarMenu(true);
        }

        #endregion
        

        #region Métodos de la clase

        /// <summary>
        /// Muestra el menú desplegable.
        /// </summary>
        /// <param name="mostrar">TRUE para mostrar, de lo contrario FALSE.</param>
        private void MostrarMenu(bool mostrar)
        {
            if(mostrar)
            {// Ajustamos su posición según el mouse.
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.contenedorDeItems.position = new Vector3(
                    mousePos.x,
                    mousePos.y,
                    mousePos.z + Camera.main.nearClipPlane + 0.2f
                );
            }
            else if (this.seleccionActual != null)
            {// Feedback de selección para el usuario
                StartCoroutine(this.retrasoDeItemSeleccionado(this.seleccionActual.gameObject));
            }

            this.contenedorDeItems.gameObject.SetActive(mostrar);
        }

        IEnumerator retrasoDeItemSeleccionado(GameObject objetoAClonar)
        {
            GameObject objetoNuevo = 
                Instantiate(objetoAClonar, objetoAClonar.transform.position, objetoAClonar.transform.rotation) as GameObject;
            yield return new WaitForSeconds(0.12f);
            Destroy(objetoNuevo);
        }

        /// <summary>
        /// Actualiza los items mostrados en el menú desplegable.
        /// </summary>
        private void ActualizarItemsDelMenu()
        {
            for (int i = 0; i < 8; i++)
            {
                if (this.items[i] == null)
                    this.menuItems[i].gameObject.SetActive(false);
                else
                {
                    this.menuItems[i].gameObject.SetActive(true);
                    this.menuItems[i].Valor = this.items[i];
                }
            }
        }

        /// <summary>
        /// Instancia e inicializa los items del menú desplegable.
        /// </summary>
        private void InicializarItems()
        {
            for (int i = 0; i < 8; i++)
            {
                this.menuItems[i] = Instantiate(this.MenuItemPrefab) as MenuRadialItem;
                this.menuItems[i].transform.parent = this.contenedorDeItems;
                this.menuItems[i].AlCambiarMouseOver += this.MenuRadialItem_AlCambiarMouseOver;
            }

            this.PosicionarItemsDelMenu();
        }

        private void MenuRadialItem_AlCambiarMouseOver(object sender, System.EventArgs e)
        {
            MenuRadialItem item = (MenuRadialItem)sender;
            if (item.MouseOver)
                this.seleccionActual = item;
            else
                this.seleccionActual = null;
        }

        /// <summary>
        /// Posiciona los Items en su lugar correspondiente.
        /// </summary>
        private void PosicionarItemsDelMenu()
        {
            float dist = this.DistanciaEntreItems * 0.7f;
            this.menuItems[0].transform.localPosition = new Vector3(0, dist, 0);// Arriba
            this.menuItems[1].transform.localPosition = new Vector3(0, -dist, 0);// Abajo
            this.menuItems[2].transform.localPosition = new Vector3(this.DistanciaEntreItems, 0, 0);// Derecha
            this.menuItems[3].transform.localPosition = new Vector3(-this.DistanciaEntreItems, 0, 0);// Izquierda


            dist = this.DistanciaEntreItems * 0.8f;
            Vector3 posInicial = new Vector3(0, dist, 0);
            this.menuItems[4].transform.localPosition = posInicial;
            this.menuItems[5].transform.localPosition = posInicial;
            this.menuItems[6].transform.localPosition = posInicial;
            this.menuItems[7].transform.localPosition = posInicial;

            this.menuItems[4].transform.RotateAround(this.contenedorDeItems.position, Vector3.forward, 60f);
            this.menuItems[5].transform.RotateAround(this.contenedorDeItems.position, Vector3.forward, 300f);
            this.menuItems[6].transform.RotateAround(this.contenedorDeItems.position, Vector3.forward, 120f);
            this.menuItems[7].transform.RotateAround(this.contenedorDeItems.position, Vector3.forward, 240f);
            
            this.menuItems[4].transform.localRotation = Quaternion.identity;
            this.menuItems[5].transform.localRotation = Quaternion.identity;
            this.menuItems[6].transform.localRotation = Quaternion.identity;
            this.menuItems[7].transform.localRotation = Quaternion.identity;
        }

        #endregion

        public class MenuRadialItemEventArgs : System.EventArgs
        {
            private MenuRadialItem item;
            /// <summary>
            /// Item involucrado en el evento.
            /// </summary>
            public MenuRadialItem Item
            {
                get
                {
                    return this.item;
                }
            }

            public MenuRadialItemEventArgs(MenuRadialItem item)
            {
                this.item = item;
            }
        }
    }
}