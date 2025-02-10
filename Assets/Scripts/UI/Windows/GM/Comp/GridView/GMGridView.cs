using System;
using FancyScrollView;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public class GMGridView :GridView<GMMethodData, GMGridContext, GMGridGroup>
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