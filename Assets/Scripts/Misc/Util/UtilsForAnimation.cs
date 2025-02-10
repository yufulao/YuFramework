// ******************************************************************
//@file         Utils.cs
//@brief        通用工具类(Animation部分)
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:18
// ******************************************************************

using System.Collections;
using UnityEngine;

namespace Yu
{
    public static partial class Utils
    {
        /// <summary>
        /// 获取animation对应的id
        /// </summary>
        /// <returns></returns>
        public static int GetAnimationIndex(string animationName)
        {
            if (string.IsNullOrEmpty(animationName))
            {
                return -1;
            }

            return Animator.StringToHash(animationName);
        }

        /// <summary>
        /// 刷新animator的bool型parameters
        /// </summary>
        public static bool UpdateAnimatorBool(Animator animator, string parameter, bool value)
        {
            if (!CheckAnimatorHasParameter(animator, parameter, AnimatorControllerParameterType.Bool))
            {
                return false;
            }

            animator.SetBool(parameter, value);
            return true;
        }

        /// <summary>
        /// 刷新animator的float型parameters
        /// </summary>
        public static bool UpdateAnimatorFloat(Animator animator, string parameter, float value)
        {
            if (!CheckAnimatorHasParameter(animator, parameter, AnimatorControllerParameterType.Float))
            {
                return false;
            }

            animator.SetFloat(parameter, value);
            return true;
        }

        /// <summary>
        /// 刷新animator的int型parameters
        /// </summary>
        public static bool UpdateAnimatorInteger(Animator animator, string parameter, int value)
        {
            if (!CheckAnimatorHasParameter(animator, parameter, AnimatorControllerParameterType.Int))
            {
                return false;
            }

            animator.SetInteger(parameter, value);
            return true;
        }

        /// <summary>
        /// 触发animator的trigger型parameters
        /// </summary>
        public static bool SetAnimatorTrigger(Animator animator, string parameter)
        {
            if (!CheckAnimatorHasParameter(animator, parameter, AnimatorControllerParameterType.Trigger))
            {
                return false;
            }

            animator.SetTrigger(parameter);
            return true;
        }

        /// <summary>
        /// 检测animator是否存在指定的parameter
        /// </summary>
        public static bool CheckAnimatorHasParameter(Animator animator, string parameter, AnimatorControllerParameterType type)
        {
            foreach (var param in animator.parameters)
            {
                if (param.type == type && param.name.Equals(parameter))
                {
                    return true;
                }
            }

            Debug.LogWarning(animator.gameObject.name + "的animator没有这个参数" + type + "        " + parameter);
            return false;
        }

        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animationName"></param>
        /// <returns></returns>
        public static IEnumerator PlayAnimation(Animator animator, string animationName)
        {
            if (!animator || string.IsNullOrEmpty(animationName) || !animator.HasState(0, Animator.StringToHash(animationName)))
            {
                yield break;
            }

            animator.Play(animationName, 0, 0f);
            yield return new WaitForSeconds(GetAnimatorLength(animator, animationName));
        }

        /// <summary>
        /// 获取动画时长
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static float GetAnimatorLength(Animator animator, string name)
        {
            float length = 0;

            var clips = animator.runtimeAnimatorController.animationClips;
            foreach (var clip in clips)
            {
                if (clip.name.Equals(name))
                {
                    length = clip.length;
                    break;
                }
            }

            return length;
        }

        /// <summary>
        /// 判断当前animator是否正在播放指定的animation
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animationName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public static bool IsAnimatorPlayingThisAnimation(Animator animator, string animationName, int layerIndex = 0)
        {
            var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            return animatorStateInfo.IsName(animationName);
        }
    }
}