// ******************************************************************
//@file         GridCell.cs
//@brief        网格列表的Item
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 16:07:16
// ******************************************************************

using FancyScrollView;
using UnityEngine;
using UnityEngine.UI;

public class GridCell<TData, TContext> : FancyGridViewCell<TData, TContext> where TContext : GridContext, new()
{
    [SerializeField] protected Button btn;

    public override void Initialize()
    {
        btn?.onClick.AddListener(() => Context.OnItemClicked?.Invoke(Index));
    }

    /// <summary>
    /// 通过data刷新单个item的内容
    /// </summary>
    public override void UpdateContent(TData data)
    {
    }
}