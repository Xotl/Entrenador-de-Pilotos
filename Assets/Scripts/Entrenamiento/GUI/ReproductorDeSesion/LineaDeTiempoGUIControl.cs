using UnityEngine;
using System.Collections;
using System;

namespace Entrenamiento.GUI.ReproductorDeSesion
{
    public class LineaDeTiempoGUIControl : MonoBehaviour
    {
        private bool arrastreIniciado = false;
        /// <summary>
        /// Obtiene un valor que indica si se está arrastrando el control.
        /// </summary>
        public bool Arrastrando
        {
            get
            {
                return this.arrastreIniciado;
            }
        }


        #region Definición de eventos

        /// <summary>
        /// Se produce cuando se arrastra este control.
        /// </summary>
        public event System.EventHandler DuranteElDrag;

        /// <summary>
        /// Se produce cuando finaliza el arrastre del control.
        /// </summary>
        public event System.EventHandler AlTerminarDrag;

        /// <summary>
        /// Se produce al inicio del arrastre de este control.
        /// </summary>
        public event System.EventHandler AlIniciarDrag;



        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento DuranteElDrag.
        /// </summary>
        private void eventoDuranteElDrag(System.EventArgs e)
        {
            if (this.DuranteElDrag != null)
                this.DuranteElDrag(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlTerminarDrag.
        /// </summary>
        private void eventoAlTerminarDrag(System.EventArgs e)
        {
            if (this.AlTerminarDrag != null)
                this.AlTerminarDrag(this, e);
        }

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlIniciarDrag.
        /// </summary>
        private void eventoAlIniciarDrag(System.EventArgs e)
        {
            if (this.AlIniciarDrag != null)
                this.AlIniciarDrag(this, e);
        }

        #endregion


        #region Eventos Unity

        private void OnMouseDrag()
        {
            if (!this.arrastreIniciado)
            {
                this.arrastreIniciado = true;
                this.eventoAlIniciarDrag(EventArgs.Empty);
            }

            this.eventoDuranteElDrag(EventArgs.Empty);
        }

        private void OnMouseUp()
        {
            if (this.arrastreIniciado)
            {
                this.arrastreIniciado = false;
                this.eventoAlTerminarDrag(EventArgs.Empty);
            }
        }

        #endregion
    }
}