// ******************************************************************
//@file         GameManager.cs
//@brief        游戏总管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:29:23
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace Yu
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private readonly List<IMonoManager> _managerList = new List<IMonoManager>();
        public bool test; //测试模式
        public bool crack; //破解版

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            // Application.targetFrameRate = 120;

            _managerList.Add(EventManager.Instance);
            _managerList.Add(AssetManager.Instance);
            _managerList.Add(SaveManager.Instance);
            _managerList.Add(ConfigManager.Instance);
            _managerList.Add(PoolManager.Instance);
            _managerList.Add(InputManager.Instance);
            _managerList.Add(FsmManager.Instance);
            _managerList.Add(LuaManager.Instance);
            _managerList.Add(BGMManager.Instance);
            _managerList.Add(SFXManager.Instance);
            _managerList.Add(SceneManager.Instance);
            _managerList.Add(UIManager.Instance);
            _managerList.Add(CameraManager.Instance);

            foreach (var manager in _managerList)
            {
                manager.OnInit();
            }
        }

        private void Start()
        {
            if (test)
            {
                IsTest();
            }
            
            BGMManager.Instance.ReloadVolume();
            SFXManager.Instance.ReloadVolume();
            ReturnToTitle();
        }

        private void Update()
        {
            foreach (var manager in _managerList)
            {
                manager.Update();
            }
        }

        private void FixedUpdate()
        {
            foreach (var manager in _managerList)
            {
                manager.FixedUpdate();
            }
        }

        private void LateUpdate()
        {
            foreach (var manager in _managerList)
            {
                manager.LateUpdate();
            }
        }

        private void OnDestroy()
        {
            for (var i = _managerList.Count - 1; i >= 0; i--)
            {
                _managerList[i].OnClear();
            }
        }

        /// <summary>
        /// 只能用于GM指令获取场景组件
        /// </summary>
        public T GetComponentForGm<T>() where T : Object
        {
            return  FindObjectOfType<T>();
        }

        /// <summary>
        /// 返回游戏标题
        /// </summary>
        public static void ReturnToTitle()
        {
            Instance.StartCoroutine(ReturnToTitleIEnumerator());
        }

        /// <summary>
        /// 检测esc暂停游戏，暂定
        /// </summary>
        public static void OnUpdateCheckPause()
        {
            if (!Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                return;
            }
            
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void QuitApplication()
        {
#if UNITY_STANDALONE
            Application.Quit();
#endif

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private static IEnumerator ReturnToTitleIEnumerator()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            UIManager.Instance.CloseLayerWindows("NormalLayer");
            CameraManager.Instance.ResetObjCamera();
            GC.Collect();
            yield return SceneManager.Instance.ChangeSceneAsync(ConfigManager.Tables.CfgScene["SampleScene"].ScenePath);
            UIManager.Instance.CloseWindow("LoadingView");
        }
        
        /// <summary>
        /// 当前是测试模式
        /// </summary>
        private void IsTest()
        {
            Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Tables.CfgUI["IngameDebugView"].UiPath), GameObject.Find("TopLayer").transform);
        }
    }
}