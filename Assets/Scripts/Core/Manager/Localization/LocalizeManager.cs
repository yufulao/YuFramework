// ******************************************************************
//@file         LocalizeManager.cs
//@brief        多语言管理
//@author       yufulao, yufulao@qq.com
//@createTime   2024.10.27 19:45:51
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yu;

public class LocalizeManager : BaseSingleTon<LocalizeManager>, IMonoManager
{
    public static DefLanguage CurLanguage { get; private set; }
    private readonly Dictionary<string, TMP_FontAsset> _cacheFontTmpDic = new();
    private readonly Dictionary<string, Font> _cacheFontDic = new();


    public void OnInit()
    {
        // CurLanguage = SaveGameManager.Instance.Get(GlobalDef.CurrentLanguageKey, DefLanguage.zh_CN, SaveType.Cfg);
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
    
    /// <summary>
    /// 切换语言
    /// </summary>
    public static void ChangeLanguage(DefLanguage language)
    {
        CurLanguage = language;
        // SaveGameManager.Instance.Set(GlobalDef.CurrentLanguageKey, language, SaveType.Cfg);
        EventManager.Instance.Dispatch(EventName.OnLanguageChanged);
    }

    /// <summary>
    /// 翻译文本
    /// </summary>
    public static string GetLocalization(int textKey)
    {
        var rowCfg = ConfigManager.Tables.CfgLocalization.GetOrDefault(textKey);
        if (rowCfg == null)
        {
            return null;
        }
        
        switch (CurLanguage)
        {
            case DefLanguage.zh_CN:
                return rowCfg.ZhCN;
            case DefLanguage.en:
                return rowCfg.En;
            case DefLanguage.jp:
                return rowCfg.Jp;
            default:
                GameLog.Error("没有实现语言" + CurLanguage);
                return null;
        }
    }
    
    /// <summary>
    /// 本地化图片
    /// </summary>
    public static Sprite GetLocalizationImage(int imageKey)
    {
        var rowCfg = ConfigManager.Tables.CfgLocalizationImage.GetOrDefault(imageKey);
        if (rowCfg == null)
        {
            return null;
        }

        string spritePath;
        switch (CurLanguage)
        {
            case DefLanguage.zh_CN:
                spritePath = rowCfg.ZhCN;
                break;
            case DefLanguage.en:
                spritePath = rowCfg.En;
                break;
            case DefLanguage.jp:
                spritePath = rowCfg.Jp;
                break;
            default:
                GameLog.Error("没有实现语言的贴图" + CurLanguage);
                return null;
        }

        return string.IsNullOrEmpty(spritePath) ? null : AssetManager.LoadAsset<Sprite>(spritePath);
    }

    /// <summary>
    /// 同步获取指定的TMP字体资源
    /// </summary>
    public TMP_FontAsset GetFontTMPSync(string fontID)
    {
        if (_cacheFontTmpDic.TryGetValue(fontID, out var font))
        {
            return font;
        }
        
        var fontPath = ConfigManager.Tables.CfgFont[fontID].SdfFontPath;
        return AssetManager.LoadAsset<TMP_FontAsset>(fontPath);
    }
    
    /// <summary>
    /// 异步获取指定的TMP字体资源，字体资源较大需要异步
    /// </summary>
    public void GetFontTMPAsync(string fontID, Action<TMP_FontAsset> callback)
    {
        GameManager.Instance.StartCoroutine(GetFontTMPAsyncCo(fontID, callback));
    }

    /// <summary>
    /// 获取指定的常规字体资源
    /// </summary>
    public void GetFont(string fontID, Action<Font> callback)
    {
        GameManager.Instance.StartCoroutine(GetFontCo(fontID, callback));
    }
    
    /// <summary>
    /// 获取TMP字体资源的协程
    /// </summary>
    private IEnumerator GetFontTMPAsyncCo(string fontID, Action<TMP_FontAsset> callback)
    {
        if (_cacheFontTmpDic.TryGetValue(fontID, out var fontExit))
        {
            callback.Invoke(fontExit);
            yield break;
        }
        
        var fontPath = ConfigManager.Tables.CfgFont[fontID].SdfFontPath;
        TMP_FontAsset font = null;
        yield return AssetManager.Instance.LoadAssetAsync<TMP_FontAsset>(fontPath, (result) => { font = result; });
        if (!font)
        {
            yield break;
        }

        if (_cacheFontTmpDic.TryGetValue(fontID, out var fontExitNew))//加载中可能外部多次loadAsset
        {
            callback.Invoke(fontExitNew);
            yield break;
        }
        
        _cacheFontTmpDic.Add(fontID,font);
        callback.Invoke(font);
    }
    
    /// <summary>
    /// 获取常规字体资源的协程
    /// </summary>
    private IEnumerator GetFontCo(string fontID, Action<Font> callback)
    {
        if (_cacheFontDic.TryGetValue(fontID, out var fontExit))
        {
            callback.Invoke(fontExit);
            yield break;
        }
        
        var fontPath = ConfigManager.Tables.CfgFont[fontID].FontPath;
        Font font = null;
        yield return AssetManager.Instance.LoadAssetAsync<Font>(fontPath, (result) => { font = result; });
        if (!font)
        {
            yield break;
        }
        
        _cacheFontDic.Add(fontID,font);
        callback.Invoke(font);
    }
}