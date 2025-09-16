// ******************************************************************
//@file         UICommand.cs
//@brief        UI类GM指令
//@author       yufulao, yufulao@qq.com
//@createTime   2024.08.10 14:33:50
// ******************************************************************

using UnityEngine;
using Yu;

[GMCatalog("UI")]
public static class UICommand
{
    [GMMethodUI, GMMethod("打开UI界面", "界面id")]
    public static void OpenView(GMTypeUIName uiID)
    {
        UIManager.Instance.OpenWindow(uiID.Value);
        GameLog.Info($"打开{uiID}界面");
    }
}