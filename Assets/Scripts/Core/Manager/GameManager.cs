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
            DontDestroyOnLoad(this.gameObject);
            // Application.targetFrameRate = 120;

            _managerList.Add(EventManager.Instance);
            _managerList.Add(AssetManager.Instance);
            _managerList.Add(ConfigManager.Instance);
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

            VersionControl(); //版本检查
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
        /// 返回游戏标题
        /// </summary>
        public void ReturnToTitle()
        {
            StartCoroutine(ReturnToTitleIEnumerator());
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void QuitApplication()
        {
#if UNITY_EDITOR //在编辑器模式下

            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// 设置时间速率
        /// </summary>
        /// <param name="timeScale"></param>
        public static void SetTimeScale(float timeScale)
        {
            Time.timeScale = timeScale;
        }

        /// <summary>
        /// 当前是测试模式
        /// </summary>
        private static void IsTest()
        {
            Instantiate(AssetManager.Instance.LoadAsset<GameObject>(ConfigManager.Tables.CfgUI["IngameDebugView"].UiPath), GameObject.Find("NormalLayer").transform);
        }

        /// <summary>
        /// 返回游戏标题的协程
        /// </summary>
        private IEnumerator ReturnToTitleIEnumerator()
        {
            SetTimeScale(1f);
            UIManager.Instance.OpenWindow("LoadingView", true);
            UIManager.Instance.CloseAllLayerWindows("NormalLayer");
            yield return new WaitForSeconds(0.3f); //等待所有windowClose动画
            CameraManager.Instance.ResetObjCamera();
            GC.Collect();
            //yield return SceneManager.Instance.ChangeSceneAsync("Assets/AddressableAssets/Scene/MainScene.unity");
            UIManager.Instance.CloseWindow("LoadingView", true);
        }

        /// <summary>
        /// 游戏版本控制
        /// </summary>
        private static void VersionControl()
        {
            var lastVersion = SaveManager.GetString("Version", "0.0.0");
            var nowVersion = Application.version;
            if (lastVersion.Equals(nowVersion))
            {
                return;
            }

            SaveManager.DeleteFile();
            SaveManager.SetString("Version", nowVersion);
            // SaveManager.DeleteKey("StageName");
            // SaveManager.DeleteKey("PlotNameInMainPlot");
            // SaveManager.DeleteKey("StageData");
            // SaveManager.DeleteKey("TeamData");
            // SaveManager.DeleteKey("SkillData");
        }
    }
}