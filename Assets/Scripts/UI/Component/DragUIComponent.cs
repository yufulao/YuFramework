// ******************************************************************
//@file         DragUIComponent.cs
//@brief        UI拖拽化组件
//@author       yufulao, yufulao@qq.com
//@createTime   2023.12.27 03:37:41
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Yu
{
    public enum DragDirection
    {
        Horizontal, //水平拖拽
        Vertical, //垂直拖拽
        Both //水平和垂直都检测
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class DragUIComponent : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public float validDragDistance = 1f; //有效拖动的最小滑动距离
        public float validDragAngle = 30f; //与检测方向的角度大于时才判定有效
        public DragDirection dragDirection; // 控制拖拽方向

        [HideInInspector] public GameObject cacheObjOnDrag; //移动过程中复制的一份物体
        private RectTransform _dragRectTransform;
        private Graphic _graphic;
        private CanvasGroup _canvasGroup;
        private Func<GameObject, bool> _filter;
        private Action<List<GameObject>, bool> _onEndDrag;
        private Action<PointerEventData> _onBeginDrag;
        private Action<PointerEventData> _onDrag;
        private Action<PointerEventData> _invalidBeginDragDispatch; //无效拖拽开始时转发的事件
        private Action<PointerEventData> _invalidOnDragDispatch; //无效拖拽中转发的事件
        private Action<PointerEventData> _invalidEndDragDispatch; //无效拖拽结束时转发的事件
        private Vector2 _originalPos; //开始拖拽的position
        private bool _isValidDragging; //当前dragging是否是有效拖拽
        private bool _hadSetValidDrag; //是否已经设置了有效拖拽
        private bool _checkValidDrag; //是否检测无效拖拽
        private readonly List<GameObject> _filteredResults = new(); //过滤UI穿透的缓存列表


        /// <summary>
        /// 初始化拖拽组件，需要手动初始化
        /// </summary>
        public void InitDragComponent(Transform onDragParent, bool checkValidDrag)
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            _graphic = GetComponent<Graphic>();
            cacheObjOnDrag = Instantiate(this.gameObject, onDragParent);
            Destroy(cacheObjOnDrag.GetComponent<DragUIComponent>());
            cacheObjOnDrag.SetActive(false);
            _dragRectTransform = cacheObjOnDrag.GetComponent<RectTransform>();
            _checkValidDrag = checkValidDrag;
        }

        /// <summary>
        /// 设置拖拽的事件，包括过滤器，开始拖拽，拖拽结束
        /// </summary>
        /// <param name="filter">拖拽事件参数的目标</param>
        /// <param name="onBeginDrag">拖拽开始事件</param>
        /// <param name="onDrag">拖拽中事件</param>
        /// <param name="onEndDrag">拖拽结束事件</param>
        public void SetDragAction(Func<GameObject, bool> filter, Action<PointerEventData> onBeginDrag = null, Action<PointerEventData> onDrag = null, Action<List<GameObject>, bool> onEndDrag = null)
        {
            _filter = filter;
            _onBeginDrag = onBeginDrag;
            _onDrag = onDrag;
            _onEndDrag = onEndDrag;
        }

        /// <summary>
        /// 设置当前是否是有效拖拽，（有效拖拽即拖拽的是目标ui）
        /// </summary>
        /// <param name="invalidBeginDragDispatch">非法拖拽开始时</param>
        /// <param name="invalidOnDragDispatch">非法拖拽中</param>
        /// <param name="invalidEndDragDispatch">非法拖拽结束时</param>
        public void SetInvalidDragAction(Action<PointerEventData> invalidBeginDragDispatch = null, Action<PointerEventData> invalidOnDragDispatch = null,
            Action<PointerEventData> invalidEndDragDispatch = null)
        {
            _invalidBeginDragDispatch = invalidBeginDragDispatch;
            _invalidOnDragDispatch = invalidOnDragDispatch;
            _invalidEndDragDispatch = invalidEndDragDispatch;
        }

        /// <summary>
        /// 开始拖拽时
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_graphic)
            {
                _graphic.raycastTarget = false;
            }

            _originalPos = eventData.position;
            _isValidDragging = true;
            _hadSetValidDrag = false;
            _onBeginDrag?.Invoke(eventData);
        }

        /// <summary>
        /// 拖拽过程中
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            //最小输入
            if (!_hadSetValidDrag)
            {
                if (!(Vector2.Distance(eventData.position, _originalPos) > validDragDistance))
                {
                    return;
                }

                SetValidDragging(eventData); //设置本次拖拽事件，是否是无效拖拽
                _hadSetValidDrag = true;
                return;
            }

            if (!_isValidDragging)
            {
                _invalidOnDragDispatch?.Invoke(eventData);
                return;
            }

            //isValidDrag
            if (!cacheObjOnDrag.activeInHierarchy)
            {
                cacheObjOnDrag.SetActive(true);
                _canvasGroup.alpha = 0f;
            }

            _dragRectTransform.position = eventData.position;
            _onDrag?.Invoke(eventData);
        }

        /// <summary>
        /// 拖拽结束时
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isValidDragging)
            {
                _invalidEndDragDispatch?.Invoke(eventData);
                return;
            }

            //正常的valid的endDrag，相对的是OnChangeInvalidDrag
            if (_graphic)
            {
                _graphic.raycastTarget = true;
            }

            //转发事件
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = eventData.position
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            cacheObjOnDrag.SetActive(false);
            _canvasGroup.alpha = 1f;
            _onEndDrag?.Invoke(Filtrate(results), true); //true指的是，是validDrag
        }

        /// <summary>
        /// 设置本次拖拽事件，是否是无效拖拽
        /// </summary>
        private void SetValidDragging(PointerEventData eventData)
        {
            if (!_checkValidDrag)
            {
                return;
            }

            //偏移向量
            var direction = eventData.position - _originalPos;
            //获取与X轴和Y轴的夹角
            var angleX = Mathf.Atan2(Mathf.Abs(direction.x), Mathf.Abs(direction.y)) * Mathf.Rad2Deg;
            var angleY = 90f - angleX;
            switch (dragDirection)
            {
                case DragDirection.Horizontal:
                    SetValidDraggingHorizontal(angleX, eventData);
                    break;
                case DragDirection.Vertical:
                    SetValidDraggingVertical(angleY, eventData);
                    break;
                case DragDirection.Both:
                    SetValidDraggingBoth(angleX, angleY, eventData);
                    break;
            }
        }

        /// <summary>
        /// 检测水平是否是无效拖拽
        /// </summary>
        private void SetValidDraggingHorizontal(float angleX, PointerEventData eventData)
        {
            if (angleX < validDragAngle)
            {
                return;
            }

            _invalidBeginDragDispatch?.Invoke(eventData);
            OnChangeInvalidDrag(eventData);
            _isValidDragging = false;
        }

        /// <summary>
        /// 检测垂直是否是无效拖拽
        /// </summary>
        private void SetValidDraggingVertical(float angleY, PointerEventData eventData)
        {
            if (angleY < validDragAngle)
            {
                return;
            }

            _invalidBeginDragDispatch?.Invoke(eventData);
            OnChangeInvalidDrag(eventData);
            _isValidDragging = false;
        }

        /// <summary>
        /// 检测垂直和水平是否是无效拖拽
        /// </summary>
        private void SetValidDraggingBoth(float angleX, float angleY, PointerEventData eventData)
        {
            if (!(angleX >= validDragAngle) || !(angleY >= validDragAngle))
            {
                return;
            }

            _invalidBeginDragDispatch?.Invoke(eventData);
            OnChangeInvalidDrag(eventData);
            _isValidDragging = false;
        }

        /// <summary>
        /// 切换为invalidDrag的时候需要做的事，执行了这个就不会执行正常的validOnEndDrag
        /// </summary>
        private void OnChangeInvalidDrag(PointerEventData eventData)
        {
            if (_graphic)
            {
                _graphic.raycastTarget = true;
            }

            //转发事件
            var pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = eventData.position
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            cacheObjOnDrag.SetActive(false);
            _canvasGroup.alpha = 1f;
            _onEndDrag?.Invoke(Filtrate(results), false); //false指 不是validDrag
        }

        /// <summary>
        /// 结束拖拽时，过滤不需要的ui穿透的结果
        /// </summary>
        private List<GameObject> Filtrate(List<RaycastResult> results)
        {
            // foreach (var _ in results)
            // {
            //     GameLog.Info(_.gameObject.name);
            // }
            //
            //移除当前拖拽的UI
            results.RemoveAll(result => result.gameObject == gameObject);
            //移除当前拖拽的UI的所有子ui
            results.RemoveAll(result => IsChildOf(result.gameObject, gameObject));

            _filteredResults.Clear();
            foreach (var result in results)
            {
                if (_filter(result.gameObject))
                {
                    _filteredResults.Add(result.gameObject); // 只添加符合条件的对象
                }
            }

            return _filteredResults;
        }

        /// <summary>
        /// 检查子物体
        /// </summary>
        private static bool IsChildOf(GameObject child, GameObject parent)
        {
            var trans = child.transform;
            while (trans)
            {
                if (trans.gameObject == parent)
                    return true;
                trans = trans.parent;
            }

            return false;
        }
    }
}