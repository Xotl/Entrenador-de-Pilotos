using UnityEngine;
using System;
using System.Collections;

namespace Interfaz.Utilities
{
    [RequireComponent(typeof(Collider))]
    public class BotonController : MonoBehaviour
    {
        /// <summary>
        /// Se desencadena cuando se hace click en el botón.
        /// </summary>
        public event EventHandler Click;

        /// <summary>
        /// Se desencadena cuando el mouse entra al control.
        /// </summary>
        public event EventHandler MouseEnter;

        /// <summary>
        /// Se desencadena cuando el mouse sale del control.
        /// </summary>
        public event EventHandler MouseExit;

        /// <summary>
        /// Se desencadena cuando se presiona un botón del mouse mientras se está sobre el control.
        /// </summary>
        public event EventHandler MouseDown;

        /// <summary>
        /// Se desencadena cuando el usuario suelta el botón del mouse.
        /// </summary>
        public event EventHandler MouseUp;
    
        private void eventoClick(EventArgs e)
        {
            if (this.Click != null)
                this.Click(this, e);
        }

        private void OnMouseUpAsButton()
        {
            this.eventoClick(new EventArgs());
        }

        private void OnMouseEnter()
        {
            this.eventoMouseEnter(new EventArgs());
        }

        private void OnMouseExit()
        {
            this.eventoMouseExit(new EventArgs());
        }

        private void eventoMouseEnter(EventArgs e)
        {
            if (this.MouseEnter != null)
                this.MouseEnter(this, e);
        }

        private void eventoMouseExit(System.EventArgs e)
        {
            if (this.MouseExit != null)
                this.MouseExit(this, e);
        }

        private void OnMouseDown()
        {
            this.eventoMouseDown(new EventArgs());
        }

        private void OnMouseUp()
        {
            this.eventoMouseUp(new EventArgs());
        }

        private void eventoMouseDown(EventArgs e)
        {
            if (this.MouseDown != null)
                this.MouseDown(this, e);
        }

        private void eventoMouseUp(System.EventArgs e)
        {
            if (this.MouseUp != null)
                this.MouseUp(this, e);
        }

        protected virtual void Awake()
        {
            foreach (BotonController b in this.GetComponentsInChildren<BotonController>())
            {
                if (b.GetInstanceID() != this.GetInstanceID())
                {
                    b.Click += new EventHandler(this.b_Click);
                    b.MouseDown += new EventHandler(this.b_MouseDown);
                    b.MouseEnter += new EventHandler(this.b_MouseEnter);
                    b.MouseExit += new EventHandler(this.b_MouseExit);
                    b.MouseUp += new EventHandler(this.b_MouseUp);
                }
            }
        }

        private void b_MouseUp(object sender, EventArgs e)
        {
            this.eventoMouseUp(e);
        }

        private void b_MouseExit(object sender, EventArgs e)
        {
            this.eventoMouseExit(e);
        }

        private void b_MouseEnter(object sender, EventArgs e)
        {
            this.eventoMouseEnter(e);
        }

        private void b_MouseDown(object sender, EventArgs e)
        {
            this.eventoMouseDown(e);
        }

        private void b_Click(object sender, EventArgs e)
        {
            this.eventoClick(e);
        }
    }
}