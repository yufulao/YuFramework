// ******************************************************************
//@file         DialogueCtrl.cs
//@brief        剧情对话界面
//@author       yufulao, yufulao@qq.com
//@createTime   2025.09.01 00:04:14 
// ******************************************************************
using Yu;

public partial class DialogueCtrl : UICtrlBase
{
    private DialogueModel _model;
    private DialogueView _view;


    public override void OnInit(params object[] param)
    {
        _model = new DialogueModel();
        _view = GetComponent<DialogueView>();
        _model.OnInit();
        EventManager.Instance.AddListener(EventName.OnConversationStart, OnConversationStart);
        EventManager.Instance.AddListener(EventName.OnConversationStop, OnConversationStop);
        EventManager.Instance.AddListener(EventName.OnDialogueChange, OnDialogueChange);
        EventManager.Instance.AddListener(EventName.OnConversationPause, OnConversationPause);
        EventManager.Instance.AddListener(EventName.OnConversationUnpause, OnConversationUnpause);
    }

    public override void OpenRoot(params object[] param)
    {
        _model.OnOpen();
        _view.OpenWindow();
        _view.HideAllPortrait();
        SetBtnNextInteractable(true);
        
        ActionManager.Instance.RegisterAtomAction(100, SetPortrait);
        ActionManager.Instance.RegisterAtomAction(102, HighLightPortrait);
    }

    public override void CloseRoot()
    {
        _view.CloseWindow();
        
        ActionManager.Instance.RemoveAtomAction(100);
        ActionManager.Instance.RemoveAtomAction(102);
    }

    public override void OnClear()
    {
        EventManager.Instance.RemoveListener(EventName.OnConversationStart, OnConversationStart);
        EventManager.Instance.RemoveListener(EventName.OnConversationStop, OnConversationStop);
        EventManager.Instance.RemoveListener(EventName.OnDialogueChange, OnDialogueChange);
        EventManager.Instance.RemoveListener(EventName.OnConversationPause, OnConversationPause);
        EventManager.Instance.RemoveListener(EventName.OnConversationUnpause, OnConversationUnpause);
    }

    public override void BindEvent()
    {
        _view.btnNext.onClick.AddListener(OnBtnClickNext);
        _view.gridViewDialogueOption.OnItemClicked(OnDialogueOptionClick);
    }

    /// <summary>
    /// 剧情开始
    /// </summary>
    private void OnConversationStart()
    {
        if (gameObject.activeInHierarchy)
        {
            return;
        }
        
        OpenRoot();
    }
    
    /// <summary>
    /// 剧情停止
    /// </summary>
    private void OnConversationStop()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        
        CloseRoot();
    }
    
    /// <summary>
    /// 剧情对话变更
    /// </summary>
    private void OnDialogueChange()
    {
        UpdateDialogue();
        UpdateDialogueOption();
        AutoSetBtnNextInteractable();
    }
    
    /// <summary>
    /// 剧情暂停
    /// </summary>
    private void OnConversationPause()
    {
    }
    
    /// <summary>
    /// 剧情取消暂停
    /// </summary>
    private void OnConversationUnpause()
    {
    }

    /// <summary>
    /// 更新对话
    /// </summary>
    private void UpdateDialogue()
    {
        var dialogue = DialogueManager.Instance.CurDialogue;
        var textId = dialogue.RowCfg.TextId;
        if (textId == default)
        {
            return;
        }
        
        _view.textDialogue.UpdateText(textId);
    }
    
    /// <summary>
    /// 更新对话选项
    /// </summary>
    private void UpdateDialogueOption()
    {
        var dialogueOptionList = DialogueManager.Instance.CurOptionList;
        _view.gridViewDialogueOption.UpdateContents(dialogueOptionList);
    }
    
    /// <summary>
    /// 自动判断btnNext是否可以交互
    /// </summary>
    private void AutoSetBtnNextInteractable()
    {
        var dialogue = DialogueManager.Instance.CurDialogue;
        var dialogueOptionList = DialogueManager.Instance.CurOptionList;
        var canNotClickNext = dialogue.RowCfg.NextId == default && dialogueOptionList.Count > 0;
        SetBtnNextInteractable(!canNotClickNext);
    }

    /// <summary>
    /// 设置btnNext是否可以交互
    /// </summary>
    private void SetBtnNextInteractable(bool interactable)
    {
        _view.btnNext.interactable = interactable;
    }
    
    /// <summary>
    /// 点击BtnNext
    /// </summary>
    private void OnBtnClickNext()
    {
        DialogueManager.Instance.NextDialogue();
    }
    
    /// <summary>
    /// 点击对话选项
    /// </summary>
    private void OnDialogueOptionClick(int index)
    {
        DialogueManager.Instance.SelectOption(index);
    }
}