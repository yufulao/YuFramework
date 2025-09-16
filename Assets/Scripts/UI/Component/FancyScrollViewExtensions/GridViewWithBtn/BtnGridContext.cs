// ******************************************************************
//@file         BtnGridContext.cs
//@brief        带默认btn的列表Context
//@author       yufulao, yufulao@qq.com
//@createTime   2025.05.10 16:57:49 
// ******************************************************************

using System;

public class BtnGridContext: GridContext
{
    public int SelectedIndex = -1;
    public Action<int> OnItemClicked;
}