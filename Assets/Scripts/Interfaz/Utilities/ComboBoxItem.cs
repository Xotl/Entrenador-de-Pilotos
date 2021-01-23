using UnityEngine;

namespace Interfaz.Utilities
{
    public class ComboBoxItem : ItemController
    {
        protected override void Awake()
        {
            base.Awake();
            this.Click += new System.EventHandler(ComboBoxItem_Click);
            this.MouseEnter += ComboBoxItem_MouseEnter;
            this.MouseExit += ComboBoxItem_MouseExit;
        }

        private void ComboBoxItem_MouseExit(object sender, System.EventArgs e)
        {
            this.MostrarFondo = false;
        }

        private void ComboBoxItem_MouseEnter(object sender, System.EventArgs e)
        {
            this.MostrarFondo = true;
        }

        private void ComboBoxItem_Click(object sender, System.EventArgs e)
        {
            this.MostrarFondo = false;
        }
    }
}