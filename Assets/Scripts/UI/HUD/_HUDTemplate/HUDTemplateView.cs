using System.Collections;
using UnityEngine;
using Yu;

public class HUDTemplateView : MonoBehaviour
{
    public GameObject objMask;
    public Animator animator;
    

    // /// <summary>
    // /// 打开窗口
    // /// </summary>
    // public void OpenWindow()
    // {
    //     objMask.SetActive(false);
    //     gameObject.SetActive(true);
    //     animator.Play("Show", 0, 0f);
    // }

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
    private IEnumerator CloseWindowIEnumerator()
    {
        objMask.SetActive(true);
        yield return Utils.PlayAnimation(animator, "Hide");
        gameObject.SetActive(false);
    }
}