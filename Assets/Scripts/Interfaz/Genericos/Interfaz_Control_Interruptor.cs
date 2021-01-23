using UnityEngine;
using Entrenamiento.GUI.Interruptores;
using Interfaz.Genericos;
using Entrenamiento.Nucleo;

namespace Interfaz.Genericos
{
    [RequireComponent(typeof(InterruptorGuiController))]
    public class Interfaz_Control_Interruptor : MonoBehaviour
    {
        /// <summary>
        /// Componente InterruptorGuiController de este objeto.
        /// </summary>
        private InterruptorGuiController interruptorGuiController;
        

        #region Control a través de un Menú Radial Desplegable

        private MenuRadialDesplegable menuRadialDesplegable;

        private void MenuRadialAwake()
        {
            this.menuRadialDesplegable = this.GetComponent<MenuRadialDesplegable>();
        }

        private void MenuRadialStart()
        {
            if (this.menuRadialDesplegable == null)
                return;

            object[] nuevosItems = new object[this.interruptorGuiController.Interruptor.EstadosPermitidos.Length];
            for (int i = 0; i < nuevosItems.Length; i++)
            {
                nuevosItems[i] = this.interruptorGuiController.Interruptor.EstadosPermitidos[i];
            }
                
            this.menuRadialDesplegable.Items = nuevosItems;
            this.menuRadialDesplegable.AlSeleccionarItem += menuRadialDesplegable_AlSeleccionarItem;
        }

        private void menuRadialDesplegable_AlSeleccionarItem(object sender, MenuRadialDesplegable.MenuRadialItemEventArgs e)
        {
            this.interruptorGuiController.PosicionActual = (EstadosDeInterruptores)e.Item.Valor;
        }

        #endregion


        #region Eventos Unity

        private void Awake()
        {
            this.interruptorGuiController = this.GetComponent<InterruptorGuiController>();
            this.MenuRadialAwake();
        }

        private void Start()
        {
            this.MenuRadialStart();
        }

        #endregion
    }
}
