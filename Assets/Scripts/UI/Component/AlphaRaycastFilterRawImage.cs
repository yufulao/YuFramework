// ******************************************************************
//@file         AlphaRaycastFilterRawImage.cs
//@brief        RawImage透明穿透
//@author       yufulao, yufulao@qq.com
//@createTime   2025.03.06 13:18:13
// ******************************************************************

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class AlphaRaycastFilterRawImage : UIBehaviour, ICanvasRaycastFilter
{
    public float alphaThreshold = 1;
    private RawImage _rawImage;


    private RawImage RawImage
    {
        get { return _rawImage ??= GetComponent<RawImage>(); }
    }

    public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        switch (alphaThreshold)
        {
            case <= 0:
                return true;
            case > 1:
                return false;
        }

        var texture = RawImage.mainTexture as Texture2D;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RawImage.rectTransform, screenPoint, eventCamera, out var local))
        {
            return false;
        }

        var rect = RawImage.GetPixelAdjustedRect();
        local.x += RawImage.rectTransform.pivot.x * rect.width;
        local.y += RawImage.rectTransform.pivot.y * rect.height;
        local = new Vector2(local.x / rect.width, local.y / rect.height);
        try
        {
            return texture && texture.GetPixelBilinear(local.x, local.y).a >= alphaThreshold;
        }
        catch (UnityException e)
        {
            GameLog.Error($"RawImage透明穿透失败: {e}");
            return true;
        }
    }
}