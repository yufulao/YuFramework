// ******************************************************************
//@file         ActionManager.cs
//@brief        注册初始原子事件
//@author       yufulao, yufulao@qq.com
//@createTime   2025.07.27 22:43:08 
// ******************************************************************

namespace Yu
{
    public partial class ActionManager
    {
        /// <summary>
        /// 注册所有action
        /// </summary>
        private void LoadInitAtomAction()
        {
            // RegisterAtomAction(203, StopBGM);
            // RegisterAtomAction(204, StopBGMDelay);
            // RegisterAtomAction(205, PlayBGM);
            // RegisterAtomAction(206, PlayBGMDelay);
            // RegisterAtomAction(207, PlayAbbBGM);
            // RegisterAtomAction(208, SetBGMVolume);
            // RegisterAtomAction(209, MuteBGM);
            RegisterAtomAction(210, JumpToDialogueWithCond);
        }

        /// <summary>
        /// 跳转至指定对话，带条件判断
        /// </summary>
        private static void JumpToDialogueWithCond(string[] args)
        {
            if (args.Length < 3)
            {
                GameLog.Error("事件参数不对", "JumpToDialogueWithCond", args.Length);
                return;
            }

            var conditionId = int.Parse(args[0]);
            var dialogueIdTrue = int.Parse(args[1]);
            var dialogueIdFalse = int.Parse(args[2]);
            if (ConditionManager.ExecuteCondition(conditionId))
            {
                DialogueManager.Instance.JumpTpDialogue(dialogueIdTrue);
                return;
            }
            
            DialogueManager.Instance.JumpTpDialogue(dialogueIdFalse);
        }
    }
}