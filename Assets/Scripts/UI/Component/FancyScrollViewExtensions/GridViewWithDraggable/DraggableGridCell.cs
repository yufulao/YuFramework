// ******************************************************************
//@file         DraggableCell.cs
//@brief        可拖拽的GridCell
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.07 21:44:57
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Yu;

public class DraggableGridCell<TItemData, TContext> : GridCell<TItemData, TContext> where TContext : DraggableContext, new()
{
    [SerializeField] protected DragUIComponent dragUI; //拖拽组件


    public override void Initialize()
    {
        dragUI.InitDragComponent(Context.OnDragParent, Context.CheckValidDrag);
        dragUI.SetDragAction(Context.DragResultFilter, OnDragBegin, OnDrag, OnDragEnd);
        dragUI.SetInvalidDragAction(OnInvalidDragBegin, OnInvalidDrag, OnInvalidDragEnd);
    }

    public override void UpdateContent(TItemData itemData)
    {
    }
    
    /// <summary>
    /// 拖拽开始时
    /// </summary>
    protected virtual void OnDragBegin(PointerEventData eventData)
    {
        Context.OnBeginDrag?.Invoke(Index);
    }

    /// <summary>
    /// 拖拽中
    /// </summary>
    protected virtual void OnDrag(PointerEventData eventData)
    {
        Context.OnDrag?.Invoke(Index);
    }

    /// <summary>
    /// 当拖拽结束时
    /// </summary>
    protected virtual void OnDragEnd(List<GameObject> objs, bool isValidDrag)
    {
        if (isValidDrag)
        {
            Context.OnEndDrag?.Invoke(Index, objs, DragFilterCallback);
        }
        
        EnableClickBtnOnEndDrag(isValidDrag);
    }
    
    /// <summary>
    /// 当拖拽结束时，拖拽是否成功回调
    /// </summary>
    protected virtual void DragFilterCallback(bool success)
    {
    }
    
    /// <summary>
    /// 当非法拖拽开始时
    /// </summary>
    protected virtual void OnInvalidDragBegin(PointerEventData eventData)
    {
        Context.Scroller.OnBeginDrag(eventData);
        Context.InvalidBeginDragDispatch?.Invoke(Index);
    }
    
    /// <summary>
    /// 当非法拖拽中
    /// </summary>
    protected virtual void OnInvalidDrag(PointerEventData eventData)
    {
        Context.Scroller.OnDrag(eventData);
        Context.InvalidOnDragDispatch?.Invoke(Index);
    }

    /// <summary>
    /// 当非法拖拽结束时
    /// </summary>
    protected virtual void OnInvalidDragEnd(PointerEventData eventData)
    {
        Context.Scroller.OnEndDrag(eventData);
        Context.InvalidEndDragDispatch?.Invoke(Index);
    }

    /// <summary>
    /// 拖拽结束时恢复btn的可交互
    /// </summary>
    protected virtual void EnableClickBtnOnEndDrag(bool isValidDrag)
    {
    }
}