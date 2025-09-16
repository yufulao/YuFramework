// ******************************************************************
//@file         MiscCommand.cs
//@brief        其他GM指令
//@author       yufulao, yufulao@qq.com
//@createTime   2024.08.10 14:33:50
// ******************************************************************

using UnityEngine;
using Yu;

[GMCatalog("杂项")]
public static class MiscCommand
{
    [GMMethodUI, GMMethod("切换语言", "语言")]
    public static void ChangeLanguage(GMTypeLanguage language)
    {
        LocalizeManager.ChangeLanguage(language.Value);
        GameLog.Info("切换语言" + language.Value.ToString());
    }
    
    [GMMethodUI, GMMethod("播放剧情", "剧情id")]
    public static void StartConversation(int dialogueId)
    {
        DialogueManager.Instance.StartConversation(dialogueId);
        GameLog.Info("播放剧情" + dialogueId);
    }
}