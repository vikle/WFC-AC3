using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCGenerator))]
public sealed class WFCGeneratorEditor : Editor
{
    WFCGenerator m_target;

    void OnEnable()
        => m_target = (WFCGenerator)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        GUILayout.Space(10f);

        if (GUILayout.Button("Generate", GUILayout.Height(36f)))
        {
            RunGeneration();
        }
        
        if (GUILayout.Button("Clear All Tiles"))
        {
            m_target.tilemap.ClearAllTiles();
        }
    }

    private async void RunGeneration()
    {
        GUI.enabled = false;
        await m_target.Generate();
        GUI.enabled = true;
        Repaint();
    }
};
