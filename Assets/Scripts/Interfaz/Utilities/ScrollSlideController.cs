using UnityEngine;
using System.Collections;
using System;

namespace Interfaz.Utilities
{
    public class ScrollSlideController : MonoBehaviour
    {
        #region Campos

        /// <summary>
        /// Cantidad máxima de unidades Unity que se puede desplazar el Slide desde su posición inicial al hacer Scroll.
        /// </summary>
        private float _maxSlideUnits = 0;

        /// <summary>
        /// Posición en coordenadas globales del ScrollSlide cuando el Scroll respectivo es 0.
        /// </summary>
        private Vector3 _initPosScrollSlide;

        /// <summary>
        /// TRUE si la ScrollBar es horizontal, de lo contrario FALSE.
        /// </summary>
        private bool esHorizontal = true;

        /// <summary>
        /// ScrollBar a la que pertenece este ScrollSlide.
        /// </summary>
        private Transform ScrollBar;

        /// <summary>
        /// Control al que pertenece este ScrollSlide.
        /// </summary>
        private ScrollControl ScrollControl;

        /// <summary>
        /// Valor que indica si aún está presionando el botón del mouse desde que inicialmente hizo click sobre este Slide.
        /// </summary>
        private bool _MouseDown = false;

        #endregion

        #region Propiedades

        private bool _seHaInicilizado = false;
        /// <summary>
        /// Obtiene un valor indicando si el ScrollSlide se ha inicializado.
        /// </summary>
        public bool seHaInicilizado
        {
            get
            {
                return this._seHaInicilizado;
            }
        }

        #endregion

        #region Eventos de Unity

        private void Start()
        {
            if (!this.seHaInicilizado)
                throw new InvalidOperationException("No se ha inicializado el ScrollSlide. Hay que llamar al método Inicializar() antes de poder usarlo.");
        }

        private void OnDestroy()
        {
            if (this.esHorizontal)
            {// Horizontal
                this.ScrollControl.AlCambiarMaxScrollH -= this.ScrollControl_AlCambiarMaxScroll;
                this.ScrollControl.AlHacerScrollHorizontal -= new EventHandler(this.ScrollControl_AlHacerScroll);
            }
            else
            {// Vertical
                this.ScrollControl.AlCambiarMaxScrollV -= this.ScrollControl_AlCambiarMaxScroll;
                this.ScrollControl.AlHacerScrollVertical -= this.ScrollControl_AlHacerScroll;
            }
        }

        private void OnMouseDown()
        {
            StartCoroutine(this.ArrastarScrollSlide());
            this.eventoMouseDownEvent(System.EventArgs.Empty);
        }

        private void OnMouseUp()
        {
            this._MouseDown = false;
            this.eventoMouseUpEvent(System.EventArgs.Empty);
        }

        private void OnMouseUpAsButton()
        {
            this._MouseDown = false;
            this.eventoClick(System.EventArgs.Empty);
        }

        private void OnMouseExit()
        {
            this.eventoMouseExitEvent(System.EventArgs.Empty);
        }

        private void OnMouseEnter()
        {
            this.eventoMouseEnterEvent(System.EventArgs.Empty);
        }

        #endregion

        #region Definiión de eventos de la clase

        /// <summary>
        /// Se desencadena cuando se da click y se suelta el botón sobre el mismo. Se usa el método OnMouseUpAsButton.
        /// </summary>
        public event System.EventHandler Click;

        /// <summary>
        /// Se produce cuando se suelta el botón mouse sobre el control.
        /// </summary>
        public event System.EventHandler MouseUpEvent;

        /// <summary>
        /// Se produce cuando se presiona el botón mouse sobre el control.
        /// </summary>
        public event System.EventHandler MouseDownEvent;

        /// <summary>
        /// Se produce cuando el cursor del mouse entra al control.
        /// </summary>
        public event System.EventHandler MouseEnterEvent;

        /// <summary>
        /// Se produce cuando el cursor del mouse sale del control.
        /// </summary>
        public event System.EventHandler MouseExitEvent;


