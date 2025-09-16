// ******************************************************************
//@file         LocalizeImage.cs
//@brief        image本地化组件
//@author       yufulao, yufulao@qq.com
//@createTime   2024.10.28 12:27:50
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;
using Yu;

public class LocalizeImage : Image
{
    [SerializeField] private int imageKey; // 图片ID


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
    /// 动态更新图像
    /// </summary>
    public void UpdateImage(int imageKeyNew)
    {
        imageKey = imageKeyNew;
        RefreshImage();
    }

    /// <summary>
    /// 刷新图像显示
    /// </summary>
    private void RefreshImage()
    {
        var asset = LocalizeManager.GetLocalizationImage(imageKey);
        sprite = asset ? asset : sprite;
    }

    /// <summary>
    /// 语言更变回调
    /// </summary>
    private void OnLanguageChanged()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        RefreshImage();
    }
}