#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticleRegistry))]
public class ParticleRegistry_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        var r = (ParticleRegistry)target;
        if (r == null) return;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Particles: " + r.GetRegisteredParticles());
        EditorGUILayout.LabelField("Defaults: " + r.GetRegisteredDefaults());
        EditorGUILayout.EndHorizontal();

        base.OnInspectorGUI();
    }
}

#endif