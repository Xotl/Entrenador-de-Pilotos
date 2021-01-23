using UnityEngine;
using System.Collections;

namespace Interfaz.Utilities
{
    public class LimitesVisuales : MonoBehaviour
    {
        /// <summary>
        /// Obtiene la posición en Y de Top.
        /// </summary>
        public float TopPos
        {
            get
            {
                return this._TopObj.position.y;
            }
        }

        /// <summary>
        /// Obtiene la posición en X de Right.
        /// </summary>
        public float RightPos
        {
            get
            {
                return this._RightObj.position.x;
            }
        }

        /// <summary>
        /// Obtiene la posición en Y de Bottom.
        /// </summary>
        public float BottomPos
        {
            get
            {
                return this._BottomObj.position.y;
            }
        }

        /// <summary>
        /// Obtiene la posición en X de Left.
        /// </summary>
        public float LeftPos
        {
            get
            {
                return this._LeftObj.position.x;
            }
        }

        /// <summary>
        /// Obtiene el ancho en unidades Unity.
        /// </summary>
        public float Ancho
        {
            get
            {
                return this.RightPos - this.LeftPos;
            }
        }

        /// <summary>
        /// Obtiene el alto en unidades Unity.
        /// </summary>
        public float Alto
        {
            get
            {
                return this.TopPos - this.BottomPos;
            }
        }

        private void Awake()
        {
            int conteo = 0;
            foreach (Transform t in this.transform)
            {
                conteo++;
                switch (t.name)
                {
                    case "Top":
                        this._TopObj = t;
                        break;

                    case "Right":
                        this._RightObj = t;
                        break;

                    case "Bottom":
                        this._BottomObj = t;
                        break;

                    case "Left":
                        this._LeftObj = t;
                        break;

                    default:
                        conteo--;
                        // Debug.Log(t.name);
                        break;
                }
            }

            if (conteo < 4)
            {
                throw new NoHayElementosException("Se encontraron" + conteo + " de 4 objetos esperados.");
            }
        }

        private void OnDrawGizmos()
        {
            Bounds b;
            Transform[] objetos = this.GetComponentsInChildren<Transform>();

            if (objetos.Length > 0)
            {
                b = new Bounds(objetos[0].position, Vector3.zero);
                for (int i = 1; i < objetos.Length; i++)
                {
                    b.Encapsulate(objetos[i].position);
                }

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(b.center, b.size);
            }
        }

        private UnityEngine.Transform _BottomObj;
        private UnityEngine.Transform _TopObj;
        private UnityEngine.Transform _RightObj;
        private UnityEngine.Transform _LeftObj;
    }
}