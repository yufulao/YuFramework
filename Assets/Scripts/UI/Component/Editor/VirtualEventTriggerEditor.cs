 // ******************************************************************
//@file         VirtualEventTriggerEditor.cs
//@brief        VirtualEventTrigger的Editor类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.26 13:50:55
// ******************************************************************
 
using UnityEditor;
using UnityEditor.EventSystems;
using UnityEngine;

namespace Yu
{
    [CustomEditor(typeof(VirtualEventTrigger))]
    public class VirtualEventTriggerEditor : EventTriggerEditor
    {
        private SerializedProperty _isParent;
        private SerializedProperty _isChild;
        private SerializedProperty _parentVirtual;
        private SerializedProperty _childVirtualList;

        protected override void OnEnable()
        {
            base.OnEnable();
            _isParent = serializedObject.FindProperty("isParent");
            _isChild = serializedObject.FindProperty("isChild");
            _parentVirtual = serializedObject.FindProperty("parentVirtual");
            _childVirtualList = serializedObject.FindProperty("childVirtualList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            // EditorGUILayout.PropertyField(_role);
            // var role = (VirtualEventTrigger.Role)_role.enumValueIndex;
            // switch (role)
            // {
            //     case VirtualEventTrigger.Role.Parent:
            //         EditorGUILayout.PropertyField(_childVirtualList, new GUIContent("虚拟子物体列表"));
            //         break;
            //     case VirtualEventTrigger.Role.Child:
            //         EditorGUILayout.PropertyField(_parentVirtual, new GUIContent("虚拟父物体"));
            //         break;
            //     default:
            //         throw new ArgumentOutOfRangeException();
            // }
            EditorGUILayout.PropertyField(_isParent);
            EditorGUILayout.PropertyField(_isChild);
            if (_isParent.boolValue)
            {
                EditorGUILayout.PropertyField(_childVirtualList, new GUIContent("虚拟子物体列表"));
            }

            if (_isChild.boolValue)
            {
                EditorGUILayout.PropertyField(_parentVirtual, new GUIContent("虚拟父物体"));
            }

            serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
        }
    }
}