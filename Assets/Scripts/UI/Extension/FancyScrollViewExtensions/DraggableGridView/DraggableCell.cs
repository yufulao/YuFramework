// ******************************************************************
//@file         DraggableCell.cs
//@brief        可拖拽的GridCell
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.07 21:44:57
// ******************************************************************

using System.Collections.Generic;
using FancyScrollView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yu;

public class DraggableCell<TItemData, TContext> : FancyGridViewCell<TItemData, TContext> where TContext : DraggableContext, new()
{
    [SerializeField] protected Button btn;
    [SerializeField] protected DragUIComponent dragUI; //拖拽组件


    public override void Initialize()
    {
        btn.onClick.AddListener(() => Context.OnItemClicked?.Invoke(Index));
        dragUI.InitDragComponent(Context.OnDragParent, Context.CheckValidDrag);
        dragUI.SetDragAction(Context.Filter, OnDragBegin, OnDrag, OnDragEnd);
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
        btn.interactable = false;
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
            Context.OnEndDrag?.Invoke(Index, objs);
        }
        
        EnableClickBtnOnEndDrag(isValidDrag);
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
        EnableClickBtn();
    }

    /// <summary>
    /// 拖拽结束时恢复btn的可交互
    /// </summary>
    private void EnableClickBtnOnEndDrag(bool isValidDrag)
    {
        if (isValidDrag)
        {
            EnableClickBtn();
        }
    }

    /// <summary>
    /// 恢复btn的可交互
    /// </summary>
    private void EnableClickBtn()
    {
        btn.interactable = true;
    }
}