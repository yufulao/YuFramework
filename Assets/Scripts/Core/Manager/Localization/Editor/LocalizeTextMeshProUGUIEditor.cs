// ******************************************************************
//@file         LocalizeTextMeshProUGUIEditor.cs
//@brief        LocalizeTextMeshProUGUI的inspect显示
//@author       yufulao, yufulao@qq.com
//@createTime   2024.10.27 21:40:36
// ******************************************************************

using TMPro.EditorUtilities;
using UnityEditor;

[CustomEditor(typeof(LocalizeTextMeshProUGUI), true), CanEditMultipleObjects]
public class LocalizeTextMeshProUGUIEditor : TMP_EditorPanelUI
{
    private SerializedProperty _textKey;
    private SerializedProperty _fontList;

    protected override void OnEnable()
    {
        base.OnEnable();
        _textKey = serializedObject.FindProperty("textKey");
        _fontList = serializedObject.FindProperty("fontList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_textKey);
        EditorGUILayout.PropertyField(_fontList);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}