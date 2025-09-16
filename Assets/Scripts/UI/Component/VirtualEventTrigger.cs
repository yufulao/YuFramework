// ******************************************************************
//@file         VirtualEventTrigger.cs
//@brief        不依赖父子关系的ui悬停范围真合并EventTrigger
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.26 14:07:15
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Yu
{
    public class VirtualEventTrigger : EventTrigger
    {
        //生命周期：EventSystem:Exit()-> hover.remove()-> (pointerEnter=pointerCurrentRaycast.gameObject)-> Enter()-> hover.add()

        public bool isParent; //自己是不是父（需要双向绑定，因为是虚拟父子关系，绑了虚拟父也要绑虚拟子，下同）
        public bool isChild; //自己是不是子
        public List<GameObject> childVirtualList = new(); //自己是父时的虚拟子列表
        public GameObject parentVirtual; //自己是子时的虚拟父
        private Func<PointerEventData, bool> _customFuncEnter; //自定义PointerEnter检测
        private Func<PointerEventData, bool> _customFuncExit; //自定义PointerExit检测
        private static readonly Dictionary<Transform, GameObject> CacheVirtualParentDict = new(); //虚拟父子链中各parentVirtual的缓存，全局共用一份
        private static PointerEventData _cachePointerEventData; //debug用


        public override void OnPointerEnter(PointerEventData pointerEventData)
        {
            _cachePointerEventData = pointerEventData;

            if (isParent)
            {
                if (isChild)
                {
                    OnPointerEnterBoth(pointerEventData);
                    return;
                }

                OnPointerEnterParent(pointerEventData);
                return;
            }

            if (isChild)
            {
                OnPointerEnterChild(pointerEventData);
                return;
            }

            base.OnPointerEnter(pointerEventData);
        }

        public override void OnPointerExit(PointerEventData pointerEventData)
        {
            if (isParent)
            {
                if (isChild)
                {
                    OnPointerExitBoth(pointerEventData);
                    return;
                }

                OnPointerExitParent(pointerEventData);
                return;
            }

            if (isChild)
            {
                OnPointerExitChild(pointerEventData);
                return;
            }

            base.OnPointerExit(pointerEventData);
        }

        /// <summary>
        /// 输出当前鼠标悬停的hovered日志
        /// </summary>
        public static void LogHovered()
        {
            if (_cachePointerEventData != null)
            {
                GameLog.Info($"LOG_HOVERED: {Utils.ToStringCollection(_cachePointerEventData.hovered, obj => obj.name)}");
            }
        }

        /// <summary>
        /// 设置自定义PointerEnter检测
        /// </summary>
        public void SetCustomFuncEnter(Func<PointerEventData, bool> customFuncEnter)
        {
            _customFuncEnter = customFuncEnter;
        }

        /// <summary>
        /// 设置自定义PointerExit检测
        /// </summary>
        public void SetCustomFuncExit(Func<PointerEventData, bool> customFuncExit)
        {
            _customFuncExit = customFuncExit;
        }

        /// <summary>
        /// both的OnPointerEnter
        /// </summary>
        private void OnPointerEnterBoth(PointerEventData pointerEventData)
        {
            // GameLog.Info($"PreEnter_Both: {gameObject.name}\ntarget: {pointerEventData.pointerEnter?.name}");
            var hadMe = pointerEventData.hovered.Contains(gameObject);
            FixHoverForOnlyMe(pointerEventData);
            FixHoveredAddParent(pointerEventData);
            if (hadMe)
            {
                return;
            }

            // GameLog.Info($"Enter_Both: {gameObject.name}, hover:" + Utils.ToStringCollection(pointerEventData.hovered, obj => obj.name));
            ExecutePointerEnter(pointerEventData);
        }

        /// <summary>
        /// both的OnPointerExit
        /// </summary>
        private void OnPointerExitBoth(PointerEventData pointerEventData)
        {
            // GameLog.Info($"PreExit_Both: {gameObject.name}\ntarget: {pointerEventData.pointerCurrentRaycast.gameObject?.name}");
            var current = pointerEventData.pointerCurrentRaycast.gameObject;
            if (IsRootOrBothRoot(pointerEventData, current)) //阻止从自己进入非同级Child时的自己Exit()派发
            {
                return;
            }

            ExitObjBetweenVirtualParentAndCurrent(pointerEventData);
            // GameLog.Info($"Exit_Both: {gameObject.name}, hover:" + Utils.ToStringCollection(pointerEventData.hovered, obj => obj.name));
            ExecutePointerExit(pointerEventData);
        }

        /// <summary>
        /// parent的OnPointerEnter
        /// </summary>
        private void OnPointerEnterParent(PointerEventData pointerEventData)
        {
            // GameLog.Info($"PreEnter_Parent: {gameObject.name}\ntarget: {pointerEventData.pointerEnter?.name}");
            var hadMe = pointerEventData.hovered.Contains(gameObject);
            FixHoverForOnlyMe(pointerEventData);
            if (hadMe)
            {
                return;
            }

            // GameLog.Info($"Enter_Parent: {gameObject.name}, hover:" + Utils.ToStringCollection(pointerEventData.hovered, obj => obj.name));
            ExecutePointerEnter(pointerEventData);
        }

        /// <summary>
        /// parent的OnPointerExit
        /// </summary>
        private void OnPointerExitParent(PointerEventData pointerEventData)
        {
            // GameLog.Info($"PreExit_Parent: {gameObject.name}\ntarget: {pointerEventData.pointerCurrentRaycast.gameObject?.name}");
            var current = pointerEventData.pointerCurrentRaycast.gameObject;
            if (IsRootOrChildRoot(current)) //阻止从自己进入非同级Child时的自己Exit()派发
            {
                pointerEventData.hovered.Add(gameObject); //将Exit()事件加进Child的Exit()
                return;
            }

            // GameLog.Info($"Exit_Parent: {gameObject.name}, hover:" + Utils.ToStringCollection(pointerEventData.hovered, obj => obj.name));
            ExecutePointerExit(pointerEventData);
        }

        /// <summary>
        /// Child的OnPointerEnter
        /// </summary>
        private void OnPointerEnterChild(PointerEventData pointerEventData)
        {
            // GameLog.Info($"PreEnter_Child: {gameObject.name}\ntarget: {pointerEventData.pointerEnter?.name}");
            var hadMe = pointerEventData.hovered.Contains(gameObject);
            FixHoveredAddParent(pointerEventData);
            if (hadMe)
            {
                return;
            }

            // GameLog.Info($"Enter_Child: {gameObject.name}, hover:" + Utils.ToStringCollection(pointerEventData.hovered, obj => obj.name));
            ExecutePointerEnter(pointerEventData);
        }

        /// <summary>
        /// child的OnPointerExit
        /// </summary>
        private void OnPointerExitChild(PointerEventData pointerEventData)
        {
            // GameLog.Info($"PreExit_Child: {gameObject.name}\ntarget: {pointerEventData.pointerCurrentRaycast.gameObject?.name}");
            var current = pointerEventData.pointerCurrentRaycast.gameObject;
            if (IsRootOrParentRoot(current)) //阻止从自己进入非同级Child时的自己Exit()派发
            {
                return;
            }

            ExitObjBetweenVirtualParentAndCurrent(pointerEventData);
            // GameLog.Info($"Exit_Child: {gameObject.name}, hover:" + Utils.ToStringCollection(pointerEventData.hovered, obj => obj.name));
            ExecutePointerExit(pointerEventData);
        }

        /// <summary>
        /// 进入parent后，移除hover重复的自己（不需要add来保留一个自己，Enter后会自动add）
        /// </summary>
        private void FixHoverForOnlyMe(PointerEventData pointerEventData)
        {
            pointerEventData.hovered.RemoveAll(obj => obj == gameObject);
        }

        /// <summary>
        /// 进入child后，保证虚拟parent也加进hovered中
        /// </summary>
        private void FixHoveredAddParent(PointerEventData pointerEventData)
        {
            if (pointerEventData.hovered.Contains(parentVirtual))
            {
                return;
            }

            for (var current = parentVirtual.transform; current; current = current.parent)
            {
                if (pointerEventData.hovered.Contains(parentVirtual))
                {
                    continue;
                }

                ExecuteEvents.Execute(current.gameObject, pointerEventData, ExecuteEvents.pointerEnterHandler);
                pointerEventData.hovered.Add(current.gameObject);
            }
        }

        /// <summary>
        /// both的虚实父子链检测
        /// </summary>
        private bool IsRootOrBothRoot(PointerEventData pointerEventData, GameObject current)
        {
            var result = false;
            if (IsRootOrChildRoot(current)) //阻止从自己进入非同级Child时的自己Exit()派发
            {
                pointerEventData.hovered.Add(gameObject); //将Exit()事件加进Child的Exit()
                result = true;
            }

            if (IsRootOrParentRoot(current)) //阻止从自己进入非同级Child时的自己Exit()派发
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// parent的虚实父子链检测
        /// </summary>
        private bool IsRootOrChildRoot(GameObject obj)
        {
            if (!obj)
            {
                return false;
            }

            if (IsRoot(gameObject, obj))
            {
                return true;
            }

            foreach (var child in childVirtualList)
            {
                if (IsRoot(child, obj))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// child的虚实父子链检测
        /// </summary>
        private bool IsRootOrParentRoot(GameObject obj)
        {
            if (!obj)
            {
                return false;
            }

            if (IsRoot(gameObject, obj))
            {
                return true;
            }

            for (var current = obj.transform; current; current = current.parent)
            {
                if (IsRoot(gameObject, current.gameObject))
                {
                    return true;
                }

                if (!CacheVirtualParentDict.TryGetValue(current, out var virtualParent))
                {
                    var compVirtual = current.GetComponent<VirtualEventTrigger>();
                    virtualParent = !compVirtual || !compVirtual.isChild || !compVirtual.parentVirtual ? null : compVirtual.parentVirtual;
                    CacheVirtualParentDict.Add(current, virtualParent);
                }

                if (IsRootOrParentRoot(virtualParent))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 与InputSystemUIInputModule一样，exit掉当前指针ui与（当前指针ui与虚拟父的）共同父之间的obj
        /// </summary>
        private void ExitObjBetweenVirtualParentAndCurrent(PointerEventData pointerEventData)
        {
            var current = pointerEventData.pointerCurrentRaycast.gameObject;
            if (!current)
            {
                return;
            }

            //把current和虚拟parent的中间层给exit掉
            var commonVirtualRoot = FindCommonRoot(parentVirtual.transform, current.transform);
            // GameLog.Info(parentVirtual.name);
            // GameLog.Info(current.name);
            // GameLog.Info(commonVirtualRoot.gameObject.name);
            // GameLog.Info(parentVirtual.transform != null);
            // GameLog.Info(parentVirtual.transform != commonVirtualRoot);
            for (var tCurrent = parentVirtual.transform; tCurrent && tCurrent != commonVirtualRoot; tCurrent = tCurrent.parent)
            {
                // GameLog.Info(tCurrent.gameObject.name);
                if (!pointerEventData.hovered.Contains(tCurrent.gameObject))
                {
                    // GameLog.Info("remove");
                    continue;
                }

                ExecuteEvents.Execute(tCurrent.gameObject, pointerEventData, ExecuteEvents.pointerExitHandler);
                pointerEventData.hovered.Remove(tCurrent.gameObject);
            }
        }

        /// <summary>
        /// 往上查找相同的transform
        /// </summary>
        private static Transform FindCommonRoot(Transform t1, Transform t2)
        {
            var cacheT2 = t2;
            while (t1)
            {
                t2 = cacheT2;
                while (t2)
                {
                    if (t1 == t2)
                    {
                        return t1;
                    }

                    t2 = t2.parent;
                }

                t1 = t1.parent;
            }

            return null;
        }

        /// <summary>
        /// obj是不是root的真实子节点
        /// </summary>
        private static bool IsRoot(GameObject root, GameObject obj)
        {
            var tRoot = root.transform;
            var tObj = obj.transform;
            while (tObj)
            {
                if (tObj == tRoot)
                {
                    return true;
                }

                tObj = tObj.parent;
            }

            return false;
        }

        /// <summary>
        /// 执行PointerEnter的Entry
        /// </summary>
        private void ExecutePointerEnter(PointerEventData pointerEventData)
        {
            //自定义EnterFunc注入
            if (_customFuncEnter != null && !_customFuncEnter(pointerEventData))
            {
                return;
            }

            PublicExecute(EventTriggerType.PointerEnter, pointerEventData);
        }

        /// <summary>
        /// 执行PointerExit的Entry
        /// </summary>
        private void ExecutePointerExit(PointerEventData pointerEventData)
        {
            //自定义ExitFunc注入
            if (_customFuncExit != null && !_customFuncExit(pointerEventData))
            {
                return;
            }

            PublicExecute(EventTriggerType.PointerExit, pointerEventData);
        }

        /// <summary>
        /// EventTrigger的Execute为private，需要手动转发事件
        /// </summary>
        private void PublicExecute(EventTriggerType id, BaseEventData eventData)
        {
            foreach (var ent in triggers.Where(ent => ent.eventID == id && ent.callback != null))
            {
                ent.callback.Invoke(eventData);
            }
        }
    }
}