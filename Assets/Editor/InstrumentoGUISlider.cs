using UnityEditor;
using Entrenamiento.GUI.Instrumentos;
using Entrenamiento.Nucleo;

namespace Assets.Editor
{
    [CustomEditor(typeof(InstrumentoGUIController), true)]
    class InstrumentoGUISlider : UnityEditor.Editor
    {
        [UnityEngine.SerializeField]
        [UnityEngine.HideInInspector]
        private static bool mostrarValores;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            mostrarValores = EditorGUILayout.Foldout(mostrarValores, "Valores de Instrumento");
            if (mostrarValores)
            {
                InstrumentoGUIController instrumentoGUI = (InstrumentoGUIController)target;

                if (instrumentoGUI.Instrumento != null)
                {
                    ValoresDeInstrumento valores = instrumentoGUI.Instrumento.Valores;
                    ValoresDeInstrumento valoresMax = instrumentoGUI.Instrumento.ValoresMaximos;
                    ValoresDeInstrumento valoresMin = instrumentoGUI.Instrumento.ValoresMinimos;
                    for (int i = 0; i < valores.Cantidad; i++)
                    {
                        valores[i] = EditorGUILayout.Slider("Valor[" + i + "]", valores[i], valoresMax[i], valoresMin[i]);
                    }
                    instrumentoGUI.Instrumento.Valores = valores;
                }
            }
        }
    }
}
