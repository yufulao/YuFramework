// ******************************************************************
//@file         EventName.cs
//@brief        事件名定义类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:25:30
// ******************************************************************
namespace Yu
{
    public enum EventName
    {
        OnMouseLeftClick,
        OnMouseRightClick,
        OnHoldBegin,
        OnHoldEnd,
        ChangeScene,
        
        OnExitGame,//退出游戏时
        OnPauseGame,//暂停游戏时
        OnResumeGame,//继续游戏时
        OnPauseViewClose,//暂停界面关闭时
    }
}
