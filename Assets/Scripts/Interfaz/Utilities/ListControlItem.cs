using UnityEngine;
using System.Collections;

namespace Interfaz.Utilities
{
    public class ListControlItem : ItemController
    {
        #region Propiedades

        private object valor;
        /// <summary>
        /// Obtiene o establece el valor asociado a este Item.
        /// </summary>
        new public object Valor
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
                    this.Texto = this.valor.ToString();
                    this.eventoAlCambiarValor(System.EventArgs.Empty);
                }
            }

        }

        private bool _Seleccionado = false;
        /// <summary>
        /// Obtiene o establece un valor indicando si este Item se encuentra seleccionado.
        /// </summary>
        public bool Seleccionado
        {
            get
            {
                return this._Seleccionado;
            }
            set
            {
                if (this._Seleccionado != value)
                {
                    this._Seleccionado = value;
                    this.MostrarFondo = this._Seleccionado;
                    this.eventoAlCambiarSeleccion(System.EventArgs.Empty);
                }
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando la propiedad Seleccionado cambia de valor.
        /// </summary>
        public event System.EventHandler AlCambiarSeleccion;

        /// <summary>
        /// Se produce cuando la propiedad Valor cambia.
        /// </summary>
        new public event System.EventHandler AlCambiarValor;

        private void eventoAlCambiarSeleccion(System.EventArgs e)
        {
            if (this.AlCambiarSeleccion != null)
                this.AlCambiarSeleccion(this, e);
        }

        private void eventoAlCambiarValor(System.EventArgs e)
        {
            if (this.AlCambiarValor != null)
                this.AlCambiarValor(this, e);
        }

        #endregion


        #region Eventos de Unity

        protected override void Awake()
        {
            base.Awake();
            this.Click += ListControlItem_Click;
            this.MouseEnter += ListControlItem_MouseEnter;
            this.MouseExit += ListControlItem_MouseExit;
        }

        private void ListControlItem_MouseExit(object sender, System.EventArgs e)
        {
            this.MostrarFondo = this.Seleccionado;
        }

        private void ListControlItem_MouseEnter(object sender, System.EventArgs e)
        {
            this.MostrarFondo = true;
        }

        private void ListControlItem_Click(object sender, System.EventArgs e)
        {
            this.Seleccionado = !this.Seleccionado;
        }

        #endregion
    }
}