// ******************************************************************
//@file         ComponentTimeUser.cs
//@brief        timeUser封装的component基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.24 14:17:31
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public abstract class ComponentTimeUser<T> : IComponentTimeUser where T : Component
    {
        protected TimeUser TimeUser { get;}
        public T Component { get;}

        protected ComponentTimeUser(TimeUser timeUser, T component)
        {
            TimeUser = timeUser;
            Component = component;
        }

        public void Initialize()
        {
            CopyProperties(Component);
        }

        public virtual void OnStartOrReEnable() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnDisable() { }
        protected virtual void CopyProperties(T source) { }
        protected virtual void AdjustProperties(float timeScale) { }

        /// <summary>
        /// 调整属性
        /// </summary>
        public void AdjustProperties()
        {
            AdjustProperties(TimeUser.timeScale);
        }

        /// <summary>
        /// TimeUserEffector中重置
        /// </summary>
        public virtual void Reset() { }
    }
}