 // ******************************************************************
//@file         PointerUIComponent.cs
//@brief        鼠标进入离开事件的ui组件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.09 18:42:56
// ******************************************************************
 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Yu
{
    /// <summary>
    /// 在处理多种鼠标事件如点击、进入离开、拖拽时，不能使用EventTrigger，会阻断IPointer接口的事件，导致所有IPointer接口事件失效
    /// </summary>
    public class PointerUIComponent : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private UnityAction<PointerEventData> _onPointerEnter;
        private UnityAction<PointerEventData> _onPointerExit;
        public UnityAction onPointerDown;
        private UnityAction<PointerEventData> _onPointerUp;

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            _onPointerUp?.Invoke(eventData);
        }
        
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExit?.Invoke(eventData);
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnter?.Invoke(eventData);
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        public void BindEvent(UnityAction<PointerEventData> onPointerEnter, UnityAction<PointerEventData> onPointerExit, UnityAction onPointerDown, UnityAction<PointerEventData> onPointerUp)
        {
            _onPointerEnter = onPointerEnter;
            _onPointerExit = onPointerExit;
            this.onPointerDown = onPointerDown;
            _onPointerUp = onPointerUp;
        }
    }
}