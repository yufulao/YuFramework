// ******************************************************************
//@file         IPoolableObject.cs
//@brief        对象池的对象，接口
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.02 00:15:38
// ******************************************************************

namespace Yu
{
    public interface IPoolableObject
    {
        void OnActivate();//激活
        void OnDeactivate();//主动归还时
        void OnIdleDestroy();//销毁闲置上限的对象
        float LastUsedTime { get; }//对象上一次使用的时间
        bool Active{ get; }
    }
}
