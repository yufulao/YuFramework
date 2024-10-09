// ******************************************************************
//@file         TimeState.cs
//@brief        时间流逝状态定义类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.20 14:59:40
// ******************************************************************

namespace Yu
{
    public enum TimeState
    {
        Accelerated,//>1
        Normal,//=1
        Slowed,//0< <1
        Paused,//=0
        Reversed//<0
    }
}
