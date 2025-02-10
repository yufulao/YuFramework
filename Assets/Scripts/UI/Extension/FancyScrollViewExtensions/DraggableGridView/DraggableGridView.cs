// ******************************************************************
//@file         DraggableGridView.cs
//@brief        可拖拽item的GridView
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.07 21:52:42
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

public class DraggableGridView<TData, TContext, TGroup> : GridView<TData, TContext, TGroup> where TContext : DraggableContext, new() where TGroup : DraggableGroup<TData, TContext>
{
    /// <summary>
    /// 初始化拖拽组件，需要手动初始化
    /// </summary>
    public void InitDragComponent(Transform onDragParent, bool checkValidDrag)
    {
        Context.Init(onDragParent, checkValidDrag);
        Context.Scroller = Scroller;
    }

    /// <summary>
    /// 设置拖拽的事件，包括过滤器，开始拖拽，拖拽结束
    /// </summary>
    /// <param name="filter">拖拽事件参数的目标</param>
    /// <param name="onBeginDrag">拖拽开始事件</param>
    /// <param name="onDrag">拖拽中事件</param>
    /// <param name="onEndDrag">拖拽结束事件</param>
    public void SetDragAction(Func<GameObject, bool> filter, Action<int> onBeginDrag = null, Action<int> onDrag = null, Action<int, List<GameObject>> onEndDrag = null)
    {
        Context.SetDragAction(filter, onBeginDrag, onDrag, onEndDrag);
    }

    /// <summary>
    /// 设置当前是否是有效拖拽，（有效拖拽即拖拽的是目标ui）
    /// </summary>
    /// <param name="invalidBeginDragDispatch">非法拖拽开始时</param>
    /// <param name="invalidOnDragDispatch">非法拖拽中</param>
    /// <param name="invalidEndDragDispatch">非法拖拽结束时</param>
    public void SetInvalidDragAction(Action<int> invalidBeginDragDispatch = null, Action<int> invalidOnDragDispatch = null, Action<int> invalidEndDragDispatch = null)
    {
        Context.SetInvalidDragAction(invalidBeginDragDispatch, invalidOnDragDispatch, invalidEndDragDispatch);
    }
}