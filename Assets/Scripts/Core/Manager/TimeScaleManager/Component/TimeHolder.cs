// ******************************************************************
//@file         TimeHolder.cs
//@brief        时钟基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.20 15:01:34
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;

namespace Yu
{
    public class TimeHolder : MonoBehaviour
    {
        private readonly HashSet<TimeHolder> _timeHolderList = new();
        private readonly HashSet<TimeUser> _timeUserList = new();
        public string key;
        [SerializeField] private string parentKey;
        public float localTimeScale = 1;
        public float TimeScale { get; private set; }
        public float Time { get; private set; } //自时钟创建以来的时间（以秒为单位），受时间刻度的影响
        public float UnscaledTime { get; private set; } //与时间刻度无关
        public float DeltaTime { get; private set; } // 完成最后一帧所花费的时间（以秒为单位）乘以时间刻度。
        public float FixedDeltaTime { get; private set; } // 物理场和其他固定帧速率更新的时间间隔（以秒为单位）乘以时间刻度。
        public float StartTime { get; private set; } // 从游戏开始到时钟创建之间的未缩放时间（以秒为单位）。
        public bool paused;
        public TimeHolder Parent { get; private set; }
        public TimeBlendMode parentBlend = TimeBlendMode.Multiplicative;
        private bool _isLerp;
        private float _lerpStart;
        private float _lerpEnd;
        private float _lerpFrom;
        private float _lerpTo;


        private void Start()
        {
            OnInit();
        }

        private void Update()
        {
            if (_isLerp)
            {
                localTimeScale = Mathf.Lerp(_lerpFrom, _lerpTo, (UnscaledTime - _lerpStart) / (_lerpEnd - _lerpStart));
                if (UnscaledTime >= _lerpEnd)
                {
                    _isLerp = false;
                }
            }

            CalculateTimeScale();
            var unscaledDeltaTime = TimeScaleManager.UnscaledDeltaTime;
            DeltaTime = unscaledDeltaTime * TimeScale;
            FixedDeltaTime = UnityEngine.Time.fixedDeltaTime * TimeScale;
            Time += DeltaTime;
            UnscaledTime += unscaledDeltaTime;
        }

        private void OnDisable()
        {
            if (Parent != null)
            {
                Parent.Unregister(this);
            }
        }

        /// <summary>
        /// 手动设置父TimeHolder
        /// </summary>
        public void SetParent(TimeHolder timeHolder)
        {
            if (Parent != null)
            {
                Parent.Unregister(this);
            }

            if (timeHolder != null)
            {
                if (timeHolder == this)
                {
                    GameLog.Error("父TimeHolder不能是自己");
                    return;
                }

                parentKey = timeHolder.key;
                Parent = timeHolder;
                Parent.Register(this);
                return;
            }

            parentKey = null;
            Parent = null;
        }

        /// <summary>
        /// timeUser绑定自己
        /// </summary>
        public void Register(TimeUser timeUser)
        {
            if (!timeUser)
            {
                GameLog.Error("timeUser为空");
                return;
            }

            _timeUserList.Add(timeUser);
        }

        /// <summary>
        /// 其他holder绑定自己为父holder
        /// </summary>
        public void Register(TimeHolder timeHolder)
        {
            if (!timeHolder)
            {
                GameLog.Error("timeHolder为空");
                return;
            }

            _timeHolderList.Add(timeHolder);
        }

        /// <summary>
        /// timeUser解除绑定
        /// </summary>
        public void Unregister(TimeUser timeUser)
        {
            if (!timeUser)
            {
                GameLog.Error("timeUser为空");
                return;
            }

            _timeUserList.Remove(timeUser);
        }

        /// <summary>
        /// timeHolder解除绑定
        /// </summary>
        public void Unregister(TimeHolder timeHolder)
        {
            if (!timeHolder)
            {
                GameLog.Error("timeHolder为空");
                return;
            }

            _timeHolderList.Remove(timeHolder);
        }

        /// <summary>
        /// 计算TimeScale
        /// </summary>
        public void CalculateTimeScale()
        {
            if (paused)
            {
                TimeScale = 0;
                return;
            }

            if (!Parent)
            {
                TimeScale = localTimeScale;
                return;
            }

            switch (parentBlend)
            {
                case TimeBlendMode.Multiplicative:
                    TimeScale = Parent.TimeScale * localTimeScale;
                    return;
                case TimeBlendMode.Additive:
                    TimeScale = Parent.TimeScale + localTimeScale;
                    break;
                default:
                    GameLog.Error("没有处理这个时间混合模式" + parentBlend);
                    return;
            }
        }

        /// <summary>
        /// 在给定的持续时间内平滑地更改本地时间刻度（以秒为单位）
        /// </summary>
        public void LerpTimeScale(float cacheTimeScale, float duration, bool steady = false)
        {
            switch (duration)
            {
                case < 0:
                    GameLog.Error("duration必须为正数");
                    break;
                case 0:
                    localTimeScale = cacheTimeScale;
                    _isLerp = false;
                    return;
            }

            float modifier = 1;
            if (steady)
            {
                modifier = Mathf.Abs(localTimeScale - cacheTimeScale);
            }

            if (modifier == 0)
            {
                return;
            }

            _lerpFrom = localTimeScale;
            _lerpStart = UnscaledTime;
            _lerpTo = cacheTimeScale;
            _lerpEnd = _lerpStart + (duration * modifier);
            _isLerp = true;
        }

        /// <summary>
        /// 初始化，Start调用
        /// </summary>
        private void OnInit()
        {
            if (string.IsNullOrEmpty(parentKey))
            {
                Parent = null;
                return;
            }

            Parent = TimeScaleManager.Instance.GetTimeHolder(parentKey);
            if (Parent)
            {
                return;
            }

            GameLog.Error("找不到父TimeHolder" + parentKey);
            StartTime = UnityEngine.Time.unscaledTime;
            if (Parent != null)
            {
                Parent.Register(this);
                Parent.CalculateTimeScale();
            }

            CalculateTimeScale();
        }
    }
}