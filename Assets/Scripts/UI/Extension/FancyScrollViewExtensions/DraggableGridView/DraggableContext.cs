 // ******************************************************************
//@file         DraggableContext.cs
//@brief        可拖拽item的context
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.07 21:38:58
// ******************************************************************

using System;
using System.Collections.Generic;
using FancyScrollView;
using UnityEngine;

public class DraggableContext : GridContext
{
    public Scroller Scroller;
    //Cell使用
    public Transform OnDragParent;
    public bool CheckValidDrag;
    public Func<GameObject, bool> Filter;
    public Action<int> OnBeginDrag;
    public Action<int> OnDrag;
    public Action<int, List<GameObject>> OnEndDrag;
    public Action<int> InvalidBeginDragDispatch; //无效拖拽开始时转发的事件
    public Action<int> InvalidOnDragDispatch; //无效拖拽中转发的事件
    public Action<int> InvalidEndDragDispatch; //无效拖拽结束时转发的事件

    
    /// <summary>
    /// 手动初始化
    /// </summary>
    public void Init(Transform onDragParent, bool checkValidDrag)
    {
        OnDragParent = onDragParent;
        CheckValidDrag = checkValidDrag;
    }
    
    /// <summary>
    /// 设置拖拽的事件，包括过滤器，开始拖拽，拖拽结束
    /// </summary>
    public void SetDragAction(Func<GameObject, bool> filter, Action<int> onBeginDrag = null, Action<int> onDrag = null, Action<int, List<GameObject>> onEndDrag = null)
    {
        Filter = filter;
        OnBeginDrag = onBeginDrag;
        OnDrag = onDrag;
        OnEndDrag = onEndDrag;
    }

    /// <summary>
    /// 设置当前是否是有效拖拽，（有效拖拽即拖拽的是目标ui）
    /// </summary>
    public void SetInvalidDragAction(Action<int> invalidBeginDragDispatch = null, Action<int> invalidOnDragDispatch = null, Action<int> invalidEndDragDispatch = null)
    {
        InvalidBeginDragDispatch = invalidBeginDragDispatch;
        InvalidOnDragDispatch = invalidOnDragDispatch;
        InvalidEndDragDispatch = invalidEndDragDispatch;
    }
}