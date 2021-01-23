using UnityEngine;
using System.Collections.Generic;
using Entrenamiento.Nucleo;
using Interfaz.Utilities;

namespace Entrenamiento.GUI.Instrumentos
{
    public class PanelDeInstrumentosController : MonoBehaviour
    {
        #region Campos públicos


        #endregion


        #region Campos privados

        /// <summary>
        /// Combo box usado para seleccionar el ambiente
        /// </summary>
        private ComboBoxBase cmbAmbiente;

        /// <summary>
        /// Delgado usado en la vinculación de los síntomas.
        /// </summary>
        private System.Func<Sintoma[]> getSintomas;

        /// <summary>
        /// Delgado usado en la vinculación de los síntomas.
        /// </summary>
        private System.Action<Sintoma[]> setSintomas;
        
        private Dictionary<NombresDeInstrumentos, Entrenamiento.Nucleo.Sintoma> _relIntrumentosSintomas;

        /// <summary>
        /// Objeto que contiene la representación visual de todos los instrumentos
        /// </summary>
        private Transform PanelDeInstrumentos;

        /// <summary>
        /// Objeto que contiene el área de edición de síntomas.
        /// </summary>
        private EditorDeSintomaController EditorDeSintoma;

        /// <summary>
        /// Mensaje que se muestra cuando no se ha seleccionado un instrumento.
        /// </summary>
        private GameObject MensajeDeNoSeleccion;

        /// <summary>
        /// Objeto que representa las luces del panel.
        /// </summary>
        private Transform LucecitasDelPanel;

        #endregion


        #region Propiedades

        /// <summary>
        /// Obtiene los síntomas en edición.
        /// </summary>
        public Sintoma[] Sintomas
        {
            get
            {
                Sintoma[]  aux = new Sintoma[this._relIntrumentosSintomas.Count];
                int cantidad = 0;
                foreach (Sintoma s in this._relIntrumentosSintomas.Values)
                {
                    if (s != null)
                    {
                        aux[cantidad++] = s;
                    }
                }

                Sintoma[] sintomas = new Sintoma[cantidad];
                while (cantidad-- > 0)
                {
                    sintomas[cantidad] = aux[cantidad];
                }

                return sintomas;
            }
            set
            {
                this.ResetDelPanel();// Regresa el panel
                foreach (Sintoma s in value)
                {// Asignamos los nuevos síntomas
                    this._relIntrumentosSintomas[s.InstrumentoAfectado] = s;
                }

                Sintoma sintoma;
                foreach (IndicadorLuminosoController luz in this.LucecitasDelPanel.GetComponentsInChildren<IndicadorLuminosoController>())
                {
                    luz.LuzEncendida = false;
                    sintoma = this._relIntrumentosSintomas[luz.NombreDelInstrumento];
                    if (sintoma != null && sintoma.Valores[0] != 0)
                    {
                        luz.LuzEncendida = true;
                    }
                }

                // Actualizamos el Ambiente
                sintoma = this._relIntrumentosSintomas[NombresDeInstrumentos.Ambiente];
                if (sintoma == null)
                {
                    this.CambiarSintomaAmbiente(TiposDeAmbiente.Normal);
                    sintoma = this._relIntrumentosSintomas[NombresDeInstrumentos.Ambiente];
                }
                if (this.cmbAmbiente.ItemsCount == 0)// Si el "Start" no ha sucedido en este momento entonces inicializamos.
                    this.RellenarComboBoxAmbiente();
                this.cmbAmbiente.SelectedIndex = System.Convert.ToInt32(sintoma.Valores[0]);
            }
        }

        #endregion


        #region Eventos de Unity

