// ******************************************************************
//@file         TMP_LinkAction.cs
//@brief        处理TextMeshProUGUI文本的Link标签调用Action
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.25 14:36:51
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Yu;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(EventTrigger))]
public class TMP_LinkAction : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private EventTrigger _eventTrigger;
    private readonly Dictionary<string, Action<BaseEventData>> _linkId2Action = new();


    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _eventTrigger = GetComponent<EventTrigger>();
        Utils.AddTriggersListener(_eventTrigger, EventTriggerType.PointerClick, OnPointerClick);
    }

    /// <summary>
    /// 绑定link标签对应的事件
    /// </summary>
    public bool BindEvent(string linkId, Action<BaseEventData> action)
    {
        return _linkId2Action.TryAdd(linkId, action);
    }

    /// <summary>
    /// 鼠标点击时
    /// </summary>
    private void OnPointerClick(BaseEventData eventData)
    {
        var linkIndex = GetLinkIndex((PointerEventData)eventData);
        if (linkIndex < 0)
        {
            return;
        }

        var linkID = _text.textInfo.linkInfo[linkIndex].GetLinkID(); //<link>标签id
        // GameLog.Info(linkID+"  "+linkID.Length+"  "+linkID.GetHashCode());
        // GameLog.Info(Utils.ToStringMap(_linkId2Action, str=>str+"  "+str.Length+"  "+str.GetHashCode(), null));
        // GameLog.Info(_linkId2Action.ContainsKey(linkID));
        _linkId2Action.TryGetValue(linkID, out var action);
        action?.Invoke(eventData);
    }

    /// <summary>
    /// 获取点击到的linkInfo
    /// </summary>
    private int GetLinkIndex(PointerEventData eventData)
    {
        if (!_text.richText)
        {
            return -1;
        }

        // GameLog.Info(Utils.ToStringArray(_text.textInfo.linkInfo, tmpLinkInfo => tmpLinkInfo.GetLinkID()));
        // GameLog.Info(TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, eventData.pressEventCamera));
        return TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, eventData.pressEventCamera);
    }
}