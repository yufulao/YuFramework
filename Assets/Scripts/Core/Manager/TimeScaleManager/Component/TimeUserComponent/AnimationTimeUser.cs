// ******************************************************************
//@file         AnimationTimeUser.cs
//@brief        timeUser封装的animation组件
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 19:51:51
// ******************************************************************

using System;
using UnityEngine;

namespace Yu
{
    public class AnimationTimeUser: ComponentTimeUser<Animation>
    {
        public AnimationTimeUser(TimeUser timeUser, Animation component) : base(timeUser, component) { }

        private float _speed;

        protected override void CopyProperties(Animation source)
        {
            float firstAnimationStateSpeed = 1;
            var found = false;
            foreach (AnimationState animationState in source)
            {
                if (found && Math.Abs(firstAnimationStateSpeed - animationState.speed) > 0f)
                {
                    Debug.LogWarning("不支持每个状态的不同动画速度");
                }

                firstAnimationStateSpeed = animationState.speed;
                found = true;
            }

            _speed = firstAnimationStateSpeed;
        }

        protected override void AdjustProperties(float timeScale)
        {
            foreach (AnimationState state in Component)
            {
                state.speed = _speed * timeScale;
            }
        }

        /// <summary>
        /// 设置速度
        /// </summary>
        /// <returns></returns>
        public void SetSpeed(float value)
        {
            _speed = value;
            AdjustProperties();
        }
    }
}