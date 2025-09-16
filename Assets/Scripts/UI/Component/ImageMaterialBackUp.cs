// ******************************************************************
//@file         ImageMaterialBackUp.cs
//@brief        将Image的Material复制来赋值 （避免直接修改原材质）
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.01 13:23:12
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageMaterialBackUp : MonoBehaviour
{
    private Image _image;
    private Material _originalMaterial;
    private Material _runtimeMaterial;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _originalMaterial = _image.material;
        if (!_originalMaterial)
        {
            return;
        }

        _runtimeMaterial = Instantiate(_originalMaterial);
        _image.material = _runtimeMaterial;
    }
}