using System.Collections.Generic;
using UnityEngine;
using Yu;

/// <summary>
/// GM指令界面model
/// </summary>
public class GMModel
{
    private bool _hadInit = false;
    public readonly List<GMMethodData> MethodDataList = new List<GMMethodData>();

    /// <summary>
    /// 尝试初始化
    /// </summary>
    public bool TryInit()
    {
        if (_hadInit)
        {
            return true;
        }

        if (!GMCommand.HadInit)
        {
            return false;
        }
        
        foreach (var methodData in GMCommand.MethodDataList)
        {
            if (methodData.UIShow)
            {
                MethodDataList.Add(methodData);
            }
        }

        _hadInit = true;
        return true;
    }

    /// <summary>
    /// 打开界面时
    /// </summary>
    public void OnOpen()
    {
    }
}