        private void Awake()
        {
            // Obtención de objetos
            this.PanelDeInstrumentos = this.transform.Find("Panel con instrumentos");
            this.LucecitasDelPanel = this.transform.Find("Panel con instrumentos/Lucecitas");
            this.EditorDeSintoma = this.transform.Find("EditorDeSintoma").GetComponent<EditorDeSintomaController>();
            this.MensajeDeNoSeleccion = this.transform.Find("Mensaje de no selección").gameObject;
            this.cmbAmbiente = this.transform.Find("cmbAmbiente").GetComponent<ComboBoxBase>();


            // Inicialización
            this._relIntrumentosSintomas = this.InicializarDiccionarioDeInstrumentos();
            this.InicializarEventosDelPanel();
            this.EditorDeSintoma.AlEditarSintoma += this.EditorDeSintoma_AlEditarSintoma;
            this.cmbAmbiente.SelectedIndexChange += this.cmbAmbiente_SelectedIndexChange;
        }

        private void Start()
        {
            if (this.cmbAmbiente.ItemsCount == 0)// Si en este momento no ha sido inicializado.
                this.RellenarComboBoxAmbiente();
        }

        #endregion


        #region Eventos de interfaz

        private void cmbAmbiente_SelectedIndexChange(object sender, System.EventArgs e)
        {
            this.CambiarSintomaAmbiente((TiposDeAmbiente)this.cmbAmbiente.SelectedIndex);
        }

        private void EditorDeSintoma_AlEditarSintoma(object sender, EditorDeSintomaController.SintomaEditadoEventArgs e)
        {
            Sintoma s = e.Sintoma;
            if (s.Valores == Instrumentacion.ObtenerValoresVaciosDeInstrumento(s.InstrumentoAfectado) || s.Intervalo == 0)
            {
                s = null;
            }

            this._relIntrumentosSintomas[e.Sintoma.InstrumentoAfectado] = s;
            this.eventoAlEditarSintoma(e);
        }

        private void Lucecita_Click(object sender, System.EventArgs e)
        {
            BotonInstrumentoDePanel btnInstrumento = (BotonInstrumentoDePanel)sender;

            btnInstrumento.GetComponent<IndicadorLuminosoController>().LuzEncendida =
                this.CambiarSintomaDeIndicadorLuminoso((btnInstrumento).NombreDelInstrumento);
        }

        private void Instrumento_Click(object sender, System.EventArgs e)
        {
            this.ActivarEdicionDeSintoma(((BotonInstrumentoDePanel)sender).NombreDelInstrumento);
        }

        #endregion


        #region Definición de eventos del objeto

        /// <summary>
        /// Se desencadena cuando los valores, el intervalo o el tipo de función de un síntoma han sido editados en el editor.
        /// </summary>
        public event System.EventHandler<EditorDeSintomaController.SintomaEditadoEventArgs> AlEditarSintoma;

