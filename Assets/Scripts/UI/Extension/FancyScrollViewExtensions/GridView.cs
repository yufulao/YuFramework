// ******************************************************************
//@file         GridView.cs
//@brief        grid列表
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 16:21:39
// ******************************************************************

using System;
using UnityEngine;
using EasingCore;
using FancyScrollView;

public enum Alignment
{
    Upper,
    Middle,
    Lower,
}

public class GridView<TData, TContext, TGroup> : FancyGridView<TData, TContext> where TContext : GridContext, new() where TGroup : GridGroup<TData, TContext>
{
    [SerializeField] private FancyGridViewCell<TData, TContext> itemPrefab;
    
    protected override void SetupCellTemplate() => Setup<TGroup>(itemPrefab);

    public float PaddingTop
    {
        get => paddingHead;
        set
        {
            paddingHead = value;
            Relayout();
        }
    }

    public float PaddingBottom
    {
        get => paddingTail;
        set
        {
            paddingTail = value;
            Relayout();
        }
    }

    public float SpacingY
    {
        get => spacing;
        set
        {
            spacing = value;
            Relayout();
        }
    }

    public float SpacingX
    {
        get => startAxisSpacing;
        set
        {
            startAxisSpacing = value;
            Relayout();
        }
    }

    /// <summary>
    /// 刷新当前选择的item
    /// </summary>
    public void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();
    }

    /// <summary>
    /// 当item点击时
    /// </summary>
    public virtual void OnItemClicked(Action<int> callback)
    {
        Context.OnItemClicked = callback;
    }

    /// <summary>
    /// 列表滑动到指定item
    /// </summary>
    public void ScrollTo(int index, float duration, Ease easing, Alignment alignment = Alignment.Middle)
    {
        UpdateSelection(index);
        ScrollTo(index, duration, easing, GetAlignment(alignment));
    }

    /// <summary>
    /// 列表定位到指定item
    /// </summary>
    public void JumpTo(int index, Alignment alignment = Alignment.Middle)
    {
        UpdateSelection(index);
        JumpTo(index, GetAlignment(alignment));
    }

    /// <summary>
    /// 获取Alignment对应的边距比
    /// </summary>
    private static float GetAlignment(Alignment alignment)
    {
        while (true)
        {
            switch (alignment)
            {
                case Alignment.Upper:
                    return 0.0f;
                case Alignment.Middle:
                    return 0.5f;
                case Alignment.Lower:
                    return 1.0f;
                default:
                    alignment = Alignment.Middle;
                    continue;
            }
        }
    }
}