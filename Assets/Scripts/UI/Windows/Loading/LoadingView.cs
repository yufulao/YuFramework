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
    public void OpenRoot()
    {
        GameManager.Instance.StartCoroutine(OpenRootCo());
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public void CloseRoot()
    {
        GameManager.Instance.StartCoroutine(CloseRootCo());
    }
    
    /// <summary>
    /// 打开窗口的协程，带动画播放时长
    /// </summary>
    public IEnumerator OpenRootCo()
    {
        mainCanvasGroup.alpha = 0;
        gameObject.SetActive(true);
        yield return Utils.PlayAnimation(animator, "LoadingEnter");
    }

    /// <summary>
    /// 关闭窗口的协程，带动画播放时长
    /// </summary>
    public IEnumerator CloseRootCo()
    {
        mainCanvasGroup.alpha = 1;
        yield return Utils.PlayAnimation(animator, "LoadingExit");
        gameObject.SetActive(false);
    }
}