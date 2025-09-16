// ******************************************************************
//@file         BtnGridView.cs
//@brief        带默认btn的列表
//@author       yufulao, yufulao@qq.com
//@createTime   2025.05.10 16:55:43 
// ******************************************************************

using System;
using EasingCore;

public class BtnGridView<TData, TContext, TGroup> : GridView<TData, TContext, TGroup> where TContext : BtnGridContext, new() where TGroup : GridGroup<TData, TContext>
{
    /// <summary>
    /// 当item点击时
    /// </summary>
    public virtual void OnItemClicked(Action<int> callback)
    {
        Context.OnItemClicked = callback;
    }
    
    /// <summary>
    /// 刷新当前选择的item
    /// </summary>
    public virtual void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();
    }

    /// <summary>
    /// 列表滑动到指定item
    /// </summary>
    public override void ScrollTo(int index, float duration, Ease easing, Alignment alignment = Alignment.Middle)
    {
        base.ScrollTo(index, duration, easing, alignment);
        UpdateSelection(index);
    }

    /// <summary>
    /// 列表定位到指定item
    /// </summary>
    public override void JumpTo(int index, Alignment alignment = Alignment.Middle)
    {
        base.JumpTo(index, alignment);
        UpdateSelection(index);
    }
}