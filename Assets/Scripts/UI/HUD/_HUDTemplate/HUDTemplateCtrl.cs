using Yu;

public class HUDTemplateCtrl : HUDBase
{
    private HUDTemplateModel _model;
    private HUDTemplateView _view;


    public override void OnInit()
    {
        _model = new HUDTemplateModel();
        _view = GetComponent<HUDTemplateView>();
        _model.OnInit();
    }
    
    public override void BindEvent()
    {
    }
    
    public override void CloseRoot()
    {
        _view.CloseWindow();
    }

    public override void OnDestroy()
    {
        
    }
}