using UnityEngine;
using UnityEditor;

public class GUI_OutlinedDiffuse : ShaderGUI
{
    MaterialEditor editor;
    MaterialProperty[] properties;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] materialProperties)
    {
        this.editor = materialEditor;
        this.properties = materialProperties;

        DiffuseSection();
        OutlineSection();
    }

    void DiffuseSection()
    {
        GUILayout.Label("Diffuse", EditorStyles.boldLabel);
        MaterialProperty mainTex = FindProperty("_MainTex", properties);
        GUIContent mainTexLabel = new GUIContent(mainTex.displayName, "Diffuse");
        editor.TexturePropertySingleLine(mainTexLabel, mainTex, FindProperty("_Color", properties));
        editor.TextureScaleOffsetProperty(mainTex);
    }

    void OutlineSection()
    {
        GUILayout.Label("Outline", EditorStyles.boldLabel);
        MaterialProperty outlineColor = FindProperty("_OutlineColor", properties);
        MaterialProperty outlineThickness = FindProperty("_OutlineThickness", properties);
        editor.ShaderProperty(outlineColor, new GUIContent(outlineColor.displayName, "Outline Color"));
        editor.ShaderProperty(outlineThickness, new GUIContent(outlineThickness.displayName, "Outline Thickness"));
    }

}
