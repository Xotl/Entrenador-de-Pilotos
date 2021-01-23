using UnityEngine;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Interruptores
{
    public class PushButtonController : InterruptorGuiController
    {
        #region Campos privados

        /// <summary>
        /// Activa o desactiva la habilidad de cambiar el estado del Push Button mediante un click del usuario.
        /// </summary>
        [SerializeField]
        private bool EnableClickGUI = true;
        
        [SerializeField]
        private float velocidad = 1;
        private string nombreDeAnimacion;

        #endregion


        #region Eventos Unity

        protected override void Awake()
        {
            base.Awake();
            this.nombreDeAnimacion = this.animation.clip.name;
        }

        private void OnMouseUpAsButton()
        {
            if (this.EnableClickGUI)
            {
                if (this.Presionado)
                    this.Interruptor.EstadoActual = EstadosDeInterruptores.No_presionado;
                else
                    this.Interruptor.EstadoActual = EstadosDeInterruptores.Presionado;
            }
        }

        #endregion


        #region Propiedades

        private bool presionado = false;
        /// <summary>
        /// Obtiene o establece un valor indicando si se encuentra presionado el botón.
        /// </summary>
        public bool Presionado
        {
            get
            {
                return this.presionado;
            }
            set
            {
                if (this.presionado != value)
                {
                    this.presionado = value;
                    this.animation[this.nombreDeAnimacion].speed = this.presionado ? this.velocidad : -this.velocidad;
                    if (!this.animation.isPlaying)
                    {
                        this.animation[this.nombreDeAnimacion].time = this.presionado ? 0 : this.animation[this.nombreDeAnimacion].length;
                        this.animation.Play(this.nombreDeAnimacion);
                    }
                }
            }
        }

        #endregion


        #region Métodos de la clase

        protected override void AlCambiarElEstadoDelInterruptor(EstadosDeInterruptores actual, EstadosDeInterruptores anterior)
        {
            this.Presionado = actual == EstadosDeInterruptores.Presionado;
        }

        #endregion
    }
}
