// ******************************************************************
//@file         DefaultEventManagerComp.cs
//@brief        无优先度功能的事件系统逻辑实现组件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.10 14:42:03
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class DefaultEventManagerComp: EventManagerCompBase
    {
        private readonly Dictionary<EventName, List<Delegate>> _listenerDict0 = new(); //监听者列表: List<监听者, 优先度>
        private readonly Dictionary<EventName, List<Delegate>> _listenerDict1 = new();
        private readonly Dictionary<EventName, List<Delegate>> _listenerDict2 = new();
        private readonly Dictionary<EventName, List<Delegate>> _listenerDict3 = new();
        
        public override void AddListener(EventName eventName, EventManager.YuEvent listener, int priority = 0)
        {
            AddListener(eventName, listener, _listenerDict0, DispatchingDict0, listener.Invoke);
        }

        public override void AddListener<T1>(EventName eventName, EventManager.YuEvent<T1> listener, int priority = 0)
        {
            var paramArray = ParamOnDispatch[1];
            AddListener(eventName, listener, _listenerDict1, DispatchingDict1
                , () => listener.Invoke((T1)paramArray[0])); //派发中订阅时，使用闭包
        }

        public override void AddListener<T1, T2>(EventName eventName, EventManager.YuEvent<T1, T2> listener, int priority = 0)
        {
            var paramArray = ParamOnDispatch[2];
            AddListener(eventName, listener, _listenerDict2, DispatchingDict2
                , () => listener.Invoke((T1)paramArray[0], (T2)paramArray[1]));
        }

        public override void AddListener<T1, T2, T3>(EventName eventName, EventManager.YuEvent<T1, T2, T3> listener, int priority = 0)
        {
            var paramArray = ParamOnDispatch[3];
            AddListener(eventName, listener, _listenerDict3, DispatchingDict3
                , () => listener.Invoke((T1)paramArray[0], (T2)paramArray[1], (T3)paramArray[2]));
        }

        public override void RemoveListener(EventName eventName, EventManager.YuEvent listener)
        {
            RemoveListener(eventName, listener, _listenerDict0, DispatchingDict0);
        }

        public override void RemoveListener<T1>(EventName eventName, EventManager.YuEvent<T1> listener)
        {
            RemoveListener(eventName, listener, _listenerDict1, DispatchingDict1);
        }

        public override void RemoveListener<T1, T2>(EventName eventName, EventManager.YuEvent<T1, T2> listener)
        {
            RemoveListener(eventName, listener, _listenerDict2, DispatchingDict2);
        }

        public override void RemoveListener<T1, T2, T3>(EventName eventName, EventManager.YuEvent<T1, T2, T3> listener)
        {
            RemoveListener(eventName, listener, _listenerDict3, DispatchingDict3);
        }

        public override void Dispatch(EventName eventName)
        {
            Dispatch(eventName, _listenerDict0, DispatchingDict0, listener 
                => ((EventManager.YuEvent)listener).Invoke);
        }

        public override void Dispatch<T1>(EventName eventName, T1 param)
        {
            Dispatch(eventName, _listenerDict1, DispatchingDict1, listener => () =>
            {
                ((EventManager.YuEvent<T1>)listener).Invoke(param);
            });
        }

        public override void Dispatch<T1, T2>(EventName eventName, T1 param1, T2 param2)
        {
            Dispatch(eventName, _listenerDict2, DispatchingDict2, listener => () =>
            {
                ((EventManager.YuEvent<T1, T2>)listener).Invoke(param1, param2);
            });
        }

        public override void Dispatch<T1, T2, T3>(EventName eventName, T1 param1, T2 param2, T3 param3)
        {
            Dispatch(eventName, _listenerDict3, DispatchingDict3, listener => () =>
            {
                ((EventManager.YuEvent<T1, T2, T3>)listener).Invoke(param1, param2, param3);
            });
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        private static void AddListener(EventName eventName, Delegate listener, Dictionary<EventName, List<Delegate>> listenerDict
            , Dictionary<EventName, int> dispatchingDict, Action invokeOnDispatch)
        {
            if (!listenerDict.TryGetValue(eventName, out var listenerList))
            {
                listenerList = new List<Delegate>();
                listenerDict.Add(eventName, listenerList);
            }
            
            if (CheckExitListener(listenerList, listener)) //重复订阅
            {
                return;
            }
            
            listenerList.Add(listener);
            //派发中
            if (CheckAddListenerOnDispatching(eventName, dispatchingDict))
            {
                invokeOnDispatch.Invoke();
            }
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        private static void RemoveListener(EventName eventName, Delegate listener, Dictionary<EventName, List<Delegate>> listenerDict
            , Dictionary<EventName, int> dispatchingDict)
        {
            for (var i = 0; i < listenerDict[eventName].Count; i++)
            {
                var listenerExit= listenerDict[eventName][i];
                if (listenerExit != listener)
                {
                    continue;
                }

                listenerDict[eventName].RemoveAt(i);
                CheckRemoveListenerOnDispatching(eventName, dispatchingDict);
                return;
            }

            Debug.LogError($"取消订阅失败，{listener}, 未订阅无参事件: {eventName}");
        }
        
        /// <summary>
        /// 派发事件
        /// </summary>
        private static void Dispatch(EventName eventName, Dictionary<EventName, List<Delegate>> listenerDict
            , Dictionary<EventName, int> dispatchingDict, Func<Delegate, Action> invokeOnDispatch)
        {
            if (!listenerDict.TryGetValue(eventName, out var listenerList))//无订阅事件
            {
                return;
            }
            
            if (!dispatchingDict.TryAdd(eventName, 0)) //派发index初始为0
            {
                Debug.LogError($"无参事件: {eventName}，发生同名递归派发成环异常。");
                return;
            }
            
            while (dispatchingDict[eventName] < listenerList.Count) //两个数字都会动态变更
            {
                var dispatchingIndex = dispatchingDict[eventName];
                //派发中移除最后一个订阅事件时，预期不越界
                var listener = listenerList[dispatchingIndex];
                // Debug.Log(listener.Method.Name + listener.Method.GetParameters().Length);
                invokeOnDispatch(listener).Invoke();
                dispatchingDict[eventName]++;
            }

            dispatchingDict.Remove(eventName);
        }
        
        /// <summary>
        /// 注册监听者前检查重复
        /// </summary>
        private static bool CheckExitListener<T>(List<T> listenerList, T listenerAdd) where T : Delegate
        {
            foreach (var listener in listenerList)
            {
                if (listener != listenerAdd)
                {
                    continue;
                }
        
                Debug.LogError($"重复添加事件: (类型: {listenerAdd}, 订阅名: {listenerAdd.Method.Name})");
                return true;
            }
        
            return false;
        }
        
        /// <summary>
        /// 派发中订阅事件
        /// </summary>
        private static bool CheckAddListenerOnDispatching(EventName eventName, Dictionary<EventName, int> dispatchingDict)
        {
            if (!dispatchingDict.ContainsKey(eventName))
            {
                return false;
            }

            //如果新插入的位置比i小，即优先度更高，要先派发，插入位置后面元素后移一位，即i++
            dispatchingDict[eventName]++;
            return true;
        }

        /// <summary>
        /// 派发中取消订阅事件
        /// </summary>
        private static void CheckRemoveListenerOnDispatching(EventName eventName, Dictionary<EventName, int> dispatchingDict)
        {
            if (!dispatchingDict.ContainsKey(eventName))
            {
                return;
            }

            //如果新插入的位置比i小，插入位置后面元素后移一位，即i++
            //取消订阅时先有订阅才能取消，此处dispatchingIndex--预期不会<0
            dispatchingDict[eventName]--;
        }
    }
}