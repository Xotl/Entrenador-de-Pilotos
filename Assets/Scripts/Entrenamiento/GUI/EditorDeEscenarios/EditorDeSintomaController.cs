using UnityEngine;
using System;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Interfaz.Utilities;

namespace Entrenamiento.GUI
{
    public class EditorDeSintomaController : MonoBehaviour
    {
        #region Campos

        /// <summary>
        /// Objeto que contiene el título del área.
        /// </summary>
        private TextMesh lblTitulo;

        /// <summary>
        /// Caja de texto del intervalo
        /// </summary>
        private CajaDeTexto txtIntervalo;

        /// <summary>
        /// Objeto que se usará de Prefab para generar las cajas de valores.
        /// </summary>
        private CajaDeTexto txtValor;

        /// <summary>
        /// Área donde se colocarán las cajas de texto para los valores.
        /// </summary>
        private Transform ValoresArea;

        /// <summary>
        /// ComboBox para la selección del tipo de función.
        /// </summary>
        private ComboBoxBase cmbTipoDeFuncion;

        private Dictionary<CajaDeTexto, int> _relCajasDeValores_Indice;

        #endregion


        #region Propiedades

        private TipoDeFuncionDeSintoma _TipoDeFuncion;
        /// <summary>
        /// Obtiene o establece el tipo de síntoma.
        /// </summary>
        public Entrenamiento.Nucleo.TipoDeFuncionDeSintoma TipoDeFuncion
        {
            get
            {
                return this.Sintoma.TipoDeFuncion;
            }
            set
            {
                this.Sintoma.TipoDeFuncion = value;
            }
        }

        private float _Intervalo;
        /// <summary>
        /// Obtiene o establece el intervalo.
        /// </summary>
        public float Intervalo
        {
            get
            {
                return this.Sintoma.Intervalo;
            }
            set
            {
                this.Sintoma.Intervalo = (int)value;
            }
        }

        private ValoresDeInstrumento _Valores;
        /// <summary>
        /// Obtiene o establece los valores del síntoma.
        /// </summary>
        public ValoresDeInstrumento Valores
        {
            get
            {
                return this.Sintoma.Valores;
            }
            set
            {
                this.Sintoma.Valores = value;
            }
        }

        private Entrenamiento.Nucleo.Sintoma _Sintoma;
        /// <summary>
        /// Obtiene o establece el síntoma que se edita.
        /// </summary>
        public Sintoma Sintoma
        {
            get
            {
                return this._Sintoma;
            }
            set
            {
                if (this._Sintoma != value)
                {
                    if (this._Sintoma != null)
                    {// Quitamos las referencias del síntoma anterior
                        this.Sintoma.AlCambiarIntervalo -= this.Sintoma_AlCambiarIntervalo;
                        this.Sintoma.AlCambiarTipoDeFuncion -= this.Sintoma_AlCambiarTipoDeFuncion;
                        this.Sintoma.AlCambiarValores -= this.Sintoma_AlCambiarValores;
                    }

                    this._Sintoma = value;

                    if (this._Sintoma != null)
                    {
                        this.lblTitulo.text = this._Sintoma.InstrumentoAfectado.ToString();
                        this.txtIntervalo.Texto = this._Sintoma.Intervalo.ToString();

                        if (this._Sintoma.Valores != null)
                        {
                            if (this.cmbTipoDeFuncion.ItemsCount == 0)
                                this.RellenarComboBoxDeFunciones();

                            this.cmbTipoDeFuncion.SelectedIndex = (int)this._Sintoma.TipoDeFuncion;
                            this.GenerarCajasDeTextoParaLosValores();
                            this.ActualizarValoresEnLasCajasDeTexto();
                        }

                        this.Sintoma.AlCambiarIntervalo += new EventHandler(this.Sintoma_AlCambiarIntervalo);
                        this.Sintoma.AlCambiarTipoDeFuncion += new EventHandler(this.Sintoma_AlCambiarTipoDeFuncion);
                        this.Sintoma.AlCambiarValores += new EventHandler(this.Sintoma_AlCambiarValores);
                    }
                }
            }
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Rellena el cmbTipoDeFuncion con las funciones existentes.
        /// </summary>
        private void RellenarComboBoxDeFunciones()
        {
            string[] tipos = Enum.GetNames(typeof(TipoDeFuncionDeSintoma));
            foreach (string tipo in tipos)
            {
                this.cmbTipoDeFuncion.AgregarElemento(tipo);
            }
        }

        /// <summary>
        /// Muestra los valores del síntoma en las cajas de texto.
        /// </summary>
        private void ActualizarValoresEnLasCajasDeTexto()
        {
            foreach (CajaDeTexto caja in this.ValoresArea.GetComponentsInChildren<CajaDeTexto>())
            {
                caja.Texto = this.Sintoma.Valores[this._relCajasDeValores_Indice[caja]].ToString();
            }
        }

        /// <summary>
        /// Agrega o quita cajas de texto según la cantidad de valores del síntoma.
        /// </summary>
        private void GenerarCajasDeTextoParaLosValores()
        {
            if (this.ValoresArea.childCount == this.Sintoma.Valores.Cantidad)
                return;
            
            if (this.ValoresArea.childCount < this.Sintoma.Valores.Cantidad)
            {// Hay que generar nuevas cajas de texto
                CajaDeTexto cajaAux;
                while (this.ValoresArea.childCount < this.Sintoma.Valores.Cantidad)
                {
                    cajaAux = Instantiate(this.txtValor) as CajaDeTexto;
                    this._relCajasDeValores_Indice.Add(cajaAux, this.ValoresArea.childCount);
                    cajaAux.transform.parent = this.ValoresArea;
                    cajaAux.transform.localPosition = Vector3.down * (this.ValoresArea.childCount - 1);
                    cajaAux.AlCambiarTexto += new EventHandler(this.txtValor_AlCambiarTexto);
                }
            }
            else
            {// Hay que borrar cajas de texto
                Transform cajaAux;
                while (this.ValoresArea.childCount > this.Sintoma.Valores.Cantidad)
                {
                    cajaAux = this.ValoresArea.GetChild(this.ValoresArea.childCount - 1);
                    cajaAux.parent = null;
                    Destroy(cajaAux.gameObject);
                }
            }
        }

        #endregion


        #region Definición de eventos del objeto

        /// <summary>
        /// Se desencadena cuando los valores, el intervalo o el tipo de función del sintoma ha sido editado.
        /// </summary>
        public event EventHandler<SintomaEditadoEventArgs> AlEditarSintoma;

        /// <summary>
        /// Se desencadena cuando la propiedad Valores cambia.
        /// </summary>
        public event EventHandler AlCambiarValores;

        /// <summary>
        /// Se desencadena cuando la propiedad Intervalo cambia.
        /// </summary>
        public event EventHandler AlCambiarIntervalo;

        /// <summary>
        /// Se desencadena cuando la propiedad TipoDeFuncion cambia.
        /// </summary>
        public event EventHandler AlCambiarTipoDeFuncion;

        private void eventoAlEditarSintoma(SintomaEditadoEventArgs e)
        {
            if (this.AlEditarSintoma != null)
                this.AlEditarSintoma(this, e);
        }

        private void eventoAlCambiarValores(System.EventArgs e)
        {
            if (this.AlCambiarValores != null)
                this.AlCambiarValores(this, e);
        }

        private void eventoAlCambiarIntervalo(System.EventArgs e)
        {
            if (this.AlCambiarIntervalo != null)
                this.AlCambiarIntervalo(this, e);
        }

        private void eventoAlCambiarTipoDeFuncion(EventArgs e)
        {
            if (this.AlCambiarTipoDeFuncion != null)
                this.AlCambiarTipoDeFuncion(this, e);
        }

        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            // Obtención de objetos
            this.lblTitulo = this.transform.Find("Nombre del instrumento").GetComponent<TextMesh>();
            this.txtIntervalo = this.transform.Find("IntervaloBox/txtIntervalo").GetComponent<CajaDeTexto>();
            this.ValoresArea = this.transform.Find("ValoresArea");
            this.txtValor = this.ValoresArea.Find("txtValor").GetComponent<CajaDeTexto>();
            this.cmbTipoDeFuncion = this.transform.Find("cmbTipoFuncion").GetComponent<ComboBoxBase>();

            // Inicialización
            this._relCajasDeValores_Indice = new Dictionary<CajaDeTexto, int>();
            this._relCajasDeValores_Indice.Add(this.txtValor, 0);
            

            // Eventos
            this.txtIntervalo.AlCambiarTexto += new System.EventHandler(txtIntervalo_AlCambiarTexto);
            this.AlCambiarIntervalo += new EventHandler(EditorDeSintomaController_AlCambiarIntervalo);
            this.txtValor.AlCambiarTexto += new EventHandler(txtValor_AlCambiarTexto);
            this.cmbTipoDeFuncion.SelectedIndexChange += new EventHandler(this.cmbTipoDeFuncion_SelectedIndexChange);
        }

