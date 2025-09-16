// ******************************************************************
//@file         GridCell.cs
//@brief        网格列表的Item
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 16:07:16
// ******************************************************************

using FancyScrollView;

public class GridCell<TData, TContext> : FancyGridViewCell<TData, TContext> where TContext : GridContext, new()
{
    /// <summary>
    /// 初始化
    /// </summary>
    public override void Initialize()
    {
    }

    /// <summary>
    /// 通过data刷新单个item的内容
    /// </summary>
    public override void UpdateContent(TData data)
    {
    }
}