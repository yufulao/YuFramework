// ******************************************************************
//@file         GridGroup.cs
//@brief        网格列表的Group
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 22:36:50
// ******************************************************************

using FancyScrollView;

/// <summary>
/// 必须套一层派生类，MonoBehaviour不支持泛型继承
/// </summary>
public class GridGroup<TData, TContext> : FancyCellGroup<TData, TContext> where TContext : GridContext, new()
{
}