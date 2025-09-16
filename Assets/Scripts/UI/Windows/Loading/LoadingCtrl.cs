// ******************************************************************
//@file         LoadingCtrl.cs
//@brief        加载界面的controller
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:35:35
// ******************************************************************

using System.Collections;
using Yu;

public class LoadingCtrl : UICtrlBase
{
    private LoadingModel _model;
    private LoadingView _view;


    public override void OnInit(params object[] param)
    {
        _model = new LoadingModel();
        _view = GetComponent<LoadingView>();
    }

    public override void OpenRoot(params object[] param)
    {
        _view.OpenRoot();
    }

    public override void CloseRoot()
    {
        _view.CloseRoot();
    }

    public override void OnClear()
    {
    }

    public override void BindEvent()
    {
    }

    /// <summary>
    /// 手动播放Open动画，带动画时长
    /// </summary>
    public IEnumerator OpenRootCo() => _view.OpenRootCo();
    
    /// <summary>
    /// 手动播放Close动画，带动画时长
    /// </summary>
    public IEnumerator CloseRootCo() => _view.CloseRootCo();
}