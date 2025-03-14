// ******************************************************************
//@file         SceneManager.cs
//@brief        场景加载系统
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:30:37
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Yu
{
    public class SceneManager : BaseSingleTon<SceneManager>, IMonoManager
    {
        private readonly Dictionary<string, SceneInstance> _loadedSceneDic = new Dictionary<string, SceneInstance>(); //已加载的场景 k:资源路径 v:实例
        public string SceneID { get; private set; }

        public void OnInit()
        {
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
            _loadedSceneDic.Clear();
        }

        /// <summary>
        /// 切换场景
        /// </summary>
        public IEnumerator ChangeScene(string sceneID)
        {
            var scenePath = ConfigManager.Tables.CfgScene[sceneID].ScenePath;
            yield return ChangeSceneAsync(scenePath);
            EventManager.Instance.Dispatch(EventName.ChangeScene);
            SceneID = sceneID;
        }

        public IEnumerator ChangeScene(string sceneID, string characterSpawnPointId)
        {
            yield return ChangeScene(sceneID);
            EventManager.Instance.Dispatch(EventName.ChangeScene, characterSpawnPointId);
        }

        /// <summary>
        /// 异步切换场景
        /// </summary>
        private IEnumerator ChangeSceneAsync(string scenePath)
        {
            //single模式切换场景
            yield return AssetManager.Instance.LoadSceneSync(scenePath, (sceneInstance) =>
            {
                DOTween.KillAll();
                if (!_loadedSceneDic.ContainsKey(scenePath))
                {
                    _loadedSceneDic.Add(scenePath, sceneInstance);
                }

                // 激活加载的场景
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(sceneInstance.Scene);
            });
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="scenePath"> 场景名称 </param>
        private void UnloadScene(string scenePath)
        {
            AssetManager.Instance.UnloadScene(_loadedSceneDic[scenePath]);
            _loadedSceneDic.Remove(scenePath);
        }
    }
}