        private void eventoMouseEnterEvent(System.EventArgs e)
        {
            if (this.MouseEnterEvent != null)
                this.MouseEnterEvent(this, e);
        }

        private void eventoMouseExitEvent(System.EventArgs e)
        {
            if (this.MouseExitEvent != null)
                this.MouseExitEvent(this, e);
        }

        private void eventoMouseDownEvent(System.EventArgs e)
        {
            if (this.MouseDownEvent != null)
                this.MouseDownEvent(this, e);
        }

        private void eventoMouseUpEvent(System.EventArgs e)
        {
            if (this.MouseUpEvent != null)
                this.MouseUpEvent(this, e);
        }

        private void eventoClick(System.EventArgs e)
        {
            if (this.Click != null)
                this.Click(this, e);
        }

        #endregion

        #region Métodos del ScrollSlide

        /// <summary>
        /// Inicializa este ScrollSlide.
        /// </summary>
        /// <param name="esHorizontal">TRUE si es una barra horizontal, de lo contrario FALSE.</param>
        /// <param name="scrollControl">ScrollControl al que pertenece este ScrollSlide.</param>
        /// <param name="scrollBar">ScrollBar al que pertenece este ScrollSlide.</param>
        public void Inicializar(bool esHorizontal, ScrollControl scrollControl, Transform scrollBar)
        {
            // Asignaciones
            this.esHorizontal = esHorizontal;
            this.ScrollControl = scrollControl;
            this.ScrollBar = scrollBar;


            //Inicialización
            this.transform.parent = this.ScrollControl.transform;
            this.transform.position = this.ScrollBar.position;// Centramos el slide
            this.transform.localPosition += new Vector3(0, 0, -0.1f);// Hacemos que se grafique encima de la scrollBar


            if (this.esHorizontal)
            {// Horizontal
                this._initPosScrollSlide = new Vector3(
                        this.ScrollBar.position.x - this.ScrollBar.renderer.bounds.size.x / 2,
                        this.ScrollBar.position.y,
                        this.ScrollBar.position.z
                    );

                // Eventos
                this.ScrollControl.AlCambiarMaxScrollH += new EventHandler(this.ScrollControl_AlCambiarMaxScroll);
                this.ScrollControl.AlHacerScrollHorizontal += new EventHandler(this.ScrollControl_AlHacerScroll);
            }
            else
            {// Vertical
                this._initPosScrollSlide = new Vector3(
                        this.ScrollBar.position.x,
                        this.ScrollBar.position.y + this.ScrollBar.renderer.bounds.size.y / 2,
                        this.ScrollBar.position.z
                    );

                // Eventos
                this.ScrollControl.AlCambiarMaxScrollV += new EventHandler(this.ScrollControl_AlCambiarMaxScroll);
                this.ScrollControl.AlHacerScrollVertical += new EventHandler(this.ScrollControl_AlHacerScroll);
            }

            this.AjustarEscalaScrollSlide();
            this._seHaInicilizado = true;
        }

        #region Eventos dependientes de la inicialización

        private void ScrollControl_AlHacerScroll(object sender, EventArgs e)
        {
            this.ActualizarPosicionScrollSlide();
        }

        private void ScrollControl_AlCambiarMaxScroll(object sender, EventArgs e)
        {
            this.AjustarEscalaScrollSlide();
        }

        #endregion

