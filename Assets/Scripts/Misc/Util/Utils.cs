// ******************************************************************
//@file         Utils.cs
//@brief        通用工具类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:34:18
// ******************************************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Yu
{
    public static partial class Utils
    {
        /// <summary>
        /// 随机获取count个item，允许重复
        /// </summary>
        public static void GetRandomItems<T>(List<T> source, int count, ref List<T> result)
        {
            if (source == null || source.Count == 0)
            {
                throw new System.ArgumentException("Source list cannot be null or empty.");
            }
            
            result.Clear();
            for (var i = 0; i < count; i++)
            {
                var randomIndex = Random.Range(0, source.Count);
                result.Add(source[randomIndex]);
            }
        }
        
        /// <summary>
        /// 随机执行事件,ratio从0到1
        /// </summary>
        public static void RateToDo(float ratio, Action action)
        {
            if (Random.value < ratio)
            {
                action?.Invoke();
            }
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
        /// 将rectTransform的大小设置为指定的大小
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="newSize"></param>
        public static void SetSize(RectTransform rectTransform, Vector2 newSize) 
        {
            var currSize = rectTransform.rect.size;
            var sizeDiff = newSize - currSize;
            var pivot = rectTransform.pivot;
            rectTransform.offsetMin -= new Vector2(sizeDiff.x * pivot.x, sizeDiff.y * pivot.y);
            var pivot1 = rectTransform.pivot;
            rectTransform.offsetMax += new Vector2(sizeDiff.x * (1.0f - pivot1.x), sizeDiff.y * (1.0f - pivot1.y));
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

        /// <summary>
        /// 以2D形式绘制调试光线并执行实际光线投射
        /// </summary>
        /// <returns>The raycast hit.</returns>
        /// <param name="rayOriginPoint">Ray origin point.</param>
        /// <param name="rayDirection">Ray direction.</param>
        /// <param name="rayDistance">Ray distance.</param>
        /// <param name="mask">Mask.</param>
        /// <param name="color">Color.</param>
        /// <param name="drawGizmo"></param>
        public static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false)
        {
            if (drawGizmo)
            {
                Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
            }

            return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
        }
    }
}