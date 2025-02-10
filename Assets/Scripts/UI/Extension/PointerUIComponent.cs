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
    public class PointerUIComponent : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        private UnityAction<PointerEventData> _onPointerEnter;
        private UnityAction<PointerEventData> _onPointerExit;
        
        
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnter?.Invoke(eventData);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            _onPointerExit?.Invoke(eventData);
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        public void BindEvent(UnityAction<PointerEventData> onPointerEnter, UnityAction<PointerEventData> onPointerExit)
        {
            _onPointerEnter = onPointerEnter;
            _onPointerExit = onPointerExit;
        }
    }
}