using UnityEngine;
using System.Collections;

namespace Interfaz.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class AutoAjusteDeCamara : MonoBehaviour
    {
        /// <summary>
        /// Los límites de la interfaz utilizados para el ajuste de la cámara.
        /// </summary>
        public LimitesVisuales Limites;

        private void Start()
        {
            this.EstablecerDimensiones();
            this.CentrarCamaraEnLimites();
        }

        /// <summary>
        /// Establece las dimensiones mínimas para que se vean todos los límites.
        /// </summary>
        private void EstablecerDimensiones()
        {
            float anchoLimites = this.Limites.RightPos - this.Limites.LeftPos;
            float altoLimites = this.Limites.TopPos - this.Limites.BottomPos;

            if ((altoLimites * this.camera.aspect) < anchoLimites)
                this.camera.orthographicSize = anchoLimites / (2 * this.camera.aspect);
            else
                this.camera.orthographicSize = altoLimites / 2;
        }

        /// <summary>
        /// Centra la cámara dentro de los límites.
        /// </summary>
        private void CentrarCamaraEnLimites()
        {
            Vector3 nuevaPosicion =
                new Vector3(
                    this.Limites.LeftPos + ((this.Limites.RightPos - this.Limites.LeftPos) / 2),
                    this.Limites.TopPos + ((this.Limites.BottomPos - this.Limites.TopPos) / 2),
                    this.camera.transform.position.z
                );
            this.camera.transform.position = nuevaPosicion;
        }

        /// <summary>
        /// Coloca la cámara al ras del límite Top y centrada horizontalmente.
        /// </summary>
        private void ColocarEnPosicionInicial()
        {
            Vector3 nuevaPosicion =
                new Vector3(
                    this.Limites.LeftPos + ((this.Limites.RightPos - this.Limites.LeftPos) / 2),
                    this.Limites.TopPos + -1 * this.camera.orthographicSize,
                    this.camera.transform.position.z
                );
            this.camera.transform.position = nuevaPosicion;
        }
    }
}