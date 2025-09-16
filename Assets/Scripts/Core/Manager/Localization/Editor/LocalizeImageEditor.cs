// ******************************************************************
//@file         LocalizeImageEditor.cs
//@brief        图片本地化的inspector
//@author       yufulao, yufulao@qq.com
//@createTime   2024.10.28 14:02:51
// ******************************************************************

using UnityEditor;
using UnityEditor.UI;
using Yu;

[CustomEditor(typeof(LocalizeImage), true), CanEditMultipleObjects]
public class LocalizeImageEditor : ImageEditor
{
    private SerializedProperty _imageKey;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        _imageKey = serializedObject.FindProperty("imageKey");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_imageKey);
        UtilsForEditor.DrawLine();
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}