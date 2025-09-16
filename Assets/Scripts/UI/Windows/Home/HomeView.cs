// ******************************************************************
//@file         HomeView.cs
//@brief        主界面View
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.22 20:38:27
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;

public class HomeView : MonoBehaviour
{
    /// <summary>
    /// 打开界面
    /// </summary>
    public void OnOpen()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public void OnClose()
    {
        gameObject.SetActive(false);
    }
}