using UnityEngine;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Instrumentos
{
    public class InstrumentoGUIController : MonoBehaviour
    {
        #region Propiedades

        private Entrenamiento.Nucleo.Instrumento _Intrumento;
        /// <summary>
        /// Obtiene o establece el instrumento lógico de este intrumento gráfico.
        /// </summary>
        public Instrumento Instrumento
        {
            get
            {
                return this._Intrumento;
            }
            set
            {
                if (this._Intrumento != value)
                {
                    if (this._Intrumento != null)
                        this._Intrumento.AlCambiarValor -= this._Intrumento_AlCambiarValor;// Deja de estar a la escucha.

                    this._Intrumento = value;

                    if (this._Intrumento != null)
                    {
                        this._Intrumento.AlCambiarValor += new System.EventHandler(this._Intrumento_AlCambiarValor);
                        this.eventoAlCambiarValor(System.EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el valor actual del indicador.
        /// </summary>
        public ValoresDeInstrumento Valor
        {
            get
            {
                return this.Instrumento.Valores;
            }
            set
            {
                this.Instrumento.Valores = value;
            }
        }

        [SerializeField]
        private TiposDeIntrumentos tipoDelInstrumento = TiposDeIntrumentos.Desconocido;
        /// <summary>
        /// Obtiene el tipo del instrumento.
        /// </summary>
        public TiposDeIntrumentos TipoDelInstrumento
        {
            get
            {
                if (this.Instrumento != null)
                    return this.Instrumento.Tipo;
                else
                    return this.tipoDelInstrumento;
            }
        }


        [SerializeField]
        private NombresDeInstrumentos nombreDelInstrumento = NombresDeInstrumentos.Desconocido;
        /// <summary>
        /// Obtiene el nombre del instrumento.
        /// </summary>
        public NombresDeInstrumentos NombreDelInstrumento
        {
            get
            {
                if (this.Instrumento != null)
                    return this.Instrumento.Nombre;
                else
                    return this.nombreDelInstrumento;
            }
        }

        #endregion


        #region Definición de eventos de la clase

        /// <summary>
        /// Se desencadena cuando la propiedad Valor cambia.
        /// </summary>
        public event System.EventHandler AlCambiarValor;

        private void eventoAlCambiarValor(System.EventArgs e)
        {
            if (this.AlCambiarValor != null)
                this.AlCambiarValor(this, e);
        }

        #endregion


        #region Eventos de Unity

        #endregion


        private void _Intrumento_AlCambiarValor(object sender, System.EventArgs e)
        {
            this.eventoAlCambiarValor(e);
        }
    }
}
