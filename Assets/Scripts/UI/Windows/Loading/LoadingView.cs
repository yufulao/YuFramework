// ******************************************************************
//@file         LoadingView.cs
//@brief        加载界面的view
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:36:05
// ******************************************************************

using System.Collections;
using UnityEngine;
using Yu;

public class LoadingView : MonoBehaviour
{
    public Animator animator;
    public CanvasGroup mainCanvasGroup;


    /// <summary>
    /// 打开窗口
    /// </summary>
    public void OpenWindow()
    {
        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(true);
        GameManager.Instance.StartCoroutine(OpenRootIEnumerator());
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public void CloseWindow()
    {
        GameManager.Instance.StartCoroutine(CloseRootIEnumerator());
    }

    /// <summary>
    /// OpenRoot()的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator OpenRootIEnumerator()
    {
        yield return Utils.PlayAnimation(animator, "LoadingEnter");
    }

    /// <summary>
    /// CloseRoot()的协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator CloseRootIEnumerator()
    {
        mainCanvasGroup.alpha = 1;
        yield return Utils.PlayAnimation(animator, "LoadingExit");
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
    }
}