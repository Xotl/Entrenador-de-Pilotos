using UnityEngine;
using Entrenamiento.Nucleo;
using Entrenamiento.GUI.Instrumentos;


namespace Entrenamiento.GUI
{
    [RequireComponent(typeof(InstrumentoGUIController))]
    public class SimuladorDeSintoma : MonoBehaviour
    {
        private InstrumentoGUIController InstrumentoGUI;

        /// <summary>
        /// Indica si ya fue aplicado el valor en un síntoma del tipo inmediato.
        /// </summary>
        private bool valorAplicado = false;

        /// <summary>
        /// Guarda la cantidad de unidades por segundo a sumarle a cada valor durante una función de interpolación.
        /// </summary>
        private ValoresDeInstrumento valoresInterpolacion;

        #region Propiedades

        private bool simulacionIniciada = false;
        /// <summary>
        /// Obtiene o establece un valor que indica si la simulación se está aplicando.
        /// </summary>
        public bool SimulacionIniciada
        {
            get
            {
                return this.simulacionIniciada;
            }
            set
            {
                if (this.simulacionIniciada != value)
                {
                    this.simulacionIniciada = value;

                    if (this.simulacionIniciada)// Limpio el estado para los síntomas del tipo inmediato.
                        this.valorAplicado = false;

                    if (this.simulacionIniciada && this.InstrumentoAfectado == null)
                    {// Si no hay un instrumento afectado la simulación no puede iniciar.
                        throw new System.InvalidOperationException("La simulación del síntoma no puede iniciar sin un instrumento afectado.");
                    }

                    if (this.SintomaASimular.TipoDeFuncion == TipoDeFuncionDeSintoma.Interpolacion)
                    {// Obtención de valores para la interpolación
                        this.valoresInterpolacion = (ValoresDeInstrumento)this.InstrumentoAfectado.Valores.Clone();
                        for (int i = 0; i < this.valoresInterpolacion.Cantidad; i++)
                        {
                            this.valoresInterpolacion[i] =
                                (this.SintomaASimular.Valores[i] - this.InstrumentoAfectado.Valores[i]) / this.SintomaASimular.Intervalo;
                        }
                    }

                    this.Simulacion();
                }
            }
        }


        /// <summary>
        /// Obtiene el instrumento lógico afectado por esta simulación.
        /// </summary>
        public Instrumento InstrumentoAfectado
        {
            get
            {
                return this.InstrumentoGUI.Instrumento;
            }
        }


        private Sintoma sintomaASimular;
        /// <summary>
        /// Obtiene o establece el síntoma que se desea simular.
        /// </summary>
        public Sintoma SintomaASimular
        {
            get
            {
                return this.sintomaASimular;
            }
            set
            {
                if (this.sintomaASimular != value)
                {
                    this.valorAplicado = false;
                    this.sintomaASimular = value;
                }
            }
        }

        #endregion


        #region Eventos Unity

        private void Awake()
        {
            this.InstrumentoGUI = this.GetComponent<InstrumentoGUIController>();
        }

        private void Update()
        {
            if (this.SimulacionIniciada)
            {
                this.Simulacion();
            }
        }

        #endregion


        #region Métodos de la clase

        /// <summary>
        /// Aplica la simulación en el instrumento afectado según el síntoma asignado.
        /// </summary>
        private void Simulacion()
        {
            ValoresDeInstrumento nuevosValores = (ValoresDeInstrumento)this.InstrumentoAfectado.Valores.Clone();

            switch (this.SintomaASimular.TipoDeFuncion)
            {
                case TipoDeFuncionDeSintoma.Constante:
                    for (int i = 0; i < nuevosValores.Cantidad; i++)
                    {// Constante
                        if (this.SintomaASimular.Intervalo != 0)
                        {
                            nuevosValores[i] +=
                                this.SintomaASimular.Valores[i] * (Time.deltaTime / this.SintomaASimular.Intervalo);
                        }
                    }

                    break;

                case TipoDeFuncionDeSintoma.Interpolacion:
                    for (int i = 0; i < nuevosValores.Cantidad; i++)
                    {// Interpolación
                        if (nuevosValores[i] != this.SintomaASimular.Valores[i])
                        {// Aún no llega a su destino

                            nuevosValores[i] += Time.deltaTime * this.valoresInterpolacion[i];

                            if (this.valoresInterpolacion[i] > 0)
                            {
                                if (nuevosValores[i] > this.SintomaASimular.Valores[i])
                                {// Si pasó el umbral positivo le asigno el valor límite
                                    nuevosValores[i] = this.SintomaASimular.Valores[i];
                                }
                            }
                            else if (nuevosValores[i] < this.SintomaASimular.Valores[i])
                            {// Si pasó el umbral negativo le asigno el valor límite
                                nuevosValores[i] = this.SintomaASimular.Valores[i];
                            }
                        }
                    }
                    break;

                default:// Inmediato
                    if (!this.valorAplicado)
                    {
                        for (int i = 0; i < nuevosValores.Cantidad; i++)
                            nuevosValores[i] = this.SintomaASimular.Valores[i];// Se aplica el valor directo
                        valorAplicado = true;
                    }
                    break;
            }

            this.InstrumentoAfectado.Valores = nuevosValores;
        }

        #endregion
    }
}