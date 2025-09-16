// ******************************************************************
//@file         ContentSizeFitterProEditor.cs
//@brief        ContentSizeFitterPro 的自定义 Inspector
//@autho        yufulao, yufulao@qq.com
//@createTime   2025.05.11 14:17:50
// ******************************************************************

using UnityEditor;
using UnityEngine;

namespace Yu
{
    [CustomEditor(typeof(ContentSizeFitterPro))]
    public class ContentSizeFitterProEditor : Editor
    {
        private SerializedProperty _horizontalFit;
        private SerializedProperty _verticalFit;
        private SerializedProperty _rebuildListOnLayoutChanged;
        private SerializedProperty _onLayoutChanged;
        private SerializedProperty _enableMaxWidth;
        private SerializedProperty _maxWidth;
        private SerializedProperty _enableMaxHeight;
        private SerializedProperty _maxHeight;

        protected virtual void OnEnable()
        {
            _horizontalFit = serializedObject.FindProperty("m_HorizontalFit");
            _verticalFit = serializedObject.FindProperty("m_VerticalFit");
            _rebuildListOnLayoutChanged = serializedObject.FindProperty("rebuildListOnLayoutChanged");
            _onLayoutChanged = serializedObject.FindProperty("onLayoutChanged");
            _enableMaxWidth = serializedObject.FindProperty("enableMaxWidth");
            _maxWidth = serializedObject.FindProperty("maxWidth");
            _enableMaxHeight = serializedObject.FindProperty("enableMaxHeight");
            _maxHeight = serializedObject.FindProperty("maxHeight");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // if (GUILayout.Button("开始布局"))
            // {
            //     if (target is ContentSizeFitterPro fitter)
            //     {
            //         LayoutRebuilder.ForceRebuildLayoutImmediate(fitter.GetComponent<RectTransform>());
            //         fitter.SetLayoutHorizontal();
            //         fitter.SetLayoutVertical();
            //         EditorUtility.SetDirty(target);
            //     }
            // }
            
            EditorGUILayout.PropertyField(_horizontalFit, new GUIContent("水平模式"));
            EditorGUILayout.PropertyField(_verticalFit, new GUIContent("垂直模式"));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_enableMaxWidth, new GUIContent("限制最大宽度"));
            if (_enableMaxWidth.boolValue)
            {
                EditorGUILayout.PropertyField(_maxWidth, new GUIContent("最大宽度"));
            }

            EditorGUILayout.PropertyField(_enableMaxHeight, new GUIContent("限制最大高度"));
            if (_enableMaxHeight.boolValue)
            {
                EditorGUILayout.PropertyField(_maxHeight, new GUIContent("最大高度"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_rebuildListOnLayoutChanged, new GUIContent("布局改变时重绘列表"));
            EditorGUILayout.PropertyField(_onLayoutChanged, new GUIContent("布局改变事件"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
