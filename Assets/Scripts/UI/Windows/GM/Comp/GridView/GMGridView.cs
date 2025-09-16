using System;
using Yu;

public class GMGridView :BtnGridView<GMMethodData, GMGridContext, GMGridGroup>
{
    public override void OnItemClicked(Action<int> callback)
    {
        Context.OnItemClicked = callback;
    }
    
    /// <summary>
    /// 当item点击execute时
    /// </summary>
    public virtual void OnItemClickExecuteBtn(Action<GMGridData> callback)
    {
        Context.OnBtnClickExecute = callback;
    }
}