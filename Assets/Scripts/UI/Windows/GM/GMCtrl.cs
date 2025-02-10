using UnityEngine;
using Yu;

/// <summary>
/// GM指令界面controller
/// </summary>
public class GMCtrl : UICtrlBase
{
    private GMModel _model;
    private GMView _view;


    public override void OnInit(params object[] param)
    {
        _model = new GMModel();
        _view = GetComponent<GMView>();
        if (!_model.TryInit())
        {
            return;
        }

        _view.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        if (!_model.TryInit())
        {
            return;
        }

        _model.OnOpen();
        _view.OpenWindow();
        
        _view.gridView.UpdateContents(_model.MethodDataList);
    }

    public override void CloseRoot()
    {
        _view.CloseWindow();
    }

    public override void OnClear()
    {
    }

    public override void BindEvent()
    {
        _view.btnBack.onClick.AddListener(OnBtnClickBack);
        _view.gridView.OnItemClickExecuteBtn(OnBtnClickExecute);
    }
    
    /// <summary>
    /// 打开GMView调试界面
    /// </summary>
    public static void OpenGmView()
    {
        if (UIManager.Instance.CheckViewActiveInHierarchy("GMView"))
        {
            UIManager.Instance.CloseWindow("GMView");
            return;
        }
            
        UIManager.Instance.OpenWindow("GMView");
    }

    /// <summary>
    /// 点击了cell的execute按钮
    /// </summary>
    private void OnBtnClickExecute(GMGridData gridData)
    {
        var methodData = _model.MethodDataList[gridData.Index];
        GMCommand.ExecuteCommand(methodData, gridData.ParameterArray);
    }

    /// <summary>
    /// 点击了back按钮
    /// </summary>
    private void OnBtnClickBack()
    {
        CloseRoot();
    }
}