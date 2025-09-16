// ******************************************************************
//@file         TMPDropdownEventEditor.cs
//@brief        TMP_Dropdown_Event编辑器类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.06.08 15:51:40 
// ******************************************************************

using UnityEditor;
using UnityEngine;

namespace Yu
{
    [CustomEditor(typeof(TMP_Dropdown_Event))]
    public class TMPDropdownEventEditor : TMPro.EditorUtilities.DropdownEditor
    {
        private SerializedProperty _textEmpty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _textEmpty = serializedObject.FindProperty("textEmpty");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_textEmpty, new GUIContent("textEmpty"));
            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}
