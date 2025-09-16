// ******************************************************************
//@file         DialogueOption.cs
//@brief        剧情选项对象
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.31 21:52:19 
// ******************************************************************

namespace Yu
{
    public class DialogueOption : IPoolableObject
    {
        public RowCfgDialogueOption RowCfg;

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