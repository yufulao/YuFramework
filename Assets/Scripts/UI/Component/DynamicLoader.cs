// ******************************************************************
//@file         DynamicLoader.cs
//@brief        动态加载组件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.06.27 12:29:57 
// ******************************************************************

using System;
using UnityEngine;
using UnityEngine.Events;
using Yu;

public class DynamicLoader : MonoBehaviour
{
    public string resPath;
    public bool loadOnAwake;
    public UnityEvent onLoaded = new();
    private Action<GameObject> _onLoadedCallback;
    private GameObject _proto;
    private GameObject _go;
    private Coroutine _cacheCo;

    private void Awake()
    {
        if (loadOnAwake)
        {
            Load(resPath, OnLoadedUnityEvent);
        }
    }

    /// <summary>
    /// 加载prefab
    /// </summary>
    /// <returns>是否执行callback</returns>
    public bool Load(string path, Action<GameObject> loadedCallback)
    {
        if (string.IsNullOrEmpty(path))
        {
            if (_cacheCo != null)
            {
                StopCoroutine(_cacheCo);
                _cacheCo = null;
            }
            
            Remove();
            return false;
        }

        if (resPath.Equals(path))
        {
            if (_proto)
            {
                OnLoaded(_proto);
                return false;
            }

            if (_cacheCo != null)
            {
                return false;
            }
        }

        resPath = path;
        _proto = null;
        _onLoadedCallback = loadedCallback;
        if (_cacheCo != null)
        {
            StopCoroutine(_cacheCo);
        }

        _cacheCo = StartCoroutine(AssetManager.Instance.LoadAssetAsync<GameObject>(path, OnLoaded));
        return true;
    }

    /// <summary>
    /// 移除prefab
    /// </summary>
    public void Remove()
    {
        if (!_go)
        {
            return;
        }

        Destroy(_go);
    }

    /// <summary>
    /// 加载完成
    /// </summary>
    private void OnLoaded(GameObject proto)
    {
        _proto = proto;
        Remove();
        var go = Instantiate(proto, transform);
        _go = go;
        _onLoadedCallback?.Invoke(go);
        OnLoadedUnityEvent(go);
        _cacheCo = null;
    }

    /// <summary>
    /// 加载完成后的unityEvent回调
    /// </summary>
    private void OnLoadedUnityEvent(GameObject go)
    {
        onLoaded.Invoke();
    }
}