        private void eventoAlEditarSintoma(EditorDeSintomaController.SintomaEditadoEventArgs e)
        {
            if (this.AlEditarSintoma != null)
                this.AlEditarSintoma(this, e);
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Limpia los síntomas del diccionario de instrumentos y algunos mensajes y valores del panel. Nota: No apaga las luces.
        /// </summary>
        private void ResetDelPanel()
        {
            NombresDeInstrumentos[] Keys = new NombresDeInstrumentos[this._relIntrumentosSintomas.Keys.Count];
            this._relIntrumentosSintomas.Keys.CopyTo(Keys, 0);
            foreach (NombresDeInstrumentos key in Keys)
            {// Limpiamos los valores anteriores
                this._relIntrumentosSintomas[key] = null;
            }

            this.EditorDeSintoma.gameObject.SetActive(false);
            this.MensajeDeNoSeleccion.SetActive(true);
        }

        private void RellenarComboBoxAmbiente()
        {
            foreach (TiposDeAmbiente amb in (TiposDeAmbiente[])System.Enum.GetValues(typeof(TiposDeAmbiente)))
            {
                this.cmbAmbiente.AgregarElemento(amb);
            }
        }

        private void InicializarEventosDelPanel()
        {
            foreach (Transform o in this.PanelDeInstrumentos)
            {// Eventos de los instrumentos
                BotonInstrumentoDePanel btn = o.GetComponent<BotonInstrumentoDePanel>();
                if (btn != null)
                {
                    btn.Click += new System.EventHandler(this.Instrumento_Click);
                }
            }

            foreach (Transform o in this.LucecitasDelPanel)
            {// Eventos de las lucecitas
                BotonInstrumentoDePanel btn = o.GetComponent<BotonInstrumentoDePanel>();
                if (btn != null)
                {
                    btn.Click += new System.EventHandler(this.Lucecita_Click);
                }
            }
        }

        /// <summary>
        /// Cambia el valor de un indicador luminoso dado.
        /// </summary>
        /// <param name="nombreDelInstrumento">Nombre del indicador luminoso que se desea cambiar de valor.</param>
        private bool CambiarSintomaDeIndicadorLuminoso(NombresDeInstrumentos nombreDelInstrumento)
        {
            Sintoma sintoma = this._relIntrumentosSintomas[nombreDelInstrumento];
            if (sintoma == null)
            {
                sintoma = new Sintoma(nombreDelInstrumento);
                this._relIntrumentosSintomas[nombreDelInstrumento] = sintoma;
            }

            sintoma.TipoDeFuncion = TipoDeFuncionDeSintoma.Inmediato;

            if (sintoma.Valores[0] == 0)
                sintoma.Valores[0] = 1;
            else
                sintoma.Valores[0] = 0;


            this.eventoAlEditarSintoma(new EditorDeSintomaController.SintomaEditadoEventArgs(sintoma));
            return System.Convert.ToBoolean(sintoma.Valores[0]);
        }

        /// <summary>
        /// Cambia el síntoma de ambiente.
        /// </summary>
        /// <param name="ambiente">Ambiente deseado.</param>
        private void CambiarSintomaAmbiente(TiposDeAmbiente ambiente)
        {
            Sintoma sintoma = this._relIntrumentosSintomas[NombresDeInstrumentos.Ambiente];
            if (sintoma == null)
            {
                sintoma = new Sintoma(NombresDeInstrumentos.Ambiente);
                this._relIntrumentosSintomas[NombresDeInstrumentos.Ambiente] = sintoma;
            }

            sintoma.TipoDeFuncion = TipoDeFuncionDeSintoma.Inmediato;
            sintoma.Valores[0] = System.Convert.ToSingle(ambiente);
            this.eventoAlEditarSintoma(new EditorDeSintomaController.SintomaEditadoEventArgs(sintoma));
        }

        /// <summary>
        /// Muestra los valores de un instrumento dado para ser editado en EditorDeSintoma.
        /// </summary>
        /// <param name="nombreDelInstrumento">Nombre del instrumento.</param>
        private void ActivarEdicionDeSintoma(NombresDeInstrumentos nombreDelInstrumento)
        {
            Sintoma sintoma = this._relIntrumentosSintomas[nombreDelInstrumento];
            if (sintoma == null)
            {
                sintoma = new Sintoma(nombreDelInstrumento);
                this._relIntrumentosSintomas[nombreDelInstrumento] = sintoma;
            }
            
            this.EditorDeSintoma.gameObject.SetActive(true);
            this.MensajeDeNoSeleccion.SetActive(false);
            this.EditorDeSintoma.Sintoma = sintoma;
        }

        /// <summary>
        /// Obtiene todos los instrumentos del panel y los indexa en un diccionario.
        /// </summary>
        private Dictionary<NombresDeInstrumentos, Sintoma> InicializarDiccionarioDeInstrumentos()
        {
            Dictionary<NombresDeInstrumentos, Entrenamiento.Nucleo.Sintoma> d = new Dictionary<NombresDeInstrumentos, Sintoma>();
            foreach (BotonInstrumentoDePanel instrumento in this.GetComponentsInChildren<BotonInstrumentoDePanel>())
            {
                d.Add(instrumento.NombreDelInstrumento, null);
            }

            // Se agrega el "Instrumento" de Ambiente
            d.Add(NombresDeInstrumentos.Ambiente, null);

            return d;
        }

        #endregion
    }
}