// ******************************************************************
//@file         Utils.cs
//@brief        通用工具类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:18
// ******************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Yu
{
    public static class Utils
    {
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

        /// <summary>
        /// 添加eventTrigger的事件
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="eventID"></param>
        /// <param name="callback"></param>
        public static void AddTriggersListener(EventTrigger trigger, EventTriggerType eventID, UnityAction<BaseEventData> callback)
        {
            if (trigger.triggers.Count == 0)
            {
                trigger.triggers = new List<EventTrigger.Entry>();
            }

            var entry = new EventTrigger.Entry()
            {
                eventID = eventID,
            };
            entry.callback.AddListener(callback);
            trigger.triggers.Add(entry);
        }

        /// <summary>
        /// 强制更新ui的layout
        /// </summary>
        /// <param name="rootTransform"></param>
        public static void ForceUpdateContentSizeFilter(Transform rootTransform)
        {
            foreach (var child in rootTransform.GetComponentsInChildren<ContentSizeFitter>(true))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(child.GetComponent<RectTransform>());
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(rootTransform.GetComponent<RectTransform>());
        }

        /// <summary>
        /// 选择该UI，并将其显示为选中状态。
        /// </summary>
        /// <param name="selectable"></param>
        /// <param name="allowStealFocus"></param>
        public static void Select(UnityEngine.UI.Selectable selectable, bool allowStealFocus = true)
        {
            var currentEventSystem = UnityEngine.EventSystems.EventSystem.current;
            if (currentEventSystem == null || selectable == null) return;
            if (currentEventSystem.alreadySelecting) return;
            if (currentEventSystem.currentSelectedGameObject != null && !allowStealFocus)
            {
                return;
            }

            currentEventSystem.SetSelectedGameObject(selectable.gameObject);
            selectable.Select();
            selectable.OnSelect(null);
        }

        /// <summary>
        /// 飘字效果
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="originalPosition"></param>
        /// <param name="yOffset"></param>
        public static Sequence TextFly(Graphic graphic, Vector3 originalPosition, float yOffset)
        {
            var transform = graphic.transform;
            var originalColor = graphic.color;

            transform.position = originalPosition;
            //Debug.Log(rectTransform.localPosition);
            originalColor.a = 0;
            graphic.color = originalColor;

            var textSequence = DOTween.Sequence().SetAutoKill(true);
            textSequence.Append(transform.DOMoveY(transform.position.y + yOffset, 0.5f));
            textSequence.Join(graphic.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 1), 0.5f));
            textSequence.AppendInterval(0.5f);
            textSequence.Append(transform.DOMoveY(transform.position.y + yOffset, 0.5f));
            textSequence.Join(graphic.DOColor(new Color(originalColor.r, originalColor.g, originalColor.b, 0), 0.5f));
            return textSequence;
        }

        /// <summary>
        /// 在场景中生成文字
        /// </summary>
        /// <param name="text">文字内容</param>
        /// <param name="parent">文字父物体</param>
        /// <param name="localPosition">文字相对父物体的偏移</param>
        /// <param name="localScale">文字缩放</param>
        /// <param name="fontSize">文字大小</param>
        /// <param name="color">文字颜色</param>
        /// <param name="textAnchor">文字锚点</param>
        /// <param name="textAlignment">文字对齐方式</param>
        /// <param name="sortingOrder">文字显示图层</param>
        /// <returns></returns>
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), Vector3 localScale = default, int fontSize = 40, Color? color = null,
            TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 0)
        {
            color ??= Color.white;
            var gameObject = new GameObject("World_Text", typeof(TextMesh));
            var transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            var textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = (Color) color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }


        /// <summary>
        /// 根据名称找到第一个组件（可以找到未激活的物体）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T FindTByName<T>(string str) where T : Component
        {
            var all = Resources.FindObjectsOfTypeAll<T>();
            foreach (var item in all)
            {
                if (item.gameObject.name == str)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(string str)
        {
            var arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}