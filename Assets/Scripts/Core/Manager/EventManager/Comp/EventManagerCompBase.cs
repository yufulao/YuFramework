// ******************************************************************
//@file         EventManagerCompBase.cs
//@brief        事件系统逻辑实现基类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.10 14:33:57
// ******************************************************************

using System.Collections.Generic;

namespace Yu
{
    public abstract class EventManagerCompBase
    {
        protected readonly Dictionary<EventName, int> DispatchingDict0 = new(); //<派发中的事件, 当前执行的listener下标>
        protected readonly Dictionary<EventName, int> DispatchingDict1 = new();
        protected readonly Dictionary<EventName, int> DispatchingDict2 = new();
        protected readonly Dictionary<EventName, int> DispatchingDict3 = new();
        protected readonly Dictionary<int, object[]> ParamOnDispatch = new()//派发中参数, <参数个数, 实参>
        {
            {1, new object[1]}, {2, new object[2]}, {3, new object[3]},
        };
        
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="listener">监听者</param>
        /// <param name="priority">优先度</param>
        public virtual void AddListener(EventName eventName, EventManager.YuEvent listener, int priority = 0){}
        public virtual void AddListener<T1>(EventName eventName, EventManager.YuEvent<T1> listener, int priority = 0){}
        public virtual void AddListener<T1, T2>(EventName eventName, EventManager.YuEvent<T1, T2> listener, int priority = 0){}
        public virtual void AddListener<T1, T2, T3>(EventName eventName, EventManager.YuEvent<T1, T2, T3> listener, int priority = 0){}
        
        /// <summary>
        /// 取消订阅
        /// </summary>
        public virtual void RemoveListener(EventName eventName, EventManager.YuEvent listener){}
        public virtual void RemoveListener<T1>(EventName eventName, EventManager.YuEvent<T1> listener){}
        public virtual void RemoveListener<T1, T2>(EventName eventName, EventManager.YuEvent<T1, T2> listener){}
        public virtual void RemoveListener<T1, T2, T3>(EventName eventName, EventManager.YuEvent<T1, T2, T3> listener){}

        /// <summary>
        /// 派发事件
        /// </summary>
        public virtual void Dispatch(EventName eventName){}
        public virtual void Dispatch<T1>(EventName eventName, T1 param){}
        public virtual void Dispatch<T1, T2>(EventName eventName, T1 param1, T2 param2){}
        public virtual void Dispatch<T1, T2, T3>(EventName eventName, T1 param1, T2 param2, T3 param3){}
        
    }
}