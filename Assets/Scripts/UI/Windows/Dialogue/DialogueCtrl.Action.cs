// ******************************************************************
//@file         DialogueCtrl.Action.cs
//@brief        剧情对话界面-事件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.09.01 00:03:55 
// ******************************************************************

using UnityEngine;
using Yu;

public partial class DialogueCtrl
{
    private static readonly Color COLOR_PORTRAIT_NOT_HIGH_LIGHT = new(0.5f, 0.5f, 0.5f, 1f);
    private static readonly Color COLOR_PORTRAIT_HIGH_LIGHT = Color.white;

    /// <summary>
    /// 切换立绘
    /// </summary>
    private void SetPortrait(string[] args)
    {
        if (args.Length < 1)
        {
            GameLog.Error("事件SetPortrait 参数数量错误", args.Length);
            return;
        }

        if (!GetPortraitIndex(args[0], out var portraitIndex))
        {
            return;
        }

        var imagePortrait = _view.imageListPortrait[portraitIndex];
        var portraitSpritePath = args.Length < 2 ? null : args[1];
        if (string.IsNullOrEmpty(portraitSpritePath))
        {
            imagePortrait.gameObject.SetActive(false);
            return;
        }

        var sprite = AssetManager.LoadAsset<Sprite>(portraitSpritePath);
        imagePortrait.sprite = sprite;
        imagePortrait.gameObject.SetActive(true);
    }

    /// <summary>
    /// 高亮立绘
    /// </summary>
    private void HighLightPortrait(string[] args)
    {
        if (args.Length < 2)
        {
            GameLog.Error("事件SetPortrait 参数数量错误", args.Length);
            return;
        }

        if (!GetPortraitIndex(args[0], out var portraitIndex))
        {
            return;
        }

        var imagePortrait = _view.imageListPortrait[portraitIndex];
        var isHighLight = bool.Parse(args[1]);
        imagePortrait.color = isHighLight ? COLOR_PORTRAIT_HIGH_LIGHT : COLOR_PORTRAIT_NOT_HIGH_LIGHT;
        // GameLog.Error(portraitIndex, isHighLight, imagePortrait.color, COLOR_PORTRAIT_HIGH_LIGHT, COLOR_PORTRAIT_NOT_HIGH_LIGHT);
    }

    /// <summary>
    /// 获取事件参数的立绘位
    /// </summary>
    private bool GetPortraitIndex(string portraitIndexStr, out int portraitIndex)
    {
        if (!int.TryParse(portraitIndexStr, out portraitIndex))
        {
            GameLog.Error("立绘序号参数错误", portraitIndexStr);
            portraitIndex = -1;
            return false;
        }

        if (portraitIndex >= 0 && portraitIndex < _view.imageListPortrait.Count)
        {
            return true;
        }

        GameLog.Error("立绘序号超出", portraitIndex, _view.imageListPortrait.Count);
        portraitIndex = -1;
        return false;
    }
}