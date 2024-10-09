// ******************************************************************
//@file         IComponentTimeUser.cs
//@brief        ComponentTimeUser接口类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 19:43:05
// ******************************************************************

namespace Yu
{
    public interface IComponentTimeUser//面向接口，TimeUserEffector统一管理
    {
        void Initialize();
        void OnStartOrReEnable();
        void Update();
        void FixedUpdate();
        void OnDisable();
        void AdjustProperties();
        void Reset();
    }
}