// ******************************************************************
//@file         GameLog.cs
//@brief        unity日志封装
//@author       yufulao, yufulao@qq.com
//@createTime   2025.06.22 14:02:17 
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Debug = UnityEngine.Debug;

public static class GameLog
{
    private static readonly StringBuilder Sb = new(256);

    // ReSharper disable Unity.PerformanceAnalysis
    [Conditional("UNITY_EDITOR")]
    public static void Info(params object[] args)
    {
        if (args == null || args.Length == 0)
        {
            Debug.Log("null");
            return;
        }

        Sb.Clear();
        for (var i = 0; i < args.Length; i++)
        {
            if (i > 0) Sb.Append(", ");
            if (args[i] == null)
            {
                Sb.Append("null");
                continue;
            }

            Sb.Append(args[i]);
        }
        
        Debug.Log(Sb.ToString());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [Conditional("UNITY_EDITOR")]
    public static void Warn(params object[] args)
    {
        if (args == null || args.Length == 0)
        {
            Debug.Log("null");
            return;
        }

        Sb.Clear();
        for (var i = 0; i < args.Length; i++)
        {
            if (i > 0) Sb.Append(", ");
            if (args[i] == null)
            {
                Sb.Append("null");
                continue;
            }
            
            Sb.Append(args[i]);
        }

        Debug.LogWarning(Sb.ToString());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [Conditional("UNITY_EDITOR")]
    public static void Error(params object[] args)
    {
        if (args == null || args.Length == 0)
        {
            Debug.Log("null");
            return;
        }

        Sb.Clear();
        for (var i = 0; i < args.Length; i++)
        {
            if (i > 0) Sb.Append(", ");
            if (args[i] == null)
            {
                Sb.Append("null");
                continue;
            }
            
            Sb.Append(args[i]);
        }

        Debug.LogError(Sb.ToString());
    }
}