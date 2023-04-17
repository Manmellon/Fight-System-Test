#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConfigSerializer))]
public class ConfigSerializerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ConfigSerializer configSerializer = (ConfigSerializer)target;
        if (GUILayout.Button("Create/Save config"))
        {
            configSerializer.SaveConfig();
        }
    }
}
#endif