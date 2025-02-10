// ******************************************************************
//@file         IMonoManager.cs
//@brief        管理器接口
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:09:59
// ******************************************************************

namespace Yu
{
    public interface IMonoManager
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void OnInit();
        /// <summary>
        /// 对应update
        /// </summary>
        void Update();
        /// <summary>
        /// 对应fixedUpdate
        /// </summary>
        void FixedUpdate();
        /// <summary>
        /// 对应lateUpdate
        /// </summary>
        void LateUpdate();
        /// <summary>
        /// 销毁时
        /// </summary>
        void OnClear();
    }
}