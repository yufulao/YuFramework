// ******************************************************************
//@file         LocalizeTextMeshProUGUI.cs
//@brief        多语言TMP
//@author       yufulao, yufulao@qq.com
//@createTime   2024.10.27 17:28:16
// ******************************************************************

using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using Yu;

//editor序列化需要
[System.Serializable]
public class LanguageFontMapping
{
    public DefLanguage language;
    public string fontID; //配表
}

public class LocalizeTextMeshProUGUI : TextMeshProUGUI
{
    [SerializeField] private int textKey; //静态文本id
    [SerializeField] private List<LanguageFontMapping> fontList = new(); //指定字体，而不是当前语言的配表默认值,dict<语言，fondID(配表)>
    private readonly StringBuilder _resultBuilder = new(); //用于拼接结果
    

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager.Instance.AddListener(EventName.OnLanguageChanged, OnLanguageChanged);
        OnLanguageChanged();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager.Instance.RemoveListener(EventName.OnLanguageChanged, OnLanguageChanged);
    }

    /// <summary>
    /// 动态更新文本
    /// </summary>
    public void UpdateText(int textKeyNew)
    {
        textKey = textKeyNew;
        RefreshText();
    }

    /// <summary>
    /// 刷新文字显示
    /// </summary>
    private void RefreshText()
    {
        if (textKey == default)
        {
            return;
        }
        
        var textTranslated = LocalizeManager.GetLocalization(textKey);
        if (string.IsNullOrEmpty(textTranslated))
        {
            return;
        }

        _resultBuilder.Clear();
        _resultBuilder.Append(textTranslated);
        text = _resultBuilder.ToString().Replace("\\n", "\n");
    }

    /// <summary>
    /// 语言更变回调
    /// </summary>
    private void OnLanguageChanged()
    {
        //Text组件估计有编辑模式运行的特性 每次关掉都会走awake
        if (!Application.isPlaying)
        {
            return;
        }

        //更新字体
        var curLanguage = LocalizeManager.CurLanguage;
        var fontID = curLanguage.ToString();
        foreach (var mapping in fontList)
        {
            if (mapping.language != curLanguage)
            {
                continue;
            }
            
            fontID = mapping.fontID;
            break;
        }

        font = LocalizeManager.Instance.GetFontTMPSync(fontID);
        RefreshText(); //先更新字体再更新文本
    }

    // /// <summary>
    // /// 字体资源加载完成回调
    // /// </summary>
    // private void OnFontAssetLoadComplete(TMP_FontAsset f)
    // {
    //     font = f;
    //     RefreshText();//先更新字体再更新文本
    //     
    //     //如果缺少配置 则使用默认字体配置
    //     if (fontSettingId == 0)
    //     {
    //         fontSettingId = 1;
    //     }
    //     
    //     var rowCfgFontSetting = ConfigManager.Tables.cfgFontSettingPro[fontSettingId];
    //     var index = SaveGameManager.Instance.Get("languageId","ch",SaveType.Cfg);
    //     //越界
    //     if (index >= rowCfgFontSetting.meshProSettings.Length) 
    //     {
    //         GameLog.Error($"当前语言缺少字体配置:{SaveGameManager.Instance.Get("languageId","ch",SaveType.Cfg)}");
    //         return;
    //     }
    //     
    //     //调整新字体参数
    //     fontSize = _cacheFontSize * rowCfgFontSetting.meshProSettings[index].fontSizeScale;
    //     lineSpacing = _cacheLineSpacing * rowCfgFontSetting.meshProSettings[index].lineSpacingScale;
    //     characterSpacing = _cacheCharacterSpacing * rowCfgFontSetting.meshProSettings[index].characterSpacingScale;
    // }
}