        /// <summary>
        /// Ajusta la escala del ScrollSlide según el contenido del control.
        /// </summary>
        private void AjustarEscalaScrollSlide()
        {
            float maxScroll;

            if (this.esHorizontal)
            {// Horizontal
                maxScroll = this.ScrollControl.MaxScrollH;
            }
            else
            {// Vertical
                maxScroll = this.ScrollControl.MaxScrollV;
            }

            if (maxScroll == 0)
            {
                //this.ScrollBar.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                return;
            }
            else
            {
                //this.ScrollBar.gameObject.SetActive(true);
                this.gameObject.SetActive(true);
            }


            if (this.esHorizontal)
            {// Horizontal
                this.transform.localScale = new Vector3(
                    this.ScrollControl.renderer.bounds.size.x / this.ScrollControl.AreaBounds.size.x,
                    this.ScrollBar.localScale.y,
                    this.ScrollBar.localScale.z
                );

                this._initPosScrollSlide = new Vector3(
                    this.ScrollBar.position.x - this.ScrollBar.renderer.bounds.size.x / 2 + this.transform.renderer.bounds.size.x / 2,
                    this.ScrollBar.position.y,
                    this.transform.position.z
                );

                this._maxSlideUnits = this.ScrollBar.renderer.bounds.size.x - this.renderer.bounds.size.x;
            }
            else
            {// Vertical
                this.transform.localScale = new Vector3(
                    this.ScrollBar.localScale.x,
                    this.ScrollControl.renderer.bounds.size.y / this.ScrollControl.AreaBounds.size.y,
                    this.ScrollBar.localScale.z
                );

                this._initPosScrollSlide = new Vector3(
                    this.ScrollBar.position.x,
                    this.ScrollBar.position.y + this.ScrollBar.renderer.bounds.size.y / 2 - this.transform.renderer.bounds.size.y / 2,
                    this.transform.position.z
                );

                this._maxSlideUnits = this.ScrollBar.renderer.bounds.size.y - this.renderer.bounds.size.y;
            }

            this.ActualizarPosicionScrollSlide();
        }

        /// <summary>
        /// Actualiza la posición del ScrollSlide según el Scroll que se ha hecho en el ScrollControl.
        /// </summary>
        private void ActualizarPosicionScrollSlide()
        {
            if (this.esHorizontal)
            {// Horizontal
                this.transform.position = new Vector3(
                        this._initPosScrollSlide.x + this.ScrollUnitsToSlideUnits(this.ScrollControl.ScrollH),
                        this._initPosScrollSlide.y,
                        this._initPosScrollSlide.z
                    );
            }
            else
            {// Vertical
                this.transform.position = new Vector3(
                        this._initPosScrollSlide.x,
                        this._initPosScrollSlide.y - this.ScrollUnitsToSlideUnits(this.ScrollControl.ScrollV),
                        this._initPosScrollSlide.z
                    );
            }
        }

        /// <summary>
        /// Coroutine usada para el deslizamiento del Slide con el mouse.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ArrastarScrollSlide()
        {
            this._MouseDown = true;
            Vector3 posAnterior = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 posNueva;
            while (this._MouseDown)
            {
                posNueva = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (this.esHorizontal)
                {// Horizontal
                    this.ScrollControl.ScrollH += this.SlideUnitsToScrollUnits(posNueva.x - posAnterior.x);
                }
                else
                {// Vertical
                    this.ScrollControl.ScrollV -= this.SlideUnitsToScrollUnits(posNueva.y - posAnterior.y);
                }

                posAnterior = posNueva;
                yield return null;
            }
        }

        /// <summary>
        /// Convierte las unidades del Scroll en unidades del Slide.
        /// </summary>
        /// <param name="cantidad">Cantidad de unidades del Scroll a convertir.</param>
        /// <returns></returns>
        private float ScrollUnitsToSlideUnits(float ScrollUnits)
        {
            float maxScroll;
            if (this.esHorizontal)
            {// Horizontal
                maxScroll = this.ScrollControl.MaxScrollH;
            }
            else
            {// Vertical
                maxScroll = this.ScrollControl.MaxScrollV;
            }

            if (maxScroll == 0)
                return 0;

            return ScrollUnits * this._maxSlideUnits / maxScroll;
        }

        /// <summary>
        /// Convierte las unidades del Slide en unidades del Scroll.
        /// </summary>
        /// <param name="SlideUnits">Cantidad de unidades del Slide a convertir.</param>
        /// <returns></returns>
        private float SlideUnitsToScrollUnits(float SlideUnits)
        {
            float maxScroll;
            if (this.esHorizontal)
            {// Horizontal
                maxScroll = this.ScrollControl.MaxScrollH;
            }
            else
            {// Vertical
                maxScroll = this.ScrollControl.MaxScrollV;
            }

            if (this._maxSlideUnits == 0)
                return 0;
            return SlideUnits * maxScroll / this._maxSlideUnits;
        }

        #endregion
    }
}
