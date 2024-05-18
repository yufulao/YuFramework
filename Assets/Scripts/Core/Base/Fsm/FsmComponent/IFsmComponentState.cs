// ******************************************************************
//@file         IFsmComponentState.cs
//@brief        自身状态机的状态
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:07:20
// ******************************************************************

namespace Yu
{
    public interface IFsmComponentState<in Owner>
    {
        /// <summary>
        /// 状态开始
        /// </summary>
        void OnEnter(Owner owner);

        /// <summary>
        /// 状态update
        /// </summary>
        void OnUpdate(Owner owner);

        /// <summary>
        /// 状态退出
        /// </summary>
        void OnExit(Owner owner);
    }
}