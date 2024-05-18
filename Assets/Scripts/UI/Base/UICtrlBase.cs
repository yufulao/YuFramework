// ******************************************************************
//@file         UICtrlBase.cs
//@brief        C#层UI控制器基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:57
// ******************************************************************
using UnityEngine;

namespace Yu
{
    public abstract class UICtrlBase : MonoBehaviour
    {
        public abstract void OnInit(params object[] param);

        public abstract void OpenRoot(params object[] param);

        public abstract void CloseRoot();

        public abstract void BindEvent();
    }
}