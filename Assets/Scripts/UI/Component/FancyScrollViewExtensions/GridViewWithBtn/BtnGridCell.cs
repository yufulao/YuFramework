// ******************************************************************
//@file         GridCellWithBtn.cs
//@brief        带默认btn的cell
//@author       yufulao, yufulao@qq.com
//@createTime   2025.05.10 16:41:04 
// ******************************************************************

using UnityEngine;
using UnityEngine.UI;

public class BtnGridCell<TData, TContext> : GridCell<TData, TContext> where TContext : BtnGridContext, new()
{
    [SerializeField] protected Button btn;
    // private PointerUIComponent _pointerUIComponent;

    public override void Initialize()
    {
        if (btn)
        {
            btn.onClick?.AddListener(OnBtnClick);
        }
    }

    /// <summary>
    /// 点击btn时
    /// </summary>
    protected virtual void OnBtnClick()
    {
        // ExecuteEvents.Execute(_pointerUIComponent.gameObject, null, ExecuteEvents.pointerClickHandler);
        Context.OnItemClicked?.Invoke(Index);
    }
}