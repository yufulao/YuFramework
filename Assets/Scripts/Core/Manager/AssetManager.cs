// ******************************************************************
//@file         AssetManager.cs
//@brief        资源加载系统(依赖Addressable)
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:27:57
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Yu
{
    public class AssetManager : BaseSingleTon<AssetManager>, IMonoManager
    {
        //value是handle，获取资源的异步操作句柄，状态可以是isDone也可以是正在加载
        private static readonly Dictionary<string, AsyncOperationHandle> HandleDict = new();
        private bool _hadInit;

        public void OnInit()
        {
            if (_hadInit)
            {
                return;
            }

            Addressables.InitializeAsync();
            _hadInit = true;
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
            //卸载所有加载了的资源
            foreach (var handle in HandleDict.Values)
            {
                Addressables.Release(handle);
            }

            HandleDict.Clear();
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callBack">加载完成后的回调函数</param>
        /// <typeparam name="T">加载完成后的资源</typeparam>
        /// <returns></returns>
        public IEnumerator LoadAssetAsync<T>(string path, Action<T> callBack) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                GameLog.Error("路径不能为空");
                yield break;
            }

            AsyncOperationHandle<T> loadHandle;
            if (HandleDict.TryGetValue(path, out var handleExit)) //已有handle，重复添加handle
            {
                loadHandle = handleExit.Convert<T>();
                if (!loadHandle.IsDone)
                {
                    yield break;
                } //如果已经操作完成，输出回调并且跳出函数

                callBack?.Invoke(loadHandle.Result);
                yield break;
            }

            //第一次添加这个handle
            loadHandle = Addressables.LoadAssetAsync<T>(path);
            HandleDict.Add(path, loadHandle);

            //如果操作没完成
            yield return loadHandle;
            if (loadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                callBack?.Invoke(loadHandle.Result);
                yield break;
            }

            GameLog.Error("加载失败" + path);
            Release(path);
        }

        /// <summary>
        /// 同步加载(单个资源)
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径，如果为空则不会加载</param>
        public static T LoadAsset<T>(string path) where T : Object
        {
            if (string.IsNullOrEmpty(path))
            {
                GameLog.Error("路径不能为空");
                return null;
            }

            AsyncOperationHandle<T> handle;
            if (HandleDict.TryGetValue(path, out var handleExit)) //dic中是否已经有handle操作
            {
                handle = handleExit.Convert<T>(); //只是获取handle，不确定是否已经complete
            }
            else
            {
                handle = Addressables.LoadAssetAsync<T>(path);
                HandleDict.Add(path, handle);
            }

            var asset = handle.WaitForCompletion(); //挂起当前线程，直到操作完成为止

            if (!asset)
            {
                GameLog.Error("加载失败" + path);
            }

            return asset;
        }

        public static GameObject LoadAssetGameObject(string path)
        {
            return LoadAsset<GameObject>(path);
        }

        /// <summary>
        /// 释放handle
        /// </summary>
        /// <param name="path">资源路径</param>
        public static void Release(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                GameLog.Info("要释放的资源的路径为空");
                return;
            }

            //释放句柄，并将这个键名从字典离移除
            if (!HandleDict.TryGetValue(path, out var handle))
            {
                GameLog.Info("没有这个handle" + path);
                return;
            }

            //释放handle
            Addressables.Release(handle);
            HandleDict.Remove(path);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="path">场景path</param>
        /// <param name="callBack">回调中获取场景sceneInstance.Scene</param>
        /// <param name="loadSceneMode">single是替换当前场景，additive是在当前场景上追加新的场景</param>
        public static IEnumerator LoadSceneSync(string path, Action<SceneInstance> callBack, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (string.IsNullOrEmpty(path))
            {
                GameLog.Info("路径不能为空");
                yield break;
            }

            //GameLog.Info(path);
            var sceneLoadHandle = Addressables.LoadSceneAsync(path, loadSceneMode);
            sceneLoadHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    callBack?.Invoke(handle.Result);
                }
                else
                {
                    GameLog.Info("异步加载场景失败" + handle.DebugName);
                    Addressables.Release(handle);
                }
            };
            yield return sceneLoadHandle;
        }

        /// <summary>
        /// 卸载加载好了的场景，同时卸载加载了的场景内的资源，但是只能等场景加载完成后才能卸载
        /// </summary>
        public static void UnloadScene(SceneInstance scene)
        {
            var unloadSceneHandle = Addressables.UnloadSceneAsync(scene);
            unloadSceneHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(handle);
                }
            };

            GameLog.Info("场景卸载失败" + unloadSceneHandle.DebugName);
        }

        /// <summary>
        /// 释放未在使用的资源
        /// </summary>
        public static void ClearUnused()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}