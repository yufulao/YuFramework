// ******************************************************************
//@file         DetailHangPanel.cs
//@brief        悬停式详情信息子界面
//@author       yufulao, yufulao@qq.com
//@createTime   2024.07.31 14:33:11
// ******************************************************************

using UnityEngine;
using Yu;

public class DetailHangPanel : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject mainPanel;
    private static readonly int AnimationOpenParamName = Utils.GetAnimationIndex("Open");


    /// <summary>
    /// 打开面板
    /// </summary>
    public virtual void OpenPanel(Vector3 position)
    {
        mainPanel.transform.position = position;
        animator.SetBool(AnimationOpenParamName, true);
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public virtual void ClosePanel()
    {
        if (!gameObject.activeInHierarchy) //不是mainPanel
        {
            return;
        }

        animator.SetBool(AnimationOpenParamName, false);
    }

    /// <summary>
    /// 强制关闭面板
    /// </summary>
    public virtual void ForceClose()
    {
        mainPanel.SetActive(false);
    }
}