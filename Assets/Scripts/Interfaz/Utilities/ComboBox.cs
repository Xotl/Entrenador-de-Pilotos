using UnityEngine;
using System.Collections.Generic;

namespace Interfaz.Utilities
{
    [RequireComponent(typeof(CajaDeTexto))]
    public class ComboBox : ComboBoxBase
    {
        #region Campos públicos

        /// <summary>
        /// Caja de texto donde se puede escribir.
        /// </summary>
        private CajaDeTexto _cajaDeTexto;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene o establece el texto asociado al control. 
        /// </summary>
        /// <remarks>Al establecer la propiedad Texto en null o en una cadena vacía (""), se establece SelectedIndex en -1. Establecer la propiedad Texto en un valor que esté en la colección Items establece SelectedIndex en el índice de ese elemento. Al establecer la propiedad Text en un valor que no está en la colección, deja SelectedIndex sin modificar.</remarks>
        public string Texto
        {
            get
            {
                return this._cajaDeTexto.Texto;
            }
            set
            {
                if (value == null)
                    value = string.Empty;

                this._cajaDeTexto.Texto = value;
            }
        }

        #endregion


        #region Definición de eventos del control

        /// <summary>
        /// Se produce cuando ha cambiado la propiedad Texto.
        /// </summary>
        public event System.EventHandler AlCambiarTexto;

        private void eventoAlCambiarTexto(System.EventArgs e)
        {
            if (this.AlCambiarTexto != null)
                this.AlCambiarTexto(this, e);
        }

        #endregion


        #region Eventos de Unity

        protected override void Awake()
        {
            base.Awake();
            this._cajaDeTexto = this.GetComponent<CajaDeTexto>();
            this._cajaDeTexto.AlCambiarTexto += new System.EventHandler(this._cajaDeTexto_AlCambiarTexto);
        }

        #region Eventos dependientes del Awake

        private void _cajaDeTexto_AlCambiarTexto(object sender, System.EventArgs e)
        {
            string value = this._cajaDeTexto.Texto;

            if (value == string.Empty)
                this.SelectedIndex = -1;
            else
            {
                foreach (ComboBoxItem item in this.Items)
                {
                    if (item.Texto == value)
                    {
                        this.SelectedItem = item;
                        break;
                    }
                }
            }

            this.eventoAlCambiarTexto(e);
        }
        
        #endregion

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Cambia el texto en el objeto que representa el item seleccionado.
        /// </summary>
        /// <param name="texto">Nuevo texto que se quiere asignar.</param>
        protected override void CambiarTextoDelItemSeleccionado(string texto)
        {
            this.Texto = texto;
        }

        /// <summary>
        /// Obtiene el texto del objeto que representa el item seleccionado.
        /// </summary>
        protected override string ObtenerTextoDelItemSeleccionado()
        {
            return this.Texto;
        }

        #endregion
    }
}