using UnityEngine;
using System.Collections;
using Entrenamiento.Nucleo;

namespace Entrenamiento.GUI.Interruptores
{
    public class InterruptorColaDeRataController : InterruptorGuiController
    {
        #region Campos públicos

        #endregion


        #region Campos privados

        private const int GRADOS_DE_DIFERENCIA_DEL_CENTRO = 40;

        /// <summary>
        /// Velocidad a la que sucede la animación.
        /// </summary>
        [SerializeField]
        private float Velocidad = 0.2f;

        [SerializeField]
        private Transform ColaDerataTransform;

        private Quaternion _CentroPos;
        private Quaternion _ArribaPos;
        private Quaternion _AbajoPos;

        /// <summary>
        /// Rotación destino usada en la animación.
        /// </summary>
        private Quaternion _Destino;

        /// <summary>
        /// Rotación origen usada en la animación.
        /// </summary>
        private Quaternion _Origen;

        /// <summary>
        /// Progreso de la interpolación durante la animación. Va de 0 (origen) a 1 (destino).
        /// </summary>
        float ProporcionDeInterpolacion;

        #endregion


        #region Eventos Unity

        protected override void Awake()
        {
            base.Awake();

            if (this.ColaDerataTransform == null)
            {
                throw new System.NotImplementedException("No has asignado el objeto cola de rata que hay que rotar.");
            }

            this._CentroPos = this.ColaDerataTransform.localRotation;
            this._ArribaPos = this._CentroPos * Quaternion.AngleAxis(GRADOS_DE_DIFERENCIA_DEL_CENTRO, Vector3.right);
            this._AbajoPos = this._CentroPos * Quaternion.AngleAxis(-1 * GRADOS_DE_DIFERENCIA_DEL_CENTRO, Vector3.right);

            this.AjustarPosicionSinAnimacion(this.PosicionActual);
        }

        #endregion


        #region Métodos de la clase

        protected override void AlCambiarElEstadoDelInterruptor(EstadosDeInterruptores actual, EstadosDeInterruptores anterior)
        {
            switch (actual)
            {// Actualizamos la posición de destino para la animación
                case EstadosDeInterruptores.Arriba:
                    this._Destino = this._ArribaPos;
                    break;

                case EstadosDeInterruptores.Abajo:
                    this._Destino = this._AbajoPos;
                    break;

                default:// Centro
                    this._Destino = this._CentroPos;
                    break;
            }

            // Actualizamos la posición de origen para la animación
            this._Origen = this.ColaDerataTransform.localRotation;

            // Reiniciamos la proporción.
            this.ProporcionDeInterpolacion = 0;

            if (!this.AnimacionActiva)
                this.StartCoroutine(this.AnimacionDePosicion());
        }

        private void AjustarPosicionSinAnimacion(EstadosDeInterruptores posicion)
        {
            Quaternion destino;
            switch (posicion)
            {
                case EstadosDeInterruptores.Arriba:
                    destino = this._ArribaPos;
                    break;

                case EstadosDeInterruptores.Abajo:
                    destino = this._AbajoPos;
                    break;

                default:// Centro
                    destino = this._CentroPos;
                    break;
            }

            this.ColaDerataTransform.localRotation = destino;
        }

        private IEnumerator AnimacionDePosicion()
        {
            this._AnimacionActiva = true;
            while (this.ProporcionDeInterpolacion <= 1)
            {
                this.ColaDerataTransform.localRotation = Quaternion.Slerp(this._Origen, this._Destino, this.ProporcionDeInterpolacion);
                yield return null;
                this.ProporcionDeInterpolacion += Time.deltaTime / this.Velocidad;
            }
            this._AnimacionActiva = false;
        }

        #endregion
    }
}
