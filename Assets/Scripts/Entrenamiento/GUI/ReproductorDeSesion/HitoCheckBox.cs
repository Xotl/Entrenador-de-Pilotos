using UnityEngine;

namespace Entrenamiento.GUI.ReproductorDeSesion
{
    public class HitoCheckBox : MonoBehaviour
    {
        /// <summary>
        /// Obtiene o establece un valor que indica si este control está marcado (Checked).
        /// </summary>
        public bool Checked
        {
            get
            {
                return this.renderer.enabled;
            }
            set
            {
                if (this.renderer.enabled != value)
                {
                    this.renderer.enabled = value;
                    this.eventoAlCambiarChecked(System.EventArgs.Empty);
                }
            }
        }

        private void OnMouseUpAsButton()
        {
            this.Checked = !this.Checked;
        }


        public event System.EventHandler AlCambiarChecked;

        /// <summary>
        /// Aquí se mandan llamar todos los delegados asignados del evento AlCambiarChecked.
        /// </summary>
        private void eventoAlCambiarChecked(System.EventArgs e)
        {
            if (this.AlCambiarChecked != null)
                this.AlCambiarChecked(this, e);
        }
    }
}
