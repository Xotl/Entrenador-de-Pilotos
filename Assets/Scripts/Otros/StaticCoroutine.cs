using UnityEngine;
using System.Collections;

public class StaticCoroutine : MonoBehaviour
{
    private static StaticCoroutine _instancia = null;
    private static StaticCoroutine instancia
    {
        get
        {
            if (StaticCoroutine._instancia == null)
            {
                GameObject gameObject = new GameObject("StaticCoroutine");
                StaticCoroutine._instancia = gameObject.AddComponent<StaticCoroutine>();
            }

            return StaticCoroutine._instancia;
        }
    }

    /// <summary>
    /// Esta función es equivalente al StartCoroutine().
    /// </summary>
    /// <param name="value">Método a realizar.</param>
    public static void IniciarCoroutine(IEnumerator value)
    {
        StaticCoroutine.instancia.StartCoroutine(value);
    }

    private void OnDestroy()
    {
        StaticCoroutine._instancia = null;
    }
}