        #endregion


        #region Eventos de los Síntomas

        private void Sintoma_AlCambiarValores(object sender, EventArgs e)
        {
            this.eventoAlCambiarValores(EventArgs.Empty);
            this.eventoAlEditarSintoma(new SintomaEditadoEventArgs(this.Sintoma));
        }

        private void Sintoma_AlCambiarTipoDeFuncion(object sender, EventArgs e)
        {
            this.eventoAlCambiarTipoDeFuncion(EventArgs.Empty);
            this.eventoAlEditarSintoma(new SintomaEditadoEventArgs(this.Sintoma));
        }

        private void Sintoma_AlCambiarIntervalo(object sender, EventArgs e)
        {
            this.eventoAlCambiarIntervalo(EventArgs.Empty);
            this.eventoAlEditarSintoma(new SintomaEditadoEventArgs(this.Sintoma));
        }

        #endregion


        #region Eventos de la interfaz

        private void cmbTipoDeFuncion_SelectedIndexChange(object sender, EventArgs e)
        {
            if (this.cmbTipoDeFuncion.SelectedIndex != -1)
            {
                this.Sintoma.TipoDeFuncion = (TipoDeFuncionDeSintoma)this.cmbTipoDeFuncion.SelectedIndex;
            }
        }

        private void EditorDeSintomaController_AlCambiarIntervalo(object sender, EventArgs e)
        {
            this.txtIntervalo.Texto = this.Intervalo.ToString();
        }

        private void txtIntervalo_AlCambiarTexto(object sender, System.EventArgs e)
        {
            float valor;

            if (float.TryParse(this.txtIntervalo.Texto, out valor))
            {
                this.Intervalo = valor;
            }
        }

        private void txtValor_AlCambiarTexto(object sender, EventArgs e)
        {
            CajaDeTexto textBox = (CajaDeTexto)sender;
            float valor;

            if (float.TryParse(textBox.Texto, out valor))
            {
                this.Sintoma.Valores[this._relCajasDeValores_Indice[(CajaDeTexto)sender]] = valor;
            }
        }

        #endregion


        public class SintomaEditadoEventArgs : EventArgs
        {
            private Sintoma sintoma;
            public Sintoma Sintoma
            {
                get
                {
                    return this.sintoma;
                }
            }

            public SintomaEditadoEventArgs(Sintoma sintoma)
            {
                this.sintoma = sintoma;
            }
        }
    }
}