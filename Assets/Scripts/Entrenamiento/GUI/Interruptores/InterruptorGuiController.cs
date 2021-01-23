using UnityEngine;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Interruptores
{
    public abstract class InterruptorGuiController : MonoBehaviour
    {
        #region Campos privados
        
        /// <summary>
        /// Estados permitdos del interruptor.
        /// </summary>
        [SerializeField]
        private EstadosDeInterruptores[] EstadosPermitidos
            = new EstadosDeInterruptores[] { EstadosDeInterruptores.Arriba, EstadosDeInterruptores.Abajo, EstadosDeInterruptores.Centro };

        /// <summary>
        /// Posición inicial de este interruptor.
        /// </summary>
        [SerializeField]
        private EstadosDeInterruptores PosicionInicial = EstadosDeInterruptores.Centro;

        #endregion


        #region Propiedades

        private Interruptor interruptor = null;
        /// <summary>
        /// Obtiene el interruptor lógico que representa este controlador.
        /// </summary>
        public Interruptor Interruptor
        {
            get
            {
                return this.interruptor;
            }
        }

        /// <summary>
        /// Obtiene o establece la posición del interruptor.
        /// </summary>
        public EstadosDeInterruptores PosicionActual
        {
            get
            {
                return this.Interruptor.EstadoActual;
            }
            set
            {
                this.Interruptor.EstadoActual = value;
            }
        }

        
        /// <summary>
        /// Nombre que tendrá el interruptor.
        /// </summary>
        [SerializeField]
        private NombresDeInterruptores nombreDelInterruptor = NombresDeInterruptores.Desconocido;
        /// <summary>
        /// Obtiene el nombre del interruptor.
        /// </summary>
        public NombresDeInterruptores NombreDelInterruptor
        {
            get
            {
                return this.Interruptor.Nombre;
            }
        }

        /// <summary>
        /// Valor que representa el estado de la animación si ésta existe.
        /// </summary>
        protected bool _AnimacionActiva = false;
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

        #endregion


        #region Eventos Unity

        protected virtual void Awake()
        {
            this.interruptor = new Interruptor(this.nombreDelInterruptor, this.EstadosPermitidos);
            this.interruptor.EstadoActual = this.PosicionInicial;
            this.interruptor.AlCambiarSuEstado += this.interruptor_AlCambiarSuEstado;
        }

        #endregion


        #region Métodos de la clase

        private void interruptor_AlCambiarSuEstado(object sender, System.EventArgs e)
        {
            this.AlCambiarElEstadoDelInterruptor(this.Interruptor.EstadoActual, this.PosicionInicial);

            // El campo PosicionInicial es usado como la posición anterior después de la inicialización.
            this.PosicionInicial = this.Interruptor.EstadoActual;
        }

        /// <summary>
        /// Función para realizar cambios según el estado del interruptor.
        /// </summary>
        /// <param name="actual">Estado actual del interruptor.</param>
        /// <param name="anterior">Estado anterior del interruptor.</param>
        protected abstract void AlCambiarElEstadoDelInterruptor(EstadosDeInterruptores actual, EstadosDeInterruptores anterior);

        #endregion
    }
}
