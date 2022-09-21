#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Displays container content in Inspector of child classes of ContainerPouringSystem
/// </summary>
[CustomEditor(typeof(ContainerPouringSystem), true)]
public class ElementContainer_Inspector : Editor
{
    bool showList = false;
    public override void OnInspectorGUI()
    {
        var container  = ((ContainerPouringSystem)target).Container;
        if(container == null) return;
     
        EditorGUILayout.LabelField(string.Format("Filled:{0:.##}/{1:.#}ml ({2:.#}%) Mass:{3:.##}g", container.Mass(), container.maxStorage, container.FilledPercantage() * 100, container.Weight()));
        if (container.IsEmpty())
        {
            EditorGUILayout.LabelField("currently Empty");            
        }
        else
        {
            //Details
            EditorGUILayout.LabelField(string.Format("T:{0:.##}°C Ph:{1}", container.temperature, container.PhValue()));
            EditorGUILayout.ColorField("Color:", container.GetColor());
            //List of ElementAmounts
            showList = EditorGUILayout.BeginFoldoutHeaderGroup(showList && container.content.Count > 0, string.Format("Chems: {0} Weight: {1:.##}g", container.content.Count, container.Weight() - container.EmptyWeight));
            if (showList)
                foreach (var item in container.content)
                {
                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField(item.ToString());
                    EditorGUILayout.EndHorizontal();
                }
            EditorGUILayout.EndFoldoutHeaderGroup();
            //Buttons
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Clear"))
            {
                foreach (var item in container.content)
                    Debug.Log("Removing: " + item.ToString());
                container.RemoveAmount(container.Mass());
            }
            if (GUILayout.Button("Log"))
            {
                string content = "Content:";
                foreach (var item in container.content)
                    content += "\n" + item.ToString();
                Debug.Log(content);
            }
            EditorGUILayout.EndHorizontal();
        }        
        EditorGUILayout.Space(20);
        DrawDefaultInspector();
    }
}

#endif