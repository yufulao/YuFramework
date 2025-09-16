// ******************************************************************
//@file         Utils.cs
//@brief        通用工具类(Animation部分)
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:18
// ******************************************************************

using System.Collections;
using System.Linq;
using UnityEngine;

namespace Yu
{
    public static partial class Utils
    {
        /// <summary>
        /// 获取animation对应的id
        /// </summary>
        public static int GetAnimationIndex(string animationName)
        {
            if (string.IsNullOrEmpty(animationName))
            {
                return -1;
            }

            return Animator.StringToHash(animationName);
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        public static IEnumerator PlayAnimation(Animator animator, string animationName)
        {
            var hash = Animator.StringToHash(animationName);
            if (!animator || string.IsNullOrEmpty(animationName) || !animator.HasState(0, hash))
            {
                yield break;
            }

            animator.Play(hash, 0, 0f);
            //animator.Play时还没切换到新clip，延迟一帧才切换，返回clip时长时，动态减去已播放时长
            yield return null;
            var remainLength = GetRemainAnimationLength(animator);
            yield return new WaitForSeconds(remainLength);
        }

        /// <summary>
        /// 获取当前播放的动画的剩余时长
        /// </summary>
        public static float GetRemainAnimationLength(Animator animator, int layer = 0)
        {
            var clip = animator.GetCurrentAnimatorStateInfo(layer);
            var timeRemaining = clip.length * (1 - clip.normalizedTime);
            return timeRemaining;
        }
        
        /// <summary>
        /// 获取当前播放的动画的已播放时长
        /// </summary>
        public static float GetPlayedAnimationLength(Animator animator, int layer = 0)
        {
            var clip = animator.GetCurrentAnimatorStateInfo(layer);
            return clip.normalizedTime;
        }

        /// <summary>
        /// 判断当前animator是否正在播放指定的animation
        /// </summary>
        public static bool IsAnimatorPlayingThisAnimation(Animator animator, string animationName, int layerIndex = 0)
        {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return animatorStateInfo.IsName(animationName);
        }
    }
}