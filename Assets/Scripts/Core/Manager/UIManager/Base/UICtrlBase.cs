// ******************************************************************
//@file         UICtrlBase.cs
//@brief        C#层UI控制器基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:57
// ******************************************************************

using UnityEngine;

namespace Yu
{
    //生命周期：OnInit()-> BindEvent()-> OpenRoot()-> CloseRoot()-> OnClear()
    public abstract class UICtrlBase : MonoBehaviour
    {
        /// <summary>
        /// 初始化，界面第一次打开时触发
        /// </summary>
        public abstract void OnInit(params object[] param);

        /// <summary>
        /// 打开界面，界面每次打开都会触发
        /// </summary>
        public abstract void OpenRoot(params object[] param);

        /// <summary>
        /// 关闭界面，界面每次关闭都会触发
        /// </summary>
        public abstract void CloseRoot();

        /// <summary>
        /// 销毁后，界面销毁时触发，销毁时不会触发CloseRoot()
        /// </summary>
        public abstract void OnClear();

        /// <summary>
        /// 绑定事件
        /// </summary>
        public abstract void BindEvent();
    }
}