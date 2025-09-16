// ******************************************************************
//@file         DialogueManager.cs
//@brief        剧情管理器
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.31 21:52:08 
// ******************************************************************

using System.Collections.Generic;

namespace Yu
{
    public class DialogueManager : BaseSingleTon<DialogueManager>, IMonoManager
    {
        public List<Dialogue> DialogueHistory{ get; private set; } //对话id历史记录
        public Dialogue CurDialogue { get; private set; } //当前对话
        public List<DialogueOption> CurOptionList{ get; private set; } //对话选项列表

        public void OnInit()
        {
            DialogueHistory = new List<Dialogue>();
            CurOptionList = new List<DialogueOption>();
            PoolManager.Instance.CreatePool(4, GeneratorDialogueOption);
            PoolManager.Instance.CreatePool(20, GeneratorDialogue);
            UIManager.Instance.CreatNewView<DialogueCtrl>("DialogueView");
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void LateUpdate()
        {
        }

        public void OnClear()
        {
        }

        /// <summary>
        /// 开始剧情
        /// </summary>
        public void StartConversation(int dialogueId)
        {
            ClearDialogueHistory();
            EventManager.Instance.Dispatch(EventName.OnConversationStart);
            SetCurDialogue(dialogueId);
        }
        
        /// <summary>
        /// 暂停剧情
        /// </summary>
        public void PauseConversation()
        {
            EventManager.Instance.Dispatch(EventName.OnConversationPause);
        }
        
        /// <summary>
        /// 继续剧情
        /// </summary>
        public void UnpauseConversation()
        {
            EventManager.Instance.Dispatch(EventName.OnConversationUnpause);
        }

        /// <summary>
        /// 停止剧情
        /// </summary>
        public void StopConversation()
        {
            EventManager.Instance.Dispatch(EventName.OnConversationStop);
            CurDialogue = null;
            ClearDialogueHistory();
            ClearCurDialogueOptionList();
        }

        /// <summary>
        /// 下一个对话
        /// </summary>
        public void NextDialogue()
        {
            if (!CheckHaveCurDialogue())
            {
                return;
            }
            
            var nextDialogueId = CurDialogue.RowCfg.NextId;
            if (nextDialogueId == default)
            {
                StopConversation();
                return;
            }
            
            SetCurDialogue(nextDialogueId);
        }
        
        /// <summary>
        /// 跳转至指定对话
        /// </summary>
        public void JumpTpDialogue(int dialogueId)
        {
            if (!CheckHaveCurDialogue())
            {
                return;
            }

            SetCurDialogue(dialogueId);
        }
        
        /// <summary>
        /// 选择对话选项
        /// </summary>
        public void SelectOption(int index)
        {
            if (index < 0 || index >= CurOptionList.Count)
            {
                GameLog.Error("当前选择的选项index错误", index, Utils.ToStringCollection(CurOptionList));
                return;
            }

            var option = CurOptionList[index];
            var rowCfgDialogueOption = option.RowCfg;
            CurDialogue.SelectedOptionId = rowCfgDialogueOption.Id;
            SetCurDialogue(rowCfgDialogueOption.NextId);
        }

        /// <summary>
        /// 设置当前对话
        /// </summary>
        private void SetCurDialogue(int dialogueId)
        {
            CurDialogue = GetDialogueById(dialogueId);
            CurDialogue.HistoryIndex = DialogueHistory.Count;
            DialogueHistory.Add(CurDialogue);
            SetCurOptions();
            ExecuteCurActions();
            EventManager.Instance.Dispatch(EventName.OnDialogueChange);
        }

        /// <summary>
        /// 设置当前对话选项
        /// </summary>
        private void SetCurOptions()
        {
            ClearCurDialogueOptionList();
            CurDialogue.SelectedOptionId = -1;
            var optionIds = CurDialogue.RowCfg.OptionIds;
            if (optionIds == null)
            {
                return;
            }
            
            foreach (var optionId in optionIds)
            {
                var rowCfgDialogueOption = ConfigManager.Tables.CfgDialogueOption.GetOrDefault(optionId);
                if (rowCfgDialogueOption == null)
                {
                    GameLog.Error("没有该对话选项id", optionId);
                    continue;
                }

                var optionCondId = rowCfgDialogueOption.CondId;
                if (optionCondId != default && !ConditionManager.ExecuteCondition(optionCondId))
                {
                    continue;
                }
                    
                var dialogueOption = PoolManager.Instance.GetObject<DialogueOption>();
                dialogueOption.RowCfg = rowCfgDialogueOption;
                CurOptionList.Add(dialogueOption);
            }
        }

        /// <summary>
        /// 执行对话事件
        /// </summary>
        private void ExecuteCurActions()
        {
            var actionIds = CurDialogue.RowCfg.ActionIds;
            if (actionIds != null)
            {
                foreach (var actionId in actionIds)
                {
                    ActionManager.Instance.ExecuteAction(actionId);
                }
            }

            var atomActionIds = CurDialogue.RowCfg.AtomActionIds;
            if (atomActionIds != null)
            {
                for (var i = 0; i < atomActionIds.Count; i++)
                {
                    var atomActionId = atomActionIds[i];
                    var atomActionParams = CurDialogue.RowCfg.AtomActionParamList[i];
                    ActionManager.Instance.ExecuteAtomAction(atomActionId, atomActionParams);
                }
            }
        }

        /// <summary>
        /// 获取dialogue对象
        /// </summary>
        /// <returns></returns>
        private Dialogue GetDialogueById(int dialogueId)
        {
            var rowCfgDialogue = ConfigManager.Tables.CfgDialogue.GetOrDefault(dialogueId);
            if (rowCfgDialogue == null)
            {
                GameLog.Error("没有该对话id", dialogueId);
                return null;
            }

            var dialogue = PoolManager.Instance.GetObject<Dialogue>();
            dialogue.RowCfg = rowCfgDialogue;
            return dialogue;
        }

        /// <summary>
        /// 检查是否有当前对话
        /// </summary>
        /// <returns></returns>
        private bool CheckHaveCurDialogue()
        {
            if (CurDialogue != null)
            {
                return true;
            }
            
            GameLog.Error("当前对话为空");
            StopConversation();
            return false;
        }

        /// <summary>
        /// 清空对话历史
        /// </summary>
        private void ClearDialogueHistory()
        {
            foreach (var dialogue in DialogueHistory)
            {
                PoolManager.Instance.ReturnObject(dialogue);
            }
            
            DialogueHistory.Clear();
        }

        /// <summary>
        /// 清空当前对话选项
        /// </summary>
        private void ClearCurDialogueOptionList()
        {
            foreach (var dialogueOption in CurOptionList)
            {
                PoolManager.Instance.ReturnObject(dialogueOption);
            }
            
            CurOptionList.Clear();
        }

        /// <summary>
        /// 创建dialogueOption对象
        /// </summary>
        private static DialogueOption GeneratorDialogueOption()
        {
            return new();
        }

        /// <summary>
        /// 创建dialogue对象
        /// </summary>
        /// <returns></returns>
        private static Dialogue GeneratorDialogue()
        {
            return new();
        }
    }
}