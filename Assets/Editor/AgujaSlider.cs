using UnityEditor;
using Entrenamiento.GUI.Instrumentos;

[CustomEditor(typeof(IntrumentoDeAgujaController))]
public class AgujaSlider : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        IntrumentoDeAgujaController ElObjeto = (IntrumentoDeAgujaController)target;
        if (ElObjeto.Instrumento != null)
            ElObjeto.ValorDeAguja = EditorGUILayout.IntSlider("Valor", (int)ElObjeto.ValorDeAguja, 0, 99999);
    }
}