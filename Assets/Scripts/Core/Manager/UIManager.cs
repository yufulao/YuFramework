// ******************************************************************
//@file         UIManager.cs
//@brief        C#层UI系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:31:09
// ******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using Object = UnityEngine.Object;

namespace Yu
{
    public class UIManager : BaseSingleTon<UIManager>, IMonoManager
    {
        private LuaTable _uiManager;
        
        private CfgUI _cfgUI;
        private Transform _uiRoot;
        private Dictionary<string, Transform> _layers;
        private Dictionary<string, UICtrlBase> _allViews;
        private Dictionary<string, Stack<UICtrlBase>> _layerStacks;


        public void OnInit()
        {
            _uiManager = LuaManager.Instance.LuaEnv.Global.Get<LuaTable>("UIManager");

            _cfgUI = ConfigManager.Tables.CfgUI;
            _uiRoot = GameObject.Find("UIRoot").transform;
            _layers = new Dictionary<string, Transform>
            {
                {"SceneLayer", GameObject.Find("SceneLayer").transform},
                {"NormalLayer", GameObject.Find("NormalLayer").transform},
                {"TopLayer", GameObject.Find("TopLayer").transform}
            };
            
            _layerStacks = new Dictionary<string, Stack<UICtrlBase>>
            {
                {"SceneLayer", new Stack<UICtrlBase>()},
                {"NormalLayer", new Stack<UICtrlBase>()},
                {"TopLayer", new Stack<UICtrlBase>()}
            };
            
            _allViews = new Dictionary<string, UICtrlBase>();
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }

        // /// <summary>
        // /// 获取uiRoot
        // /// </summary>
        // /// <returns></returns>
        // public Transform GetUIRoot()
        // {
        //     return _uiRoot;
        // }

        /// <summary>
        /// 打开页面
        /// </summary>
        public void OpenWindow(string windowName,bool byLua, params object[] param)
        {
            if (byLua)
            {
                var funcOpenWindow = _uiManager.Get<Action<LuaTable, string>>("OpenWindow");
                funcOpenWindow?.Invoke(_uiManager, windowName);
                return;
            }

            var ctrl = GetCtrl<UICtrlBase>(windowName, param);
            _layerStacks[ConfigManager.Tables.CfgUI[windowName].Layer].Push(ctrl);
            ctrl.OpenRoot(param);
        }

        /// <summary>
        /// 关闭页面
        /// </summary>
        public void CloseWindow(string windowName,bool byLua)
        {
            if (byLua)
            {
                var funcCloseWindow = _uiManager.Get<Action<LuaTable, string>>("CloseWindow");
                funcCloseWindow?.Invoke(_uiManager, windowName);
                return;
            }

            var ctrl = GetCtrl<UICtrlBase>(windowName);
            var layer = ConfigManager.Tables.CfgUI[windowName].Layer;
            while (_layerStacks[layer].Count != 0)
            {
                var ctrlBefore = _layerStacks[layer].Pop();
                if (ctrlBefore == ctrl)
                {
                    break;
                }
            
                ctrlBefore.CloseRoot();
            }
            
            ctrl.CloseRoot();
        }

        /// <summary>
        /// 关闭指定层级的所有页面
        /// </summary>
        /// <param name="layerName"></param>
        public void CloseAllLayerWindows(string layerName)
        {
            var funcCloseWindow = _uiManager.Get<Action<LuaTable, string>>("CloseAllLayerWindows");
            funcCloseWindow?.Invoke(_uiManager, layerName);
            
            var windowsStack = _layerStacks[layerName];
            while (windowsStack.Count != 0)
            {
                windowsStack.Pop().CloseRoot();
            }
        }

        // /// <summary>
        // /// 销毁窗口
        // /// </summary>
        // /// <param name="windowName"></param>
        // public void DestroyWindow(string windowName)
        // {
        //     if (!_allViews.ContainsKey(windowName))
        //     {
        //         return;
        //     }
        //
        //     var ctrl = _allViews[windowName];
        //     _allViews.Remove(windowName);
        //     var layer = ConfigManager.Tables.CfgUI[windowName].Layer;
        //     while (_layerStacks[layer].Count != 0)
        //     {
        //         var ctrlBefore = _layerStacks[layer].Pop();
        //         if (ctrlBefore == ctrl)
        //         {
        //             break;
        //         }
        //
        //         ctrlBefore.CloseRoot();
        //     }
        //
        //     Object.Destroy(ctrl.gameObject);
        // }

        /// <summary>
        /// 关闭所有页面
        /// </summary>
        public void CloseAllWindows()
        {
            var funcCloseWindow = _uiManager.Get<Action<LuaTable>>("CloseAllWindows");
            funcCloseWindow?.Invoke(_uiManager);
            
            foreach (var (_, value) in _layerStacks)
            {
                if (value.Count == 0)
                {
                    continue;
                }
            
                while (value.Count > 0)
                {
                    var ctrl = value.Pop();
                    ctrl.CloseRoot();
                }
            }
        }

        /// <summary>
        /// 获取ui的controller
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCtrl<T>(string windowName, params object[] param) where T : UICtrlBase
        {
            if (_allViews.ContainsKey(windowName))
            {
                return (T) _allViews[windowName];
            }

            return (T) CreatNewView<T>(windowName, param);
        }

        /// <summary>
        /// 获取ui的controller，没有则返回null
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetCtrlWithoutCreate<T>(string windowName, params object[] param) where T : UICtrlBase
        {
            if (_allViews.ContainsKey(windowName))
            {
                return (T) _allViews[windowName];
            }
        
            return null;
        }
        
        // /// <summary>
        // /// 检测页面是否在打开状态
        // /// </summary>
        // /// <returns></returns>
        // public bool CheckViewActiveInHierarchy(string windowName)
        // {
        //     if (!_allViews.ContainsKey(windowName))
        //     {
        //         return false;
        //     }
        //
        //     var view = _allViews[windowName];
        //     return view && view.gameObject.activeInHierarchy;
        // }
        //
        // /// <summary>
        // /// 判断是不是对应的ctrl
        // /// </summary>
        // /// <param name="viewName"></param>
        // /// <param name="ctrl"></param>
        // /// <returns></returns>
        // private bool IsViewName2Ctrl(string viewName, UICtrlBase ctrl)
        // {
        //     if (!_allViews.ContainsKey(viewName))
        //     {
        //         return false;
        //     }
        //
        //     if (_allViews[viewName] != ctrl)
        //     {
        //         return false;
        //     }
        //
        //     return true;
        // }

        /// <summary>
        /// 创建一个新的ui
        /// </summary>
        /// <param name="windowName"></param>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T CreatNewView<T>(string windowName, params object[] param) where T : UICtrlBase
        {
            var rowCfgUi = _cfgUI[windowName];
            var rootObj = Object.Instantiate(AssetManager.Instance.LoadAsset<GameObject>(rowCfgUi.UiPath), _layers[rowCfgUi.Layer]);
        
            //rootObj上的ctrl开始start并实例化view和model
            rootObj.SetActive(false);
            var canvas = rootObj.GetComponent<Canvas>();
            canvas.worldCamera = CameraManager.Instance.GetUICamera();
            canvas.sortingOrder = rowCfgUi.SortOrder;
            T ctrlNew = null;
            var components = rootObj.GetComponents<Component>();
            foreach (var t in components)
            {
                if (t is UICtrlBase)
                {
                    ctrlNew = t as T;
                    break;
                }
            }
        
            if (!ctrlNew)
            {
                Debug.LogError("找不到viewObj挂载的ctrl" + rootObj.name);
                return null;
            }
        
            _allViews.Add(windowName, ctrlNew);
            ctrlNew.OnInit(param);
            ctrlNew.BindEvent();
            return ctrlNew;
        }
    }
}