// ******************************************************************
//@file         IFsmState.cs
//@brief        状态机状态
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:09:25
// ******************************************************************

namespace Yu
{
    public interface IFsmState
    {
        /// <summary>
        /// 状态开始
        /// </summary>
        void OnEnter(params object[] objs);

        /// <summary>
        /// 状态update
        /// </summary>
        void OnUpdate();
        
        /// <summary>
        /// 状态fixedUpdate
        /// </summary>
        void OnFixedUpdate();

        /// <summary>
        /// 状态退出
        /// </summary>
        void OnExit();
    }
}