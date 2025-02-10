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

        private async void Start()
        {
            try
            {
                if (test)
                {
                    await GMCommand.OnInit(); //初始化GM指令
                    EventManager.Instance.AddListener(EventName.OnGmOpen, GMCtrl.OpenGmView);
                }

                if (crack)
                {
                }
                
                BGMManager.Instance.ReloadVolume();
                SFXManager.Instance.ReloadVolume();
                ReturnToTitle();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
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
            return FindObjectOfType<T>();
        }

        /// <summary>
        /// 返回游戏标题
        /// </summary>
        public static void ReturnToTitle()
        {
            Instance.StartCoroutine(ReturnToTitleIEnumerator());
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

        /// <summary>
        /// 返回游戏标题的协程
        /// </summary>
        private static IEnumerator ReturnToTitleIEnumerator()
        {
            UIManager.Instance.OpenWindow("LoadingView");
            UIManager.Instance.CloseLayerWindows("NormalLayer");
            CameraManager.Instance.ResetObjCamera();
            GC.Collect();
            yield return SceneManager.Instance.ChangeScene("SampleScene");
            HUDManager.Instance.CloseAll();
            UIManager.Instance.CloseWindow("LoadingView");
        }

    }
}