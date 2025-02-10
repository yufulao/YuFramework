using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yu;

/// <summary>
/// GM指令界面View
/// </summary>
public class GMView : MonoBehaviour
{
    public GameObject objMask;
    public Animator animator;
    public Button btnBack;
    public GMGridView gridView;
    

    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    public void OpenWindow()
    {
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

        GameManager.Instance.StartCoroutine(CloseWindowIEnumerator());
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