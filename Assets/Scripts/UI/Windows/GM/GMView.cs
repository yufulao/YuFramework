// ******************************************************************
//@file         GMView.cs
//@brief        GM指令界面View
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.27 15:13:10
// ******************************************************************

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Yu;

public class GMView : MonoBehaviour
{
    public GameObject objMask;
    public Animator animator;
    public Button btnBack;
    public GMGridView gridView;
    public Transform containerGmTab;
    public ToggleGroup toggleGroupTab;
    // public List<Button> gmBtnList = new(); // GM指令按钮列表
    
    // [Serializable]
    // public struct CommandBtnInfo
    // {
    //     public int btnWidth;
    //     public int btnHeight;
    // }
    // public CommandBtnInfo commandBtnInfo;
    

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