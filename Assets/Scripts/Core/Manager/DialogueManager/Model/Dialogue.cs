// ******************************************************************
//@file         Dialogue.cs
//@brief        剧情对话对象
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.31 22:26:18 
// ******************************************************************

namespace Yu
{
    public class Dialogue : IPoolableObject
    {
        public RowCfgDialogue RowCfg;
        public int DialogueId => RowCfg?.Id ?? -1;
        public int SelectedOptionId; //选择的对话选项id
        public int HistoryIndex; //对话历史index

        public void OnActivate()
        {
            Active = true;
        }

        public void OnDeactivate()
        {
            Active = false;
        }

        public void OnIdleDestroy()
        {
        }

        public float LastUsedTime { get; }
        public bool Active { get; private set; }
    }
}