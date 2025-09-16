// ******************************************************************
//@file         DialogueOptionGridCell.cs
//@brief        对话选项列表cell
//@author       yufulao, yufulao@qq.com
//@createTime   2025.08.31 23:29:40 
// ******************************************************************

using UnityEngine;
using Yu;

public class DialogueOptionGridCell : BtnGridCell<DialogueOption, DialogueOptionGridContext>
{
    [SerializeField] private LocalizeTextMeshProUGUI text;

    public override void UpdateContent(DialogueOption dialogueOption)
    {
        var rowCfg = dialogueOption.RowCfg;
        text.UpdateText(rowCfg.TextId);
    }
}