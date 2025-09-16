// ******************************************************************
//@file         UtilsForEditor.cs
//@brief        Utils的关于Editor部分
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.18 16:44:08
// ******************************************************************

using UnityEditor;
using UnityEngine;

namespace Yu
{
    public static class UtilsForEditor
    {
        /// <summary>
        /// 绘制分割线
        /// </summary>
        public static void DrawLine()
        {
            EditorGUILayout.Space();
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, Color.gray);
            EditorGUILayout.Space();
        }
    }
}