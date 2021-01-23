using UnityEngine;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Interfaz.Utilities;

namespace Entrenamiento.GUI.Interruptores
{
    public class PanelDeInterruptoresController : MonoBehaviour
    {
        #region Campos privados

        private BotonController btnQuitar;
        private ListControl ListaInterruptores;
        private CheckBox chkSolucionOrdenada;
        private Transform contenedorDeInterruptores;
        private Dictionary<NombresDeInterruptores, InterruptorGuiController> _InterruptoresDelPanel;

        /// <summary>
        /// Valor que indica si se está inicializando el panel, es decir, que se acaba de asignar una nueva solución.
        /// </summary>
        private bool _inicializando = false;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene todos los interruptores lógicos de este panel.
        /// </summary>
        public Interruptor[] InterruptoresDelPanel
        {
            get
            {
                Interruptor[] interruptores = new Interruptor[this._InterruptoresDelPanel.Count];
                int u = 0;
                foreach (InterruptorGuiController i in this._InterruptoresDelPanel.Values)
                {
                    interruptores[u++] = i.Interruptor;
                }
                return interruptores;
            }
        }

        private Solucion _Solucion;
        /// <summary>
        /// Obtiene o establece la solución que se muestra.
        /// </summary>
        public Solucion Solucion
        {
            get
            {
                return this._Solucion;
            }
            set
            {
                if (this._Solucion != value)
                {
                    this._inicializando = true;

                    if (this._Solucion != null)
                    {// Quitamos los eventos
                        this._Solucion.AlAgregarEstadoDeInterruptor -= this._Solucion_AlAgregarEstadoDeInterruptor;
                        this._Solucion.AlQuitarEstadoDeInterruptor -= this._Solucion_AlQuitarEstadoDeInterruptor; 
                    }

                    this.ListaInterruptores.Clear();
                    this._Solucion = value;
                    
                    if (this._Solucion != null)
                    {// Inicializamos y asignamos eventos
                        this.chkSolucionOrdenada.Checked = this._Solucion.ElOrdenImporta;
                        foreach (ParDeDatosInterruptorEstado par in this._Solucion.EstadoDeInterruptoresDeseado)
                        {
                            this._InterruptoresDelPanel[par.Interruptor].PosicionActual = par.Estado;
                            this.ListaInterruptores.AgregarItem(par);
                        }

                        // Eventos
                        this._Solucion.AlAgregarEstadoDeInterruptor += this._Solucion_AlAgregarEstadoDeInterruptor;
                        this._Solucion.AlQuitarEstadoDeInterruptor += this._Solucion_AlQuitarEstadoDeInterruptor;
                    }

                    this._inicializando = false;
                }
            }
        }
        
        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se produce cuando la solución ha sufrido algún tipo de modificación.
        /// </summary>
        public event System.EventHandler AlModificarSolucion;

        private void eventoAlModificarSolucion(System.EventArgs e)
        {
            if (this.AlModificarSolucion != null)
                this.AlModificarSolucion(this, e);
        }

        #endregion


        #region Eventos Unity

        private void Awake()
        {
            // Obtención de objetos
            this.btnQuitar = this.transform.Find("List/btnQuitar").GetComponent<BotonController>();
            this.ListaInterruptores = this.transform.Find("List").GetComponent<ListControl>();
            this.chkSolucionOrdenada = this.transform.Find("chkSolucionOrdenada").GetComponent<CheckBox>();
            this.contenedorDeInterruptores = this.transform.Find("Fondo Panel de interruptores");


            // Inicialización
            this.btnQuitar.Click += this.btnQuitar_Click;
            this.chkSolucionOrdenada.OnCheckedChange += this.chkSolucionOrdenada_OnCheckedChange;
            this._InterruptoresDelPanel = new Dictionary<NombresDeInterruptores, InterruptorGuiController>();
        }

        private void Start()
        {
            foreach (InterruptorGuiController i in this.contenedorDeInterruptores.GetComponentsInChildren<InterruptorGuiController>())
            {
                i.Interruptor.AlCambiarSuEstado += this.Interruptor_AlCambiarSuEstado;
                this._InterruptoresDelPanel.Add(i.Interruptor.Nombre, i);
            }
        }

        #endregion


        #region Eventos de la Solución

        private void _Solucion_AlQuitarEstadoDeInterruptor(object sender, ParEstadoInterruptorEventArgs e)
        {
            foreach (ListControlItem item in this.ListaInterruptores.Items)
            {
                if (item.Valor.Equals(e.ParDeDatosInterruptorEstado))
                {
                    this.ListaInterruptores.QuitarItem(item);
                    return;
                }
            }
        }

        private void _Solucion_AlAgregarEstadoDeInterruptor(object sender, ParEstadoInterruptorEventArgs e)
        {
            this.ListaInterruptores.AgregarItem(e.ParDeDatosInterruptorEstado);
        }

        #endregion


        #region Eventos de la interfaz

        private void Interruptor_AlCambiarSuEstado(object sender, System.EventArgs e)
        {
            if (!this._inicializando && this.Solucion != null)
            {
                Interruptor interruptor = (Interruptor)sender;
                ParDeDatosInterruptorEstado nuevo = new ParDeDatosInterruptorEstado();
                nuevo.Interruptor = interruptor.Nombre;
                nuevo.Estado = interruptor.EstadoActual;
                this.Solucion.AgregarEstadoDeInterruptoresDeseado(nuevo);
                this.eventoAlModificarSolucion(e);
            }
        }

        private void btnQuitar_Click(object sender, System.EventArgs e)
        {
            ListControlItem[] itemsSeleccionados = this.ListaInterruptores.SelectedItems;
            for (int i = itemsSeleccionados.Length - 1; i >= 0; i--)
            {
                this.Solucion.QuitarEstadoDeInterruptoresDeseado((ParDeDatosInterruptorEstado)itemsSeleccionados[i].Valor);
            }
            this.eventoAlModificarSolucion(e);
        }

        private void chkSolucionOrdenada_OnCheckedChange(object sender, System.EventArgs e)
        {
            this.Solucion.ElOrdenImporta = this.chkSolucionOrdenada.Checked;
            this.eventoAlModificarSolucion(e);
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Similar al SetActive de GameObject. Activa o desactiva este objeto, eso incluye a sus hijos y objetos dependientes.
        /// </summary>
        /// <param name="value">TRUE para activar, de lo contrario FALSE.</param>
        public void SetActive(bool value)
        {
            if (this.ListaInterruptores != null)
                this.ListaInterruptores.SetActive(value);
            this.gameObject.SetActive(value);
        }

        #endregion
    }
}