#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Displays container content in Inspector of child classes of ContainerPouringSystem
/// </summary>
[CustomEditor(typeof(ContainerObjectProperties))]
public class ContainerObjectProperties_Inspector : Editor
{
    bool showActualTemperature = false;
    public override void OnInspectorGUI()
    {
        if (target == null) return;
        var container = (ContainerObjectProperties)target;
        if (!container.hasContainer())
        {
            EditorGUILayout.LabelField("Temperature / Weight only at runtime");
            return;
        }
        //Temperature
        showActualTemperature = EditorGUILayout.BeginFoldoutHeaderGroup(showActualTemperature,
            "Temperature: " + container.Temperature );
        if(showActualTemperature) EditorGUILayout.LabelField("Actual Temperature: " + container.ActualTemperature);
        EditorGUILayout.EndFoldoutHeaderGroup();
        //Weight
        EditorGUILayout.LabelField("Weight: " + container.Weight);
        //Ph
        EditorGUILayout.LabelField("PhValue: " + container.PhValue);

        DrawDefaultInspector();
    }
}

#endif