// ******************************************************************
//@file         RadarChartEditor.cs
//@brief        RadarChart的Editor类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.18 15:55:08
// ******************************************************************

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using Yu;

[CustomEditor(typeof(RadarChart))]
public class RadarChartEditor : GraphicEditor
{
    private SerializedProperty _showOutline;
    private SerializedProperty _showInline;
    private SerializedProperty _showCenterPoint;
    private SerializedProperty _outlineColor;
    private SerializedProperty _inlineColor;
    private SerializedProperty _centerPointColor;
    private SerializedProperty _outlineWidth;
    private SerializedProperty _inlineWidth;
    private SerializedProperty _centerPointRadius;
    private SerializedProperty _dataArray;

    protected override void OnEnable()
    {
        base.OnEnable();
        _showOutline = serializedObject.FindProperty("showOutline");
        _showInline = serializedObject.FindProperty("showInline");
        _showCenterPoint = serializedObject.FindProperty("showCenterPoint");
        _outlineColor = serializedObject.FindProperty("outlineColor");
        _inlineColor = serializedObject.FindProperty("inlineColor");
        _centerPointColor = serializedObject.FindProperty("centerPointColor");
        _outlineWidth = serializedObject.FindProperty("outlineWidth");
        _inlineWidth = serializedObject.FindProperty("inlineWidth");
        _centerPointRadius = serializedObject.FindProperty("centerPointRadius");
        _dataArray = serializedObject.FindProperty("dataArray");
    }

    /// <summary>
    /// 重绘
    /// </summary>
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //GraphicEditor
        AppearanceControlsGUI();
        RaycastControlsGUI();
        MaskableControlsGUI();

        //自定义
        UtilsForEditor.DrawLine();
        EditorGUILayout.PropertyField(_showOutline, new GUIContent("是否显示外轮廓"));
        if (_showOutline.boolValue)
        {
            EditorGUILayout.PropertyField(_outlineColor, new GUIContent("外轮廓颜色"));
            EditorGUILayout.PropertyField(_outlineWidth, new GUIContent("外轮廓宽度"));
        }

        EditorGUILayout.PropertyField(_showInline, new GUIContent("是否显示内轮廓"));
        if (_showInline.boolValue)
        {
            EditorGUILayout.PropertyField(_inlineColor, new GUIContent("内轮廓颜色"));
            EditorGUILayout.PropertyField(_inlineWidth, new GUIContent("内轮廓宽度"));
        }

        EditorGUILayout.PropertyField(_showCenterPoint, new GUIContent("是否显示中心点"));
        if (_showCenterPoint.boolValue)
        {
            EditorGUILayout.PropertyField(_centerPointColor, new GUIContent("中心点颜色"));
            EditorGUILayout.PropertyField(_centerPointRadius, new GUIContent("中心点半径"));
        }

        EditorGUILayout.PropertyField(_dataArray, new GUIContent("线段数据"), true);
        serializedObject.ApplyModifiedProperties();
    }
}

/// <summary>
/// 绘制RadarData的字段注释
/// </summary>
[CustomPropertyDrawer(typeof(RadarChart.RadarData))]
public class RadarDataDrawer : PropertyDrawer
{
    private const float Space = 5f;

    /// <summary>
    /// 重绘
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var value = property.FindPropertyRelative("value");
        var angle = property.FindPropertyRelative("angle");
        var maxLength = property.FindPropertyRelative("maxLength");

        //分配区域
        var lineHeight = EditorGUIUtility.singleLineHeight;
        var valueRect = new Rect(position.x, position.y, position.width, lineHeight);
        var angleRect = new Rect(position.x, position.y + lineHeight + Space, position.width, lineHeight);
        var maxLengthRect = new Rect(position.x, position.y + (lineHeight + Space) * 2, position.width, lineHeight);

        //绘制
        EditorGUI.PropertyField(valueRect, value, new GUIContent("当前值"));
        EditorGUI.PropertyField(angleRect, angle, new GUIContent("区块角度"));
        EditorGUI.PropertyField(maxLengthRect, maxLength, new GUIContent("线段总长度"));
    }

    /// <summary>
    /// 控制字段的高度
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + Space) * 3; //三个字段，每个字段需要一行加间距
    }
}