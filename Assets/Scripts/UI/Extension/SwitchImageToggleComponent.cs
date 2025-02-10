// ******************************************************************
//@file         SwitchImageToggleComponent.cs
//@brief        用来切换toggle，只显示一种图片的组件
//@author       yufulao, yufulao@qq.com
//@createTime   2024.02.28 12:04:13
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SwitchImageToggleComponent : MonoBehaviour
{
    private Toggle _toggle;
    private Graphic _bgImage;
    private Graphic _checkMarkImage; //源码是通过改graphic来隐藏checkMark的。graphic.CrossFadeAlpha(m_IsOn ? 1f : 0f, instant ? 0f : 0.1f, true);
    private static Color _transparentColor;
    private static Color _opaqueColor;


    private void Start()
    {
        _toggle = GetComponent<Toggle>();
        _bgImage = _toggle.targetGraphic;
        _checkMarkImage = _toggle.graphic;
        _transparentColor = new Color(_bgImage.color.a, _bgImage.color.g, _bgImage.color.b, 0);
        _opaqueColor = new Color(_bgImage.color.a, _bgImage.color.g, _bgImage.color.b, 255);
        _toggle.onValueChanged.AddListener(CheckIsOn);
        CheckIsOn(_toggle.isOn);
    }

    private void OnDestroy()
    {
        _toggle.onValueChanged.RemoveAllListeners();
    }

    private void CheckIsOn(bool isOn)
    {
        if (isOn)
        {
            _bgImage.color = _transparentColor;
            _checkMarkImage.gameObject.SetActive(true); //隐藏checkMark方式手动改为SetActive
            return;
        }

        _bgImage.color = _opaqueColor;
        _checkMarkImage.gameObject.SetActive(false);
    }
}