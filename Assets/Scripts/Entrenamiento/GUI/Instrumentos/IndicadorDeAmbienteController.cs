using System;
using System.Collections;
using Entrenamiento.Nucleo;
using UnityEngine;

namespace Entrenamiento.GUI.Instrumentos
{
    class IndicadorDeAmbienteController : InstrumentoGUIController
    {
        #region Campos privados

        private GUIText txtSombra;
        private Color colorOriginalTexto;
        private Color colorOriginalSombra;

        /// <summary>
        /// Tiempo en segundos que tarda entre cada transición. 
        /// Nota: 2 * TIEMPO_DE_PARPADEO será el tiempo en segundos que tarda en aparecer y desaparecer, es decir, el ciclo completo.
        /// </summary>
        private const float TIEMPO_DE_PARPADEO = 1.5f;

        #endregion

        #region Propiedades

        private bool parpadeoActivo = false;
        /// <summary>
        /// Obtiene un valor que indica si el mensaje de Ambiente se envuentra activo.
        /// </summary>
        public bool MensajeActivo
        {
            get
            {
                return this.parpadeoActivo;
            }
        }

        #endregion

        #region Eventos Unity

        private void Awake()
        {
            this.txtSombra = this.transform.Find("Sombra").guiText;
            this.AlCambiarValor += IndicadorDeAmbienteController_AlCambiarValor;

            this.colorOriginalSombra = this.txtSombra.color;
            this.colorOriginalTexto = this.guiText.color;

            this.MostrarMensaje(false);
        }

        private void Start()
        {
            this.transform.position = new Vector3(0.5f, 0.5f, 0f);
        }

        #endregion


        private void IndicadorDeAmbienteController_AlCambiarValor(object sender, EventArgs e)
        {
            this.ActualizarAmbiente((TiposDeAmbiente)Convert.ToInt32(this.Valor[0]));
        }

        /// <summary>
        /// Muestra un ambiente dado.
        /// </summary>
        /// <param name="ambiente">Ambiente que se desea representar.</param>
        private void ActualizarAmbiente(TiposDeAmbiente ambiente)
        {
            if (ambiente == TiposDeAmbiente.Normal)
            {
                this.MostrarMensaje(false);
                return;
            }

            this.MostrarMensaje(true);
            switch(ambiente) {
                case TiposDeAmbiente.Fuego_visible:
                    this.guiText.text = "Fuego visible";
                    break;

                case TiposDeAmbiente.Humo_visible:
                    this.guiText.text = "Humo visible";
                    break;
            }
            this.txtSombra.text = this.guiText.text;
        }

        /// <summary>
        /// Muestra u oculta el mensaje de ambiente.
        /// </summary>
        /// <param name="mostrar">TRUE para mostrar, de lo contrario FALSE.</param>
        public void MostrarMensaje(bool mostrar)
        {
            this.guiText.enabled = mostrar;
            this.txtSombra.enabled = mostrar;

            if (mostrar && !this.parpadeoActivo)
            {
                StartCoroutine(this.ParpadeoCoroutine());
            }
        }

        /// <summary>
        /// Co-rutina que hace que el mensaje de ambiente se mantenga parpadeando.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ParpadeoCoroutine()
        {
            bool desvanecer = false;
            Color auxTexto = this.colorOriginalTexto;
            Color auxSombra = this.colorOriginalSombra;

            auxTexto.a = 0;
            auxSombra.a = 0;
            this.guiText.color = auxTexto;
            this.txtSombra.color = auxSombra;
            this.parpadeoActivo = true;

            while (this.guiText.enabled)
            {
                if (desvanecer)
                {
                    auxTexto.a -= Time.deltaTime / TIEMPO_DE_PARPADEO;

                    if (auxTexto.a < 0)
                    {
                        auxTexto.a = 0;
                        desvanecer = false;
                    }
                }
                else
                {
                    auxTexto.a += Time.deltaTime / TIEMPO_DE_PARPADEO;

                    if (auxTexto.a > 1)
                    {
                        auxTexto.a = 1;
                        desvanecer = true;
                    }
                }

                auxSombra.a = auxTexto.a / 2;
                this.guiText.color = auxTexto;
                this.txtSombra.color = auxSombra;
                yield return null;
            }

            this.parpadeoActivo = false;
        }
    }
}
