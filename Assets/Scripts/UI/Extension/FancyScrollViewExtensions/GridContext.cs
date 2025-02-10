// ******************************************************************
//@file         GridContext.cs
//@brief        gridView的context
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 16:13:10
// ******************************************************************

using System;
using FancyScrollView;

/// <summary>
/// Context的作用是，在cell和gridView中间的中介，GridView设置Context，Cell使用Context
/// </summary>
public class GridContext: FancyGridViewContext
{
    public int SelectedIndex = -1;
    public Action<int> OnItemClicked;
}