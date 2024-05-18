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
        void OnInit();
        void Update();
        void FixedUpdate();
        void LateUpdate();
        void OnClear();
    }
}