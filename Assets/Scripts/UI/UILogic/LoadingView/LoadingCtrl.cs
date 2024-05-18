// ******************************************************************
//@file         LoadingCtrl.cs
//@brief        加载界面的controller
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:35:35
// ******************************************************************

namespace Yu
{
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
            _view.OpenWindow();
        }

        public override void CloseRoot()
        {
            _view.CloseWindow();
        }

        public override void BindEvent()
        {
        }

    }
}