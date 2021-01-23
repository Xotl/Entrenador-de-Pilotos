using UnityEngine;
using System;
using System.Collections.Generic;

namespace Interfaz.Utilities
{
    public class ScrollControl : MonoBehaviour
    {
        /// <summary>
        /// Listado con todos los scrolls cargados actualmente. Usado para reacomodar sus posiciones.
        /// </summary>
        static private List<ScrollControl> ListadoDeScrolls = new List<ScrollControl>();

        /// <summary>
        /// Prefab usado para instanciar "LaCamara".
        /// </summary>
        public Camera PrefabDeCamara;

        /// <summary>
        /// Prefab usado para instanciar "ScrollBarH" y "ScrollBarV".
        /// </summary>
        public Transform prefabScrollBar;

        /// <summary>
        /// Prefab usado para instanciar "ScrollSlideH" y "ScrollSlideV".
        /// </summary>
        public Transform prefabScrollSlide;

        /// <summary>
        /// Padding dentro del ScrollSpace.
        /// </summary>
        public float ScrollPadding;

        /// <summary>
        /// Ancho o alto (según si es horizontal o vertical) en unidades Unity que deben tener las scrollBars;
        /// </summary>
        public float ScrollBarsTam = 0.2f;

        public bool OcultarScrollBarH = false;
        public bool OcultarScrollBarV = false;

        #region Campos

        /// <summary>
        /// Posición en coordenadas globales del ScrollSlideH cuando la propiedad ScrollH es 0.
        /// </summary>
        private Vector3 _initPosScrollSlideH;

        /// <summary>
        /// Posición en coordenadas globales del ScrollSlideV cuando la propiedad ScrollV es 0.
        /// </summary>
        private Vector3 _initPosScrollSlideV;

        /// <summary>
        /// Barra que representa los límites del ScrollBar horizontal.
        /// </summary>
        private Transform ScrollBarH;

        /// <summary>
        /// Barra que representa los límites del ScrollBar vertical.
        /// </summary>
        private Transform ScrollBarV;

        /// <summary>
        /// ScrollSlide de la ScrollBarH.
        /// </summary>
        private Transform ScrollSlideH;

        /// <summary>
        /// ScrollSlide de la ScrollBarV.
        /// </summary>
        private Transform ScrollSlideV;

        /// <summary>
        /// Cámara usada para visualización del scroll.
        /// </summary>
        private Camera LaCamara;

        /// <summary>
        /// Listado de todos los elementos dentro de contenedor.
        /// </summary>
        private List<GameObject> ElementosDentroDelScroll;

        /// <summary>
        /// Objeto que contendrá todos los elementos del scroll.
        /// </summary>
        private Transform Contenedor;

        /// <summary>
        /// Posición Top-Left del área que ocupan los elementos del contenedor.
        /// </summary>
        private Vector3 _boundsTopLeftPos;

        /// <summary>
        /// Punto TopLeft que debe tener el contenedor de los elementos cuando el scroll en ambos ejes es 0.
        /// </summary>
        private Vector3 _contenedorRelTopLeftPos;

        #endregion

        #region Propiedades
        
        /// <summary>
        /// Obtiene todos los elementos agregados a este ScrollControl.
        /// </summary>
        public GameObject[] ElementosDelScroll
        {
            get
            {
                return this.ElementosDentroDelScroll.ToArray();
            }
        }

        private float _Padding = 0f;
        /// <summary>
        /// Obtiene o establece el padding dentro del scroll.
        /// </summary>
        public float Padding
        {
            get
            {
                return this._Padding;
            }

            set
            {
                if (this._Padding != value && value >= 0)
                {
                    this._Padding = value;
                    this.ScrollPadding = this._Padding;
                    this.ActualizarAreaBounds();
                }
            }
        }

        private Bounds _bounds;
        /// <summary>
        /// Obtiene o establece las dimensiones que ocupan todos los elementos.
        /// </summary>
        private Bounds bounds
        {
            get
            {
                return this._bounds;
            }
            set
            {
                if (value.center != this._bounds.center || value.size != this._bounds.size)
                {// Si las dimensiones son diferentes entra aquí
                    this._bounds = value;
                    this._boundsTopLeftPos = this.CalcularBoundsTopLeft();
                    this._contenedorRelTopLeftPos = this.CalcularTopLeftDeScrollView();
                    this.eventoAlCambiarBounds(new EventArgs());
                }
            }
        }

        /// <summary>
        /// Obtiene las dimensiones que ocupan todos los elementos.
        /// </summary>
        public Bounds AreaBounds
        {
            get
            {
                return this.bounds;
            }
        }

        private float _ScrollH = 0;
        /// <summary>
        /// Obtiene o establece la posición del scroll horizontal. Asignar 0 lo posiciona al inicio.
        /// </summary>
        public float ScrollH
        {
            get
            {
                return this._ScrollH;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > this.MaxScrollH)
                    value = this.MaxScrollH;

                if (this._ScrollH != value)
                {
                    this._ScrollH = value;
                    this.ActualizarScrollH();
                    this.eventoAlHacerScrollHorizontal(EventArgs.Empty);
                }
            }
        }

        private float _ScrollV = 0;
        /// <summary>
        /// Obtiene o establece la posición del scroll vertical. Asignar 0 lo posiciona al inicio.
        /// </summary>
        public float ScrollV
        {
            get
            {
                return this._ScrollV;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > this.MaxScrollV)
                    value = this.MaxScrollV;

                if (this._ScrollV != value)
                {
                    this._ScrollV = value;
                    this.ActualizarScrollV();
                    this.eventoAlHacerScrollVertical(EventArgs.Empty);
                }
            }
        }

        private float _MaxScrollH = 0;
        /// <summary>
        /// Obtiene el máximo scroll horizontal que se puede hacer.
        /// </summary>
        public float MaxScrollH
        {
            get 
            {
                return this._MaxScrollH;
            }
        }

        private float _MaxScrollV = 0;
        /// <summary>
        /// Obtiene el máximo scroll vertical que se puede hacer.
        /// </summary>
        public float MaxScrollV
        {
            get
            {
                return this._MaxScrollV;
            }
        }

        /// <summary>
        /// Obtiene las dimensiones que ocupa el el control deplegado, es decir, el pequeño.
        /// </summary>
        public Bounds ControlAreaBounds
        {
            get
            {
                Bounds bounds = this.renderer.bounds;
                Vector3 expansion = Vector3.zero;
                Vector3 posicion = bounds.center;

                if (!this.OcultarScrollBarH)
                    expansion += Vector3.up;

                if (!this.OcultarScrollBarV)
                    expansion += Vector3.right;

                expansion *= this.ScrollBarsTam;
                bounds.Expand(expansion);
                posicion += new Vector3(expansion.x / 2, -expansion.y / 2, 0);

                return new Bounds(posicion, bounds.size);
            }
        }

        #endregion

        #region Eventos de la clase

        /// <summary>
        /// Se desencadena cuando se modifica AreaBounds.
        /// </summary>
        public event EventHandler AlCambiarBounds;

        /// <summary>
        /// Se desencadena cuando se agrega un nuevo elemento a la lista de elementos.
        /// </summary>
        public event EventHandler AlAgregarElemento;

        /// <summary>
        /// Se desencadena cuando se quita un elemento de la lista de elementos.
        /// </summary>
        public event EventHandler AlQuitarElemento;

        /// <summary>
        /// Se desencadena cuando la propiedad ScrollX cambia de valor.
        /// </summary>
        public event EventHandler AlHacerScrollHorizontal;

        /// <summary>
        /// Se desencadena cuando la propiedad ScrollY cambia de valor.
        /// </summary>
        public event EventHandler AlHacerScrollVertical;

        /// <summary>
        /// Se desencadena cuando la propiedad MaxScrollV cambia.
        /// </summary>
        public event EventHandler AlCambiarMaxScrollV;

        /// <summary>
        /// Se desencadena cuando la propiedad MaxScrollH cambia.
        /// </summary>
        public event EventHandler AlCambiarMaxScrollH;


        private void eventoAlHacerScrollHorizontal(EventArgs e)
        {
            if (this.AlHacerScrollHorizontal != null)
                this.AlHacerScrollHorizontal(this, e);
        }

        private void eventoAlHacerScrollVertical(EventArgs e)
        {
            if (this.AlHacerScrollVertical != null)
                this.AlHacerScrollVertical(this, e);
        }

        private void eventoAlCambiarMaxScrollH(EventArgs e)
        {
            if (this.AlCambiarMaxScrollH != null)
                this.AlCambiarMaxScrollH(this, e);
        }

        private void eventoAlCambiarMaxScrollV(System.EventArgs e)
        {
            if (this.AlCambiarMaxScrollV != null)
                this.AlCambiarMaxScrollV(this, e);
        }


        private void eventoAlCambiarBounds(EventArgs e)
        {
            if (this.AlCambiarBounds != null)
                this.AlCambiarBounds(this, e);
        }

        private void eventoAlAgregarElemento(EventArgs e)
        {
            if (this.AlAgregarElemento != null)
                this.AlAgregarElemento(this, e);
        }

        private void eventoAlQuitarElemento(EventArgs e)
        {
            if (this.AlQuitarElemento != null)
                this.AlQuitarElemento(this, e);
        }

        #endregion

        #region Eventos Unity

        private void Awake()
        {
            // Instanciamos objetos necesarios
            ElementosDentroDelScroll = new List<GameObject>();
            this.LaCamara = Instantiate(this.PrefabDeCamara) as Camera;
            this.Contenedor = (new GameObject("Contenedor de scroll")).transform;
            this.InicializacionDeScrollBars();// Instanciación de las ScrollBars

            // Inicialización
            this.Contenedor.parent = this.LaCamara.transform;
            this.Contenedor.localPosition = new Vector3(0, 0, this.LaCamara.nearClipPlane + 100.1f);
            ScrollControl.ListadoDeScrolls.Add(this);
            this.PosicionarCamara();
            this.AjustarTamanoDeCamara();
            this.RecalcularBounds();
            this.ActualizarScrollH();
            this.ActualizarScrollV();

            // Eventos dependientes
            this.AlCambiarMaxScrollH += new EventHandler(this.ScrollControl_AlCambiarMaxScrollH);
            this.AlCambiarMaxScrollV += new EventHandler(this.ScrollControl_AlCambiarMaxScrollV);
            this.AlCambiarBounds += new EventHandler(this.ScrollControl_AlCambiarBounds);
        }
        
        #region Eventos dependientes del Awake

        private void ScrollControl_AlCambiarBounds(object sender, EventArgs e)
        {
            this.ActualizarMaxScrolls();
            this.ActualizarScrollH();
            this.ActualizarScrollV();
        }

        private void ScrollControl_AlCambiarMaxScrollV(object sender, EventArgs e)
        {
            if (this.ScrollV > this.MaxScrollV)
                this.ScrollV = this.MaxScrollV;
        }

        private void ScrollControl_AlCambiarMaxScrollH(object sender, EventArgs e)
        {
            if (this.ScrollH > this.MaxScrollH)
                this.ScrollH = this.MaxScrollH;
        }

        #endregion

        private void OnDestroy()
        {
            ScrollControl.ListadoDeScrolls.Remove(this);
            if(Camera.main != null)
                this.ReposicionarLosScrolls();
            
            Destroy(this.Contenedor);
            Destroy(this.LaCamara);

            // Eventos dependientes
            this.AlCambiarMaxScrollH -= this.ScrollControl_AlCambiarMaxScrollH;
            this.AlCambiarMaxScrollV -= this.ScrollControl_AlCambiarMaxScrollV;
            this.AlCambiarBounds -= this.ScrollControl_AlCambiarBounds;
        }

        private void Update()
        {
            this.AjustarTamanoDeCamara();
            this.Padding = this.ScrollPadding;
        }

        #endregion

        #region Métodos base del scroll

        /// <summary>
        /// Quita y destruye todos los GameObjects agregados al scroll.
        /// </summary>
        public void Limpiar()
        {
            foreach (GameObject hijo in this.ElementosDentroDelScroll)
            {
                Destroy(hijo);
                hijo.transform.parent = null;
            }
            
            this.ElementosDentroDelScroll.Clear();
            this.ActualizarAreaBounds();
            this.eventoAlQuitarElemento(EventArgs.Empty);
        }

        /// <summary>
        /// Similar al SetActive de GameObject. Activa o desactiva este objeto, eso incluye a sus hijos y objetos dependientes.
        /// </summary>
        /// <param name="value">TRUE para activar, de lo contrario FALSE.</param>
        public void SetActive(bool value)
        {
            this.LaCamara.gameObject.SetActive(value);
            this.gameObject.SetActive(value);
        }

        /// <summary>
        /// Cambia la capa del objeto y la de sus hijos.
        /// </summary>
        /// <param name="objeto">Objeto al que se le desea cambiar la capa.</param>
        /// <param name="layer">Nueva capa que se desea asignar.</param>
        private void AsignarCapa(Transform objeto, int layer)
        {
            foreach (Transform obj in objeto.GetComponentsInChildren<Transform>())
            {
                obj.gameObject.layer = layer;
            }
        }

        /// <summary>
        /// Reposiciona las cámaras de los Scrolls según sus índices en la lista.
        /// </summary>
        private void ReposicionarLosScrolls()
        {
            foreach (ScrollControl camara in ScrollControl.ListadoDeScrolls)
            {
                camara.ReposicionarCamara();
            }
        }

        /// <summary>
        /// Reposiciona la cámara según su índice.
        /// </summary>
        public void ReposicionarCamara()
        {
            this.PosicionarCamara();
        }

        /// <summary>
        /// Ajusta la posición de la cámara del scroll.
        /// </summary>
        private void PosicionarCamara()
        {
            int indice = ScrollControl.ListadoDeScrolls.IndexOf(this);
            if (this.LaCamara != null)// Validación necesaria para evitar Excepción cuando se cierra la aplicación.
            {
                this.LaCamara.transform.position =
                    new Vector3(Camera.main.transform.position.x - 40, 0, Camera.main.transform.position.z + indice * (this.PrefabDeCamara.farClipPlane + 1));
            }
        }

        /// <summary>
        /// Calcula y ajusta el tamaño de la cámara al ViewArea (El área donde se graficará el control).
        /// </summary>
        private void AjustarTamanoDeCamara()
        {
            Bounds dimensiones = this.renderer.bounds;

            float posX = dimensiones.center.x - dimensiones.size.x / 2;
            float posY = dimensiones.center.y - dimensiones.size.y / 2;

            Vector3 posBottomLeft =
                   Camera.main.WorldToViewportPoint(new Vector3(posX, posY, this.transform.position.z));

            posX += dimensiones.size.x;
            posY += dimensiones.size.y;

            Vector3 posTopRight =
                Camera.main.WorldToViewportPoint(new Vector3(posX, posY, this.transform.position.z));

            posX = posTopRight.x - posBottomLeft.x;
            posY = posTopRight.y - posBottomLeft.y;

            this.LaCamara.rect = new Rect(posBottomLeft.x, posBottomLeft.y, posX, posY);
            this.LaCamara.orthographicSize = Camera.main.orthographicSize * this.LaCamara.rect.height;
        }

        /// <summary>
        /// Agrega un elemento al scroll donde el z-index será 50.
        /// </summary>
        /// <param name="elemento">Elemento a agregar.</param>
        /// <param name="posicion">Posicion donde se desea agregar.</param>
        public void AgregarElemento(GameObject elemento, Vector2 posicion)
        {
            this.AgregarElemento(elemento, new Vector3(posicion.x, posicion.y, 50));
        }

        /// <summary>
        /// Agrega un elemento al scroll.
        /// </summary>
        /// <param name="elemento">Elemento a agregar.</param>
        /// <param name="posicion">Posicion donde se desea agregar. El componente Z actúa como el z-index de CSS. El z-index debe estar dentro del rango [0, 100].</param>
        public void AgregarElemento(GameObject elemento, Vector3 posicion)
        {
            this.AgregarElemento(elemento, posicion, null);
        }

        /// <summary>
        /// Agrega un elemento como hijo de algún otro elemento que esté dentro del scroll.
        /// </summary>
        /// <param name="elemento">Elemento a agregar.</param>
        /// <param name="posicion">Posición relativa al objeto padre.</param>
        /// <param name="objetoPadre">Objeto padre del elemento o NULL para agregar en la raíz. Nota: El objeto padre debe estar dentro del scroll.</param>
        public void AgregarElemento(GameObject elemento, Vector3 posicion, Transform objetoPadre)
        {
            if (objetoPadre == null)
            {
                objetoPadre = this.Contenedor;
            }
            else if (!objetoPadre.IsChildOf(this.Contenedor))
            {
                throw new InvalidOperationException("El objeto padre (\"" + objetoPadre.name + "\") no está dentro del scroll.");
            }

            if (posicion.z < 0 || posicion.z > 100)
            {
                throw new InvalidOperationException("La posición en el eje Z debe estar dentro del rango [0, 100].");
            }

            if (elemento.transform.IsChildOf(this.Contenedor))
            {
                throw new InvalidOperationException("Este elemento ya se encuentra dentro del scroll.");
            }

            // Asignación al contenedor
            elemento.transform.parent = objetoPadre;
            elemento.transform.localPosition = new Vector3(posicion.x, posicion.y, -1 * posicion.z);
            this.AsignarCapa(elemento.transform, LayerMask.NameToLayer("Scroll"));

            foreach (Renderer r in elemento.GetComponentsInChildren<Renderer>())
            {
                if (r.transform.position.z > this.Contenedor.position.z || r.transform.position.z < (this.Contenedor.position.z - 100.1f))
                {
                    string nombeOriginal = r.name;
                    r.name += " [Fuera de los límites]";// Edito el nombre original para que sea más fácil identificarlo
                    throw new InvalidOperationException("Un elemento que forma parte de \"" + elemento.name +
                        "\" está fuera de los límites. El nombre del elemento fuera de los límites es: " + nombeOriginal);
                }
            }


            this.ElementosDentroDelScroll.Add(elemento.gameObject);
            
            if (this.ElementosDentroDelScroll.Count == 1)
                this.ActualizarAreaBounds();// Si es el primero entonces calculamos las dimensiones
            else
                this.ExpandirAreaBounds(this.CalcularDimensionesDeObjeto(elemento));// Si hay otros, entonces sólo encapsulamos

            this.eventoAlAgregarElemento(EventArgs.Empty);
        }

        /// <summary>
        /// Quita un elemento de este control y posteriormente lo destruye.
        /// </summary>
        /// <param name="elemento">Objeto que se desea Eliminar.</param>
        /// <returns>Devuelve True si el elemento se eliminó, de lo contrario FALSE.</returns>
        public bool EliminarElemento(GameObject elemento)
        {
            if (this.QuitarElemento(elemento))
            {
                Destroy(elemento);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Quita un elemento de este control. Nota: El "Layer" del elemento y de sus hijos se queda en "Scroll".
        /// </summary>
        /// <param name="elemento">Objeto que se desea quitar.</param>
        /// <returns>Devuelve True si el elemento se quitó, de lo contrario FALSE.</returns>
        public bool QuitarElemento(GameObject elemento)
        {
            if (this.ElementosDentroDelScroll.Remove(elemento))
            {
                elemento.transform.parent = null;
                this.ActualizarAreaBounds();
                this.eventoAlQuitarElemento(EventArgs.Empty);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Quita un elemento de este control y posteriormente lo destruye.
        /// </summary>
        /// <param name="indice">Indice del objeto a eliminar.</param>
        public void EliminarElemento(int indice)
        {
            GameObject obj = this.ElementosDentroDelScroll[indice];
            this.QuitarElemento(indice);
            Destroy(obj);
        }

        /// <summary>
        /// Quita un elemento de este control. Nota: El elemento no es destruido.
        /// </summary>
        /// <param name="indice">Indice del objeto a quitar.</param>
        public void QuitarElemento(int indice)
        {
            this.ElementosDentroDelScroll[indice].transform.parent = null;
            this.ElementosDentroDelScroll.RemoveAt(indice);
            this.RecalcularBounds();
            this.eventoAlQuitarElemento(new EventArgs());
        }

        /// <summary>
        /// Calcula las dimensiones de un objeto dado y sus hijos.
        /// </summary>
        /// <param name="objeto">Objeto del que se desean las dimensiones.</param>
        /// <returns>Dimensiones del objeto junto con sus hijos.</returns>
        private Bounds CalcularDimensionesDeObjeto(GameObject objeto)
        {
            Bounds bounds = new Bounds(objeto.transform.position, Vector3.zero);
            Renderer[] elementos = objeto.GetComponentsInChildren<Renderer>();
            if (elementos.Length > 0)
            {// Calcula las dimensiones
                bounds = new Bounds(elementos[0].bounds.center, elementos[0].bounds.size);
                for (int i = 1; i < elementos.Length; i++)
                {
                    bounds.Encapsulate(elementos[i].bounds);
                }
            }
            return bounds;
        }

        /// <summary>
        /// Encapsula al AreaBounds las nuevas dimensiones dadas.
        /// </summary>
        /// <param name="bounds">Bounds que se encapsularán al AreaBounds.</param>
        private void ExpandirAreaBounds(Bounds bounds)
        {
            Bounds nuevo = this.bounds;
            nuevo.Encapsulate(bounds);
            this.bounds = nuevo;
        }

        /// <summary>
        /// Actualiza las dimensiones del AreaBounds según los elementos dentro de Contenedor.
        /// </summary>
        /// <returns>Objeto con las nuevas dimensiones.</returns>
        private Bounds ActualizarAreaBounds()
        {
            Bounds bounds = new Bounds(this.Contenedor.position, Vector3.zero);
            bounds.Encapsulate(this.CalcularDimensionesDeObjeto(this.Contenedor.gameObject));
            bounds.Expand(new Vector3(this.Padding, this.Padding, 0));// Le agrega el padding
            this.bounds = bounds;
            return bounds;
        }

        /// <summary>
        /// Forza la actualización de las dimensiones según los objetos dentro del Contenedor.
        /// </summary>
        public void RecalcularBounds()
        {
            this.ActualizarAreaBounds();
        }

        #endregion

        #region Métodos del scrolling
        
        /// <summary>
        /// Obtiene la posicón Top-Left del AreaBounds en coordenadas globales.
        /// </summary>
        /// <returns>Vector3 con la posición.</returns>
        private Vector3 CalcularBoundsTopLeft()
        {
            return new Vector3(
                    this.AreaBounds.center.x - this.AreaBounds.size.x / 2,
                    this.AreaBounds.center.y + this.AreaBounds.size.y / 2,
                    this.AreaBounds.center.z
                );
        }

        /// <summary>
        /// Actualiza el valor de _MaxScrollH y _MaxScrollV.
        /// </summary>
        private void ActualizarMaxScrolls()
        {
            Bounds scrollViewTam = this.renderer.bounds;//Obtiene el tamaño del ViewArea

            // Se calcula el _MaxScrollH
            float maxScroll = this.AreaBounds.size.x - scrollViewTam.size.x;
            if (maxScroll < 0)
                maxScroll = 0;

            if (this._MaxScrollH != maxScroll)
            {
                this._MaxScrollH = maxScroll;
                this.eventoAlCambiarMaxScrollH(EventArgs.Empty);
            }

            // Se calcula el _MaxScrollV
            maxScroll = this.AreaBounds.size.y - scrollViewTam.size.y;
            if (maxScroll < 0)
                maxScroll = 0;

            if (this._MaxScrollV != maxScroll)
            {
                this._MaxScrollV = maxScroll;
                this.eventoAlCambiarMaxScrollV(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Realiza el scroll horizontal según los valores actuales.
        /// </summary>
        private void ActualizarScrollH()
        {
            this.Contenedor.position = new Vector3(
                    this._contenedorRelTopLeftPos.x - this.ScrollH,
                    this.Contenedor.position.y,
                    this.Contenedor.position.z
                );
        }

        /// <summary>
        /// Realiza el scroll vertical según los valores actuales.
        /// </summary>
        private void ActualizarScrollV()
        {
            this.Contenedor.position = new Vector3(
                    this.Contenedor.position.x,
                    this._contenedorRelTopLeftPos.y + this.ScrollV,
                    this.Contenedor.position.z
                );
        }

        /// <summary>
        /// Calcula el punto TopLeft que debe tener el contenedor de los elementos cuando el scroll en ambos ejes es 0.
        /// </summary>
        /// <returns>Punto Top-Left del scroll.</returns>
        private Vector3 CalcularTopLeftDeScrollView()
        {
            Vector3 camTopLeft = this.LaCamara.ViewportToWorldPoint(Vector3.up);
            camTopLeft = new Vector3(
                    this.Contenedor.position.x + camTopLeft.x - this._boundsTopLeftPos.x,
                    this.Contenedor.position.y + camTopLeft.y - this._boundsTopLeftPos.y,
                    this.Contenedor.position.z
                );
            return camTopLeft;
        }

        #endregion

        #region Métodos del ScrollBar

        /// <summary>
        /// Método para la inicialización de las ScrollBars.
        /// </summary>
        private void InicializacionDeScrollBars()
        {
            this.ScrollBarsTam = Mathf.Abs(this.ScrollBarsTam);
            Vector3 escalaScrollBars = Vector3.one * this.ScrollBarsTam;
            Bounds tamControl = this.renderer.bounds;

            if (!this.OcultarScrollBarH)
            {// Scroll horizontal
                this.ScrollBarH = Instantiate(this.prefabScrollBar) as Transform;
                this.ScrollBarH.localScale = escalaScrollBars;// Se le aplica el grosor en unidades globales
                this.ScrollBarH.parent = this.transform;

                // Ajuste de posición y escala del ScrollBar
                this.ScrollBarH.localScale = new Vector3(1, this.ScrollBarH.localScale.y, this.ScrollBarH.localScale.z);
                Bounds tamBar = this.ScrollBarH.renderer.bounds;
                this.ScrollBarH.localPosition = Vector3.zero;
                this.ScrollBarH.position += new Vector3(0, tamBar.size.y / -2 - tamControl.size.y / 2, -0.1f);

                // ScrollSlide
                this.ScrollSlideH = Instantiate(this.prefabScrollSlide) as Transform;
                ScrollSlideController slide = this.ScrollSlideH.GetComponent<ScrollSlideController>();
                slide.Inicializar(true, this, this.ScrollBarH);
            }

            if (!this.OcultarScrollBarV)
            {// Scroll vertical
                this.ScrollBarV = Instantiate(this.prefabScrollBar) as Transform;
                this.ScrollBarV.localScale = escalaScrollBars;// Se le aplica el grosor en unidades globales
                this.ScrollBarV.parent = this.transform;

                // Ajuste de posición y escala del ScrollBar
                this.ScrollBarV.localScale = new Vector3(this.ScrollBarV.localScale.x, 1, this.ScrollBarV.localScale.z);
                Bounds tamBar = this.ScrollBarV.renderer.bounds;
                this.ScrollBarV.localPosition = Vector3.zero;
                this.ScrollBarV.position += new Vector3(tamControl.size.x / 2 + tamBar.size.x / 2, 0, -0.1f);

                // ScrollSlide
                this.ScrollSlideV = Instantiate(this.prefabScrollSlide) as Transform;
                ScrollSlideController slide = this.ScrollSlideV.GetComponent<ScrollSlideController>();
                slide.Inicializar(false, this, this.ScrollBarV);
            }
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            // ********** Dibuja las dimensiones con todo y sus scrolls **********
            bounds = this.ControlAreaBounds;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            // ******** Fin Dibuja las dimensiones con todo y sus scrolls ********
        }
    }
}