// ******************************************************************
//@file         HUDBase.cs
//@brief        HUD界面基类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.22 14:38:51
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public abstract class HUDBase : MonoBehaviour
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void OnInit();

        /// <summary>
        /// 绑定事件
        /// </summary>
        public abstract void BindEvent();

        /// <summary>
        /// 关闭HUD
        /// </summary>
        public abstract void CloseRoot();

        /// <summary>
        /// 销毁HUD时
        /// </summary>
        public abstract void OnDestroy();
    }
}