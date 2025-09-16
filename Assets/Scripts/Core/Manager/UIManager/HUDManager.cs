// ******************************************************************
//@file         HUDManager.cs
//@brief        HUD管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.22 14:07:54
// ******************************************************************

using System.Collections.Generic;
using UnityEngine;
using Yu;
using Object = UnityEngine.Object;

public class HUDManager : BaseSingleTon<HUDManager>
{
    public Transform Root { get; private set; }
    private readonly Dictionary<string, HUDBase> _hudDict = new();


    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(Transform root)
    {
        Root = root;
    }

    /// <summary>
    /// 获取HUD
    /// </summary>
    public HUDBase GetHUD(string hudName)
    {
        if (_hudDict.TryGetValue(hudName, out var hudExist))
        {
            return hudExist;
        }

        var objOriginal = AssetManager.LoadAssetGameObject(ConfigManager.Tables.CfgHUD[hudName].UiPath);
        var obj = Object.Instantiate(objOriginal, Root);
        var hud = obj.GetComponent<HUDBase>();
        _hudDict[hudName] = hud;
        obj.SetActive(false);
        hud.OnInit();
        hud.BindEvent();
        return hud;
    }
    
    /// <summary>
    /// 获取指定HUDBase
    /// </summary>
    public T GetHUD<T>(string hudName) where T : HUDBase
    {
        var hud = GetHUD(hudName);
        return hud as T;
    }

    /// <summary>
    /// 隐藏HUD
    /// </summary>
    public void CloseHUD(string hudName)
    {
        GetHUD(hudName).CloseRoot();
    }
    
    /// <summary>
    /// 关闭所有HUD界面
    /// </summary>
    public void CloseAll()
    {
        foreach (var (_, hud) in _hudDict)
        {
            hud.CloseRoot();
        }
    }
}