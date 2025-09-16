using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yu;

public class DialogueView : MonoBehaviour
{
    public List<Image> imageListPortrait;
    public LocalizeTextMeshProUGUI textDialogue;
    public Button btnNext;
    public DialogueOptionGridView gridViewDialogueOption;
    public GameObject objMask;
    public Animator animator;
    private Coroutine _cacheCloseWindowCo;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 隐藏所有立绘
    /// </summary>
    public void HideAllPortrait()
    {
        foreach (var image in imageListPortrait)
        {
            image.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    public void OpenWindow()
    {
        if (_cacheCloseWindowCo != null)
        {
            StopCoroutine(_cacheCloseWindowCo);
            _cacheCloseWindowCo = null;
        }
        
        objMask.SetActive(false);
        gameObject.SetActive(true);
        animator.Play("Show", 0, 0f);
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public void CloseWindow()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        _cacheCloseWindowCo = StartCoroutine(CloseWindowIEnumerator());
    }

    /// <summary>
    /// 关闭窗口的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator CloseWindowIEnumerator()
    {
        objMask.SetActive(true);
        yield return Utils.PlayAnimation(animator, "Hide");
        gameObject.SetActive(false);
    }
}