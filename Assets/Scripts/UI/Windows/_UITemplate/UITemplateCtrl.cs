using Yu;

public class UITemplateCtrl : UICtrlBase
{
    private UITemplateModel _model;
    private UITemplateView _view;


    public override void OnInit(params object[] param)
    {
        _model = new UITemplateModel();
        _view = GetComponent<UITemplateView>();
        _model.OnInit();
    }

    public override void OpenRoot(params object[] param)
    {
        _model.OnOpen();
        _view.OpenWindow();
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
        // _view.btnXXX.onClick.AddListener(BtnOnClickXXX);
    }

    /// <summary>
    /// XXX
    /// </summary>
    private void BtnOnClickXXX()
    {
        CloseRoot();
    }
}