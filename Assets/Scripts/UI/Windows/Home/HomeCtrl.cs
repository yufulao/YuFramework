// ******************************************************************
//@file         HomeCtrl.cs
//@brief        主界面controller
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.22 20:38:41
// ******************************************************************

using Yu;

public class HomeCtrl : UICtrlBase
{
    private HomeModel _model;
    private HomeView _view;

    public override void OnInit(params object[] param)
    {
        _model = new HomeModel();
        _view = GetComponent<HomeView>();
    }

    public override void OpenRoot(params object[] param)
    {
        _view.OnOpen();
    }

    public override void CloseRoot()
    {
        _view.OnClose();
    }

    public override void OnClear()
    {
    }

    public override void BindEvent()
    {

    }
}