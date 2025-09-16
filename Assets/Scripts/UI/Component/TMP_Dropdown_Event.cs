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
    [SerializeField] private LocalizeTextMeshProUGUI textEmpty;//无待选项时显示的文字
    public UnityEvent onDropdownOpenBefore = new();
    public UnityEvent onDropdownOpenAfter = new();
    public UnityEvent onDropdownCloseBefore = new();
    public UnityEvent onDropdownCloseAfter = new();


    public override void OnPointerClick(PointerEventData eventData)
    {
        onDropdownOpenBefore.Invoke();
        base.OnPointerClick(eventData);
        onDropdownOpenAfter.Invoke();
        // GameLog.Error("OnPointerClick");
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        onDropdownOpenBefore.Invoke();
        base.OnSubmit(eventData);
        onDropdownOpenAfter.Invoke();
        // GameLog.Error("OnSubmit");
    }

    public override void OnCancel(BaseEventData eventData)
    {
        onDropdownCloseBefore.Invoke();
        base.OnCancel(eventData);
        onDropdownCloseAfter.Invoke();
        // GameLog.Error("OnCancel");
    }

    public override void OnSelect(BaseEventData eventData)
    {
        onDropdownCloseBefore.Invoke();
        base.OnSelect(eventData);
        onDropdownCloseAfter.Invoke();
        // GameLog.Error("OnSelect");
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        // GameLog.Error("OnDeselect");
    }

    /// <summary>
    /// 刷新当前options，保留原有选择
    /// </summary>
    public void UpdateOptions(List<string> optionList)
    {
        if (optionList == null || optionList.Count <= 0)
        {
            ClearOptions();
            AfterUpdateOptions();
            return;
        }
        
        if (value >= options.Count) //非法数据遗留
        {
            ClearOptions();
            AddOptions(optionList);
            value = 0;
            AfterUpdateOptions();
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
            break;
        }
        
        AfterUpdateOptions();
    }

    /// <summary>
    /// 设置无待选项时的文字显示
    /// </summary>
    public virtual void SetEmptyTextKey(int strKey)
    {
        if (!textEmpty)
        {
            GameLog.Error("未设置textEmpty组件");
            return;
        }
        
        textEmpty.UpdateText(strKey);
    }

    /// <summary>
    /// 设置待选项后
    /// </summary>
    public virtual void AfterUpdateOptions()
    {
        if (!textEmpty)
        {
            return;
        }
        
        var isEmpty = options == null || options.Count <= 0;
        textEmpty.gameObject.SetActive(isEmpty);
    }
}