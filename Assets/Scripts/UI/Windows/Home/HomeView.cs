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
    // public Button btnNewGame;
    // public Button btnContinue;
    public Button btnLoad;
    public Button btnSetting;
    public Button btnQuit;


    /// <summary>
    /// 打开界面
    /// </summary>
    public void OnOpen()
    {
        //RefreshLastSaveGame();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    public void OnClose()
    {
        gameObject.SetActive(false);
    }

    // /// <summary>
    // /// 刷新最新存档摁钮
    // /// </summary>
    // public void RefreshLastSaveGame()
    // {
    //     var lastSaveGame = SaveManager.Get(GlobalDef.LastSaveGameIndex, -1, SaveType.Global);
    //     var hasLastSaveGame = lastSaveGame >= 0;
    //     btnContinue.gameObject.SetActive(hasLastSaveGame);
    // }
}