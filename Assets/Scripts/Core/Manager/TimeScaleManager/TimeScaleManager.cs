// ******************************************************************
//@file         TimeManager.cs
//@brief        时间管理系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.20 14:42:13
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class TimeScaleManager : MonoSingleton<TimeScaleManager>//todo 待做：脱离mono
    {
        private readonly Dictionary<string, TimeHolder> _timeHolderList = new();
        public IEnumerable<TimeHolder> TimeHolderList => _timeHolderList.Values;
        

        protected override void Awake()
        {
            base.Awake();
            foreach (var globalTimeHolder in GetComponents<TimeHolder>())
            {
                //GameLog.Info(globalTimeHolder.key);
                _timeHolderList.Add(globalTimeHolder.key, globalTimeHolder);
            }
        }

        /// <summary>
        /// 添加TimeHolder
        /// </summary>
        public void AddTimeHolder(string key,TimeHolder holder)
        {
            if (HasTimeHolder(key))
            {
                GameLog.Error("TimeHolder重复添加" + key);
                return;
            }
            
            _timeHolderList.Add(key, holder);
        }

        /// <summary>
        /// 移除TimeHolder
        /// </summary>
        public void RemoveTimeHolder(string key)
        {
            if (!HasTimeHolder(key))
            {
                GameLog.Error("没有这个TimeHolder" + key);
                return;
            }

            _timeHolderList.Remove(key);
        }

        /// <summary>
        /// 获取当前的时间流逝状态
        /// </summary>
        public static TimeState GetTimeState(float timeScale)
        {
            switch (timeScale)
            {
                case <0:
                    return TimeState.Reversed;
                case 0f:
                    return TimeState.Paused;
                case <1:
                    return TimeState.Slowed;
                case 1:
                    return TimeState.Normal;
                case >1:
                    return TimeState.Accelerated;
            }

            return TimeState.Accelerated;
        }

        /// <summary>
        /// 获取unscaleTime
        /// </summary>
        public static float UnscaledDeltaTime => Time.frameCount <= 2 ? 0.02f : Mathf.Min(Time.unscaledDeltaTime, Time.maximumDeltaTime);

        /// <summary>
        /// 获取TimeHolder
        /// </summary>
        public TimeHolder GetTimeHolder(string key)
        {
            return HasTimeHolder(key) ? _timeHolderList[key] : null;
        }
        
        /// <summary>
        /// 是否有指定TimeHolder
        /// </summary>
        private bool HasTimeHolder(string key)
        {
            // GameLog.Info("find+"+key);
            // foreach (var timeHolder in _timeHolderList)
            // {
            //     GameLog.Info(timeHolder.Value.key);
            // }
            return !string.IsNullOrEmpty(key) && _timeHolderList.ContainsKey(key);
        }
    }
}