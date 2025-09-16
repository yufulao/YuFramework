// ******************************************************************
//@file         ContentSizeFitterPro.cs
//@brief        高级ContentSizeFitter
//@author       yufulao, yufulao@qq.com
//@createTime   2025.05.11 13:57:45 
// ******************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Yu
{
    [RequireComponent(typeof(RectTransform))]
    public class ContentSizeFitterPro : ContentSizeFitter
    {
        public List<RectTransform> rebuildListOnLayoutChanged = new();
        public UnityEvent onLayoutChanged;
        public bool enableMaxWidth;//限制宽度
        public float maxWidth = 1920;//最大宽度
        public bool enableMaxHeight;//限制高度
        public float maxHeight = 1080;//最大高度
        
        private bool _pendingLayoutUpdate;//标志位，防递归
        private RectTransform _rectTransform;

        
        protected override void Awake()
        {
            base.Awake();
            _rectTransform = GetComponent<RectTransform>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            StartCoroutine(DelayRebuildOnStart());
        }

        public override void SetLayoutHorizontal()
        {
            var targetWidth = LayoutUtility.GetPreferredWidth(_rectTransform);
            if (enableMaxWidth)
            {
                targetWidth = Mathf.Min(targetWidth, maxWidth);
            }

            if (Mathf.Approximately(_rectTransform.rect.width, targetWidth))
            {
                return;
            }
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
            OnLayoutChange();
        }
        
        public override void SetLayoutVertical()
        {
            var targetHeight = LayoutUtility.GetPreferredHeight(_rectTransform);
            if (enableMaxHeight)
            {
                targetHeight = Mathf.Min(targetHeight, maxHeight);
            }

            if (Mathf.Approximately(_rectTransform.rect.height, targetHeight))
            {
                return;
            }
            
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, targetHeight);
            OnLayoutChange();
        }
        
        /// <summary>
        /// 初始化时延迟一帧刷新布局
        /// </summary>
        /// <returns></returns>
        protected IEnumerator DelayRebuildOnStart()
        {
            yield return null;
            SetLayoutHorizontal();
            SetLayoutVertical();
            //避免布局未变导致不刷新，强制刷新一次
            OnLayoutChange();
        }

        /// <summary>
        /// 布局变化时
        /// </summary>
        protected void OnLayoutChange()
        {
            if (_pendingLayoutUpdate)
            {
                return;
            }

            _pendingLayoutUpdate = true;
            RebuildOnce();
        }

        /// <summary>
        /// 重绘rect列表
        /// </summary>
        protected void RebuildOnce()
        {
            Canvas.willRenderCanvases -= RebuildOnce;
            _pendingLayoutUpdate = false;
            
            foreach (var rect in rebuildListOnLayoutChanged)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            }
            
            onLayoutChanged?.Invoke();
        }
    }
}