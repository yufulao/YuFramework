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

namespace Yu
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private readonly List<IMonoManager> _managerList = new();
        public bool test; //测试模式
        public bool crack; //破解版

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            Application.targetFrameRate = 240;
            // Framework
            _managerList.Add(AssetManager.Instance);
            _managerList.Add(SaveManager.Instance);
            _managerList.Add(ConfigManager.Instance);
            _managerList.Add(PoolManager.Instance);
            _managerList.Add(InputManager.Instance);
            _managerList.Add(FsmManager.Instance);
            _managerList.Add(BGMManager.Instance);
            _managerList.Add(SFXManager.Instance);
            _managerList.Add(SceneManager.Instance);
            _managerList.Add(UIManager.Instance);
            _managerList.Add(CameraManager.Instance);
            _managerList.Add(ConditionManager.Instance);
            _managerList.Add(ActionManager.Instance);
            // GameLogic
            _managerList.Add(LocalizeManager.Instance);
            _managerList.Add(DialogueManager.Instance);

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
                    EventManager.Instance.AddListener(EventName.OnGmKeyDown, () =>
                    {
                        UIManager.Instance.SwitchWindowActive("GMView");
                    });
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
                GameLog.Error(e);
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
        public static T _Gm_GetComponent<T>() where T : UnityEngine.Object
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
        /// 切换场景
        /// </summary>
        public static void ChangeScene(string sceneID)
        {
            Instance.StartCoroutine(ChangeSceneIEnumerator(sceneID));
        }

        /// <summary>
        /// 暂停指定协程
        /// </summary>
        public static void KillCoroutine(Coroutine coroutine)
        {
            if (coroutine == null)
            {
                return;
            }
            
            Instance.StopCoroutine(coroutine);
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
        /// 切换场景的协程
        /// </summary>
        public static IEnumerator ChangeSceneIEnumerator(string sceneID)
        {
            BGMManager.Instance.StopBgmFadeDelay(0, 0.5f);
            yield return UIManager.Instance.GetCtrl<LoadingCtrl>("LoadingView").OpenRootCo(); //等待LoadingView打开动画
            GC.Collect();
            var rowCfgScene = ConfigManager.Tables.CfgScene[sceneID];
            BGMManager.Instance.PlayBgm(rowCfgScene.BGM);
            yield return SceneManager.Instance.ChangeScene(sceneID);
            UIManager.Instance.CloseWindow("LoadingView");
        }

        /// <summary>
        /// 返回游戏标题的协程
        /// </summary>
        private static IEnumerator ReturnToTitleIEnumerator()
        {
            EventManager.Instance.Dispatch(EventName.OnExitGame);
            yield return UIManager.Instance.GetCtrl<LoadingCtrl>("LoadingView").OpenRootCo(); //等待LoadingView打开动画
            UIManager.Instance.CloseLayerWindows("NormalLayer");
            GC.Collect();
            // yield return SceneManager.Instance.ChangeScene("Title");
            HUDManager.Instance.CloseAll();
            UIManager.Instance.OpenWindow("HomeView");
            UIManager.Instance.CloseWindow("LoadingView");
        }
    }
}