using UnityEngine;
using System;
using System.Collections;
using Entrenamiento.Nucleo;
using Interfaz.Genericos;

namespace Entrenamiento.GUI.Interruptores
{
    [RequireComponent(typeof(MenuRadialDesplegable))]
    [Obsolete("Usar InterruptorGuiController en su lugar.", true)]
    public class InterruptorController : MonoBehaviour
    {
        private const int GRADOS_DE_DIFERENCIA_DEL_CENTRO = 40;
        
        #region Campos públicos

        public NombresDeInterruptores NombreDelInterruptor = NombresDeInterruptores.Desconocido;
        public EstadosDeInterruptores[] EstadosPermitidos = new EstadosDeInterruptores[3];
        /// <summary>
        /// Velocidad de la animación en segundos.
        /// </summary>
        public float Velocidad = 0.3f;
        public EstadosDeInterruptores LaPosicion = EstadosDeInterruptores.Desconocido;

        #endregion


        #region Campos privados

        //private EstadosDeInterruptores _PosicionActual = EstadosDeInterruptores.Centro;
        private Transform _ColaDerata;
        
        
        private UnityEngine.Quaternion _CentroPos;
        private UnityEngine.Quaternion _ArribaPos;
        private Quaternion _AbajoPos;
        
        private MenuRadialDesplegable _menuRadialDesplegable;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene o establece la posición del interruptor.
        /// </summary>
        public Entrenamiento.Nucleo.EstadosDeInterruptores PosicionActual
        {
            get
            {
                return this.Interruptor.EstadoActual;
            }
            set
            {
                try
                {
                    this.Interruptor.EstadoActual = value;
                }
                catch (InvalidOperationException ex)
                {
                    this.LaPosicion = this.Interruptor.EstadoActual;
                    Debug.Log("Posición \"" + value + "\" inválida en el interruptor \"" +
                        this.Interruptor.Nombre + "\".\n\n" + ex.Message);
                }
            }
        }

        private bool _AnimacionActiva = false;
        /// <summary>
        /// Obtiene un valor indicando si la animación está en proceso.
        /// </summary>
        public bool AnimacionActiva
        {
            get
            {
                return this._AnimacionActiva;
            }
        }

        private Interruptor _Interruptor;
        /// <summary>
        /// Obtiene o establece el interruptor lógico vinculado a éste interruptor gráfico.
        /// </summary>
        public Interruptor Interruptor
        {
            get
            {
                return this._Interruptor;
            }
            set
            {
                if (this._Interruptor != value)
                {
                    if (this._Interruptor != null)
                    {
                        this._Interruptor.AlCambiarSuEstado -= this._Interruptor_AlCambiarSuEstado;// Deja de estar a la escucha.
                        if (this.AnimacionActiva)
                        {
                            this.StopCoroutine("AnimacionDePosicion");
                            this._AnimacionActiva = false;
                        }
                    }

                    this._Interruptor = value;

                    if (this._Interruptor != null)
                    {
                        this._Interruptor.AlCambiarSuEstado += this._Interruptor_AlCambiarSuEstado;
                        if (!this._Interruptor.EsUnEstadoPermitido(this.LaPosicion))
                        {// Coloca el interruptor en alguna posición permitida
                            this.LaPosicion = this.Interruptor.EstadosPermitidos[0];
                            this.PosicionActual = this.LaPosicion;
                        }
                    }
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se desencadena cuando la propiedad PosicionActual cambia de valor.
        /// </summary>
        public event EventHandler AlCambiarDePosicion;

        private void eventoAlCambiarDePosicion(EventArgs e)
        {
            if (this.AlCambiarDePosicion != null)
                this.AlCambiarDePosicion(this, e);
        }

        #endregion


        #region Eventos de Unity

        private void Update()
        {
            //if (Input.GetKey(KeyCode.RightArrow))
            //    this.PosicionActual = EstadosDeInterruptores.Abajo;
            //else if (Input.GetKey(KeyCode.LeftArrow))
            //    this.PosicionActual = EstadosDeInterruptores.Arriba;
            //else if (Input.GetKey(KeyCode.UpArrow))
            //    this.PosicionActual = EstadosDeInterruptores.Centro;

            this.PosicionActual = this.LaPosicion;
        }

        private void Start()
        {
            if (this._menuRadialDesplegable != null)
                this.incializarMenuDesplegable();
        }

        private void Awake()
        {
            this._ColaDerata = this.transform.Find("Cola_de_interruptor");
            this._CentroPos = this._ColaDerata.localRotation;
            this._ArribaPos = this._CentroPos * Quaternion.AngleAxis(-1 * GRADOS_DE_DIFERENCIA_DEL_CENTRO, Vector3.right);
            this._AbajoPos = this._CentroPos * Quaternion.AngleAxis(GRADOS_DE_DIFERENCIA_DEL_CENTRO, Vector3.right);
            this._menuRadialDesplegable = this.transform.GetComponent<MenuRadialDesplegable>();
            this.Interruptor = new Interruptor(this.NombreDelInterruptor, this.EstadosPermitidos);
        }

        #endregion


        #region Métodos de la clase

        private void _menuRadialDesplegable_AlSeleccionarItem(object sender, MenuRadialDesplegable.MenuRadialItemEventArgs e)
        {
            this.PosicionActual = (EstadosDeInterruptores)e.Item.Valor;
        }

        private void _Interruptor_AlCambiarSuEstado(object sender, EventArgs e)
        {
            this.LaPosicion = this.Interruptor.EstadoActual;
            if (!this.AnimacionActiva)
                this.StartCoroutine(this.AnimacionDePosicion());
            this.eventoAlCambiarDePosicion(new EventArgs());
        }

        private void AjustarPosicionSinAnimacion()
        {
            EstadosDeInterruptores posDestino = this.PosicionActual;
            Quaternion destino;
            switch (posDestino)
            {
                case EstadosDeInterruptores.Arriba:
                    destino = this._ArribaPos;
                    break;

                case EstadosDeInterruptores.Abajo:
                    destino = this._AbajoPos;
                    break;

                default:// Centro
                    destino = this._CentroPos;
                    break;
            }

            this._ColaDerata.localRotation = destino;
        }

        /// <summary>
        /// Agrega items al menú radial desplegable según la variable EstadosPermitidos.
        /// </summary>
        private void incializarMenuDesplegable()
        {
            object[] items = new object[EstadosPermitidos.Length];
            EstadosPermitidos.CopyTo(items, 0);
            this._menuRadialDesplegable.Items = items;

            // Eventos
            this._menuRadialDesplegable.AlSeleccionarItem += this._menuRadialDesplegable_AlSeleccionarItem;
        }

        private IEnumerator AnimacionDePosicion()
        {
            this._AnimacionActiva = true;
            Quaternion destino;
            EstadosDeInterruptores posDestino = this.PosicionActual;
            switch (posDestino)
            {
                case EstadosDeInterruptores.Arriba:
                    destino = this._ArribaPos;
                    break;

                case EstadosDeInterruptores.Abajo:
                    destino = this._AbajoPos;
                    break;

                default:// Centro
                    destino = this._CentroPos;
                    break;
            }

            Quaternion origen = this._ColaDerata.localRotation;
            float proporcionDeInterpolacion = 0;

            while (proporcionDeInterpolacion <= 1)
            {
                this._ColaDerata.localRotation = Quaternion.Slerp(origen, destino, proporcionDeInterpolacion);
                yield return null;

                proporcionDeInterpolacion += Time.deltaTime / this.Velocidad;

                if (this.PosicionActual != posDestino)
                {
                    posDestino = this.PosicionActual;
                    switch (posDestino)
                    {
                        case EstadosDeInterruptores.Arriba:
                            destino = this._ArribaPos;
                            break;

                        case EstadosDeInterruptores.Abajo:
                            destino = this._AbajoPos;
                            break;

                        default:// Centro
                            destino = this._CentroPos;
                            break;
                    }
                    origen = this._ColaDerata.localRotation;
                    proporcionDeInterpolacion = 0;
                }
            }
            this._AnimacionActiva = false;
        }

        #endregion
    }
}