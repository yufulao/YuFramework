// ******************************************************************
//@file         GMCtrl.cs
//@brief        GM指令界面controller
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.27 15:12:57
// ******************************************************************

using System;
using System.Collections.Generic;
using Yu;

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

        InitGmTab(_model.CommandCatalogSet);
    }

    public override void OpenRoot(params object[] param)
    {
        if (!_model.TryInit())
        {
            return;
        }

        _model.OnOpen();
        _view.OpenWindow();
        //
        // _view.gridView.UpdateContents(_model.DefaultCommandsList); // 默认显示第一个指令类型的指令列表
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
        // BindEvent4EachCommandBtn();
    }

    /// <summary>
    /// 点击了cell的execute按钮
    /// </summary>
    private void OnBtnClickExecute(GMGridData gridData)
    {
        var commandData = _model.CurrentCommandList[gridData.Index];
        GMCommand.ExecuteCommand(commandData, gridData.ParameterArray);
    }

    /// <summary>
    /// 点击了back按钮
    /// </summary>
    private void OnBtnClickBack()
    {
        CloseRoot();
    }

    /// <summary>
    /// 通过指令类别刷新列表
    /// </summary>
    private void UpdateGridContent(string catalog)
    {
        _model.FilterByCatalog(catalog);
        _view.gridView.UpdateContents(_model.CurrentCommandList);
    }
    
    /// <summary>
    /// 创建GM指令Tab
    /// </summary>
    private void InitGmTab(IEnumerable<string> catalogList)
    {
        foreach (var catalog in catalogList)
        {
            var proto = AssetManager.LoadAssetGameObject(ConfigManager.Tables.CfgPrefab["GMTab"].PrefabPath);
            var tab = Instantiate(proto, _view.containerGmTab.transform).GetComponent<GMTab>();
            tab.text.text = catalog;
            tab.toggle.group = _view.toggleGroupTab;
            tab.toggle.onValueChanged.AddListener(isOn => OnToggleValueChangeGmTab(isOn, catalog));
        }
    }

    /// <summary>
    /// 切换tab的toggle时
    /// </summary>
    private void OnToggleValueChangeGmTab(bool isOn, string catalog)
    {
        if (!isOn)
        {
            return;
        }

        UpdateGridContent(catalog);
    }

    // /// <summary>
    // /// 为每个指令按钮绑定事件
    // /// </summary>
    // private void BindEvent4EachCommandBtn()
    // {
    //     foreach (var btn in _view.gmBtnList)
    //     {
    //         btn.onClick.AddListener(OnBtnClickCommandView);
    //     }
    // }

    // /// <summary>
    // /// GM指令类型按钮被点击时，切换到对应的指令列表
    // /// </summary>
    // private void OnBtnClickCommandView()
    // {
    //     var currentCommands = GetCommandsByTypeName();
    //     _view.gridView.UpdateContents(currentCommands);
    // }

    // /// <summary>
    // /// 获取特定指令类型的指令列表
    // /// </summary>
    // /// <returns></returns>
    // private List<GMMethodData> GetCommandsByTypeName()
    // {
    //     var btnName = EventSystem.current.currentSelectedGameObject.name;
    //     var commandType = _model.GetTypeByTypeName(btnName);
    //     var commandsList = _model.GetCommandsByType(commandType);
    //     return commandsList;
    // }
}