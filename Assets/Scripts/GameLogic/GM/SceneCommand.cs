// ******************************************************************
//@file         SceneCommand.cs
//@brief        场景GM指令
//@author       yufulao, yufulao@qq.com
//@createTime   2024.10.18 10:58:21
// ******************************************************************

using UnityEngine;
using Yu;

[GMCatalog("场景")]
public static class SceneCommand
{
    [GMMethodUI, GMMethod("切换场景", "场景id")]
    public static void ChangeScene(GMTypeSceneName sceneName)
    {
        GameManager.Instance.StartCoroutine(SceneManager.Instance.ChangeScene(sceneName.Value));
        GameLog.Info("切换场景");
    }
}