// ******************************************************************
//@file         CustomFillOriginImageEditor.cs
//@brief        CustomFillOriginImage的Editor类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.22 01:36:22
// ******************************************************************

using UnityEditor;
using UnityEngine;

namespace Yu
{
    [CustomEditor(typeof(CustomFillOriginImage))]
    public class CustomFillOriginImageEditor : Editor
    {
        private SerializedProperty _customFillOriginProperty;
        private SerializedProperty _customFillMaxAngleProperty;
        private SerializedProperty _clockwiseProperty;
        private SerializedProperty _lineLengthProperty;
        private SerializedProperty _fillOriginProperty;
        private SerializedProperty _fillClockwiseProperty;
        private SerializedProperty _typeProperty;
        private SerializedProperty _fillMethodProperty;

        private void OnEnable()
        {
            _customFillOriginProperty = serializedObject.FindProperty("customFillOrigin");
            _customFillMaxAngleProperty = serializedObject.FindProperty("customFillMaxAngle");
            _clockwiseProperty = serializedObject.FindProperty("clockwise");
            _lineLengthProperty = serializedObject.FindProperty("lineLength");
            
            _fillOriginProperty = serializedObject.FindProperty("m_FillOrigin");
            _fillClockwiseProperty = serializedObject.FindProperty("m_FillClockwise");
            _typeProperty = serializedObject.FindProperty("m_Type");
            _fillMethodProperty = serializedObject.FindProperty("m_FillMethod");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _fillOriginProperty.isExpanded = false;
            _fillClockwiseProperty.isExpanded = false;
            _typeProperty.isExpanded = false;
            _fillMethodProperty.isExpanded = false;
            DrawPropertiesExcluding(serializedObject, "customFillOrigin", "customFillMaxAngle", "clockwise", "lineLength", "m_FillOrigin", "m_FillClockwise", "m_Type", "m_FillMethod");
            EditorGUILayout.Slider(_customFillOriginProperty, 0f, 360f, new GUIContent("Custom Fill Origin"));
            EditorGUILayout.Slider(_customFillMaxAngleProperty, 0f, 360f, new GUIContent("Custom Fill Max Angle"));
            EditorGUILayout.PropertyField(_clockwiseProperty, new GUIContent("Clockwise"));
            EditorGUILayout.PropertyField(_lineLengthProperty, new GUIContent("Line Length"));
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            var customFilledImage = (CustomFillOriginImage) target;
            if (!customFilledImage)
            {
                return;
            }
            
            var transform = customFilledImage.transform;
            //起始角度和最大角度的计算
            var startAngle = customFilledImage.CustomFillOrigin;
            var maxAngle = customFilledImage.CustomFillMaxAngle;
            var endAngle = customFilledImage.Clockwise ? startAngle + maxAngle : startAngle - maxAngle;
            //计算起始角度和结束角度的弧度值
            var startRad = (startAngle - 90) * Mathf.Deg2Rad;
            var endRad = (endAngle - 90) * Mathf.Deg2Rad;
            //计算起始点和终点的世界坐标
            var position = transform.position;
            var startPoint = position + new Vector3(Mathf.Cos(startRad), Mathf.Sin(startRad)) * customFilledImage.LineLength;
            var endPoint = position + new Vector3(Mathf.Cos(endRad), Mathf.Sin(endRad)) * customFilledImage.LineLength;
            Handles.color = Color.red;
            Handles.DrawLine(position, startPoint); //绘制起始线段
            Handles.DrawLine(position, endPoint); //绘制终点线段
        }
    }
}