// ******************************************************************
//@file         TMP_Dropdown_Event.cs
//@brief        自定义dropdown，暴露展开和关闭事件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.07 17:44:35
// ******************************************************************

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TMP_Dropdown_Event : TMP_Dropdown
{
    public UnityEvent onDropdownOpenBefore = new UnityEvent();
    public UnityEvent onDropdownOpenAfter = new UnityEvent();
    public UnityEvent onDropdownCloseBefore = new UnityEvent();
    public UnityEvent onDropdownCloseAfter = new UnityEvent();


    public override void OnPointerClick(PointerEventData eventData)
    {
        onDropdownOpenBefore.Invoke();
        base.OnPointerClick(eventData);
        onDropdownOpenAfter.Invoke();
        // Debug.LogError("OnPointerClick");
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        onDropdownOpenBefore.Invoke();
        base.OnSubmit(eventData);
        onDropdownOpenAfter.Invoke();
        // Debug.LogError("OnSubmit");
    }

    public override void OnCancel(BaseEventData eventData)
    {
        onDropdownCloseBefore.Invoke();
        base.OnCancel(eventData);
        onDropdownCloseAfter.Invoke();
        // Debug.LogError("OnCancel");
    }

    public override void OnSelect(BaseEventData eventData)
    {
        onDropdownCloseBefore.Invoke();
        base.OnSelect(eventData);
        onDropdownCloseAfter.Invoke();
        // Debug.LogError("OnSelect");
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        // Debug.LogError("OnDeselect");
    }

    /// <summary>
    /// 刷新当前options，保留原有选择
    /// </summary>
    public void UpdateOptions(List<string> optionList)
    {
        if (optionList == null)
        {
            ClearOptions();
            return;
        }
        
        if (value >= options.Count) //非法数据遗留
        {
            ClearOptions();
            AddOptions(optionList);
            return;
        }

        //保留原选项
        var cacheString = options[value].text;
        ClearOptions();
        AddOptions(optionList);
        for (var i = 0; i < optionList.Count; i++)
        {
            if (!optionList[i].Equals(cacheString))
            {
                continue;
            }

            value = i;
            return;
        }
    }
}