using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WFCTile)), CanEditMultipleObjects]
public sealed class MetaTileEditor : Editor
{
    const float k_TexYOffset = 27f;
    const float k_ViewTexSize = 256f;
    const float k_HalfTexSize = k_ViewTexSize / 2f;

    WFCTile m_target;
    Rect m_texRect;

    void OnEnable()
        => m_target = (WFCTile)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        var tile_sprite = m_target.sprite;
        if (tile_sprite == null) return; 
        
        DrawTexture(tile_sprite.texture);
        
        DrawForwardLabel();
        DrawRightLabel();
        DrawBackLabel();
        DrawLeftLabel();

        SynchronizeAssetName(m_target, tile_sprite.name);
    }

    private void DrawTexture(Texture texture)
    {
        m_texRect = EditorGUILayout.GetControlRect();

        m_texRect.x += m_texRect.width / 2f - k_ViewTexSize / 2f;
        m_texRect.y += k_TexYOffset;

        m_texRect.width = k_ViewTexSize;
        m_texRect.height = k_ViewTexSize;

        GUI.DrawTexture(m_texRect, texture, ScaleMode.ScaleToFit);
    }

    private void DrawForwardLabel()
    {
        var rect = m_texRect;
        string label_text = m_target.forward.ToString();
        rect.x += k_HalfTexSize - GetTextSize(label_text) / 2f;
        rect.y -= k_HalfTexSize + 10f;
        GUI.Label(rect, label_text);
    }
    
    private void DrawRightLabel()
    {
        var rect = m_texRect;
        string label_text = m_target.right.ToString();
        rect.x += k_ViewTexSize + 5f;
        GUI.Label(rect, label_text);
    }
    
    private void DrawBackLabel()
    {
        var rect = m_texRect;
        string label_text = m_target.back.ToString();
        rect.x += k_HalfTexSize - GetTextSize(label_text) / 2f;
        rect.y += k_HalfTexSize + 10f;
        GUI.Label(rect, label_text);
    }
    
    private void DrawLeftLabel()
    {
        var rect = m_texRect;
        string label_text = m_target.left.ToString();
        rect.x -= GetTextSize(label_text) + 5f;
        GUI.Label(rect, label_text);
    }

    private static float GetTextSize(string text)
        => GUI.skin.label.CalcSize(new GUIContent(text)).x;
    
    private static void SynchronizeAssetName(Object asset, string desiredName)
    {
        if (Application.isPlaying || AssetDatabase.Contains(asset) == false) return;
        
        if (asset.name == desiredName) return;
        
        string asset_path = AssetDatabase.GetAssetPath(asset);
        asset.name = desiredName;
        AssetDatabase.RenameAsset(asset_path, desiredName);
        Selection.activeObject = asset;
    }
};
