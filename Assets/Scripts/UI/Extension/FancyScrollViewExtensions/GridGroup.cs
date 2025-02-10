// ******************************************************************
//@file         GridGroup.cs
//@brief        网格列表的Group
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 22:36:50
// ******************************************************************

using FancyScrollView;

public class GridGroup<TData, TContext> : FancyCellGroup<TData, TContext> where TContext : GridContext, new()
{
}