// ******************************************************************
//@file         AlphaRaycastFilterImage.cs
//@brief        Image透明穿透
//@author       yufulao, yufulao@qq.com
//@createTime   2025.03.06 13:21:31
// ******************************************************************

using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaRaycastFilterImage : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] [LabelText("透明度大于多少才允许点击")]
    private float alphaThreshold = 0.1f;

    private Image _image;


    private void Awake()
    {
        _image = GetComponent<Image>();
        ResetAlphaHitTestMinimumThreshold();
    }

    /// <summary>
    /// 修改透明度穿透阈值
    /// </summary>
    public void UpdateAlphaThreshold(float threshold)
    {
        alphaThreshold = threshold;
        ResetAlphaHitTestMinimumThreshold();
    }

    /// <summary>
    /// 更新image的alphaHitTestMinimumThreshold
    /// </summary>
    private void ResetAlphaHitTestMinimumThreshold()
    {
        if (!_image.sprite.texture.isReadable)
        {
            GameLog.Error($"透明穿透功能未开启深度写入, image: {gameObject.name}, sprite: {_image.sprite.name}, 请开启该sprite的Advanced->Read/Write");
            return;
        }

        _image.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}