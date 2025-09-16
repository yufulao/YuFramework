// ******************************************************************
//@file         GMCommandTab.cs
//@brief        GM界面
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:36:21
// ******************************************************************

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Yu;

public class GMCommandTab : MonoBehaviour
{
    private GMCommandTabModel _model;
    [SerializeField] private InputField inputFieldCommand;
    [SerializeField] private RectTransform commandSuggestionsContainer;
    [SerializeField] private Text commandSuggestionPrefab;
    [SerializeField] private string commandSuggestionHighlightStart = "<color=orange>";
    [SerializeField] private string commandSuggestionHighlightEnd = "</color>";
    private List<Text> _commandSuggestionInstances;
    private int _visibleCommandSuggestionInstances = 0;
    private StringBuilder _sharedStringBuilder; //刷新单个补全指令的内容，缓存下来的StringBuilder
    public bool commandInputFieldAutoCompletedNow; //当前补全的指令
    public string commandInputFieldAutoCompleteBase; //tab切换提示词时缓存的前缀
    public bool isChangeSuggestion;

    private void Start()
    {
        OnInit();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void OnInit()
    {
        inputFieldCommand.onValidateInput += OnValidateInput; //输入字符回调
        inputFieldCommand.onValueChanged.AddListener(OnValueChanged); //内容更改回调
        inputFieldCommand.onEndEdit.AddListener(OnEndEdit);
        _model = new GMCommandTabModel();
        _model.OnInit();
        _commandSuggestionInstances = new List<Text>(8);
        _sharedStringBuilder = new StringBuilder(1024);
        isChangeSuggestion = false;
    }

    /// <summary>
    /// 添加字符之前的回调
    /// </summary>
    private char OnValidateInput(string text, int charIndex, char addedChar)
    {
        if (!GMCommand.HadInit)
        {
            return '\0';
        }

        switch (addedChar)
        {
            case '\t':
            {
                isChangeSuggestion = true;
                if (!string.IsNullOrEmpty(text))
                {
                    if (string.IsNullOrEmpty(commandInputFieldAutoCompleteBase)) //内容更改时会重置缓存前缀
                    {
                        commandInputFieldAutoCompleteBase = text;
                    }

                    //commandInputFieldAutoCompleteBase必不为空
                    var autoCompletedCommand = GMCommand.GetAutoCompleteCommand(commandInputFieldAutoCompleteBase, text);
                    if (!string.IsNullOrEmpty(autoCompletedCommand) && autoCompletedCommand != text)
                    {
                        commandInputFieldAutoCompletedNow = true;
                        inputFieldCommand.text = autoCompletedCommand; //此处替换text时，每一个char执行一次OnValidateCommand
                    }
                }

                isChangeSuggestion = false; //！！！所以在替换text后恢复isChangeSuggestion，不然补全的第一个char就恢复，然后就更新指令提示了
                return '\0';
            }

            case '\n':
            {
                if (text.Length > 0)
                {
                    GMCommand.ExecuteCommand(text);
                }

                return '\0';
            }

            default:
                return addedChar;
        }
    }

    /// <summary>
    /// 内容更改时回调
    /// </summary>
    private void OnValueChanged(string command)
    {
        if (!GMCommand.HadInit)
        {
            return;
        }

        if (!isChangeSuggestion) //如果不是tab切换指令提示时才刷新提示
        {
            RefreshCommandSuggestions(command); //刷新补全指令提示列表
        }

        if (!commandInputFieldAutoCompletedNow)
        {
            commandInputFieldAutoCompleteBase = null; //如果不是自动补全的，即客户自行修改的，就重置缓存前缀
            return;
        }

        commandInputFieldAutoCompletedNow = false;
    }

    /// <summary>
    /// inputField失去聚焦
    /// </summary>
    private void OnEndEdit(string command)
    {
        if (commandSuggestionsContainer.gameObject.activeSelf)
        {
            commandSuggestionsContainer.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 刷新补全指令提示列表
    /// </summary>
    private void RefreshCommandSuggestions(string command)
    {
        var caretPos = inputFieldCommand.caretPosition;
        var (commandChanged, commandNameOrParametersChanged) = _model.SetCommandSuggestions(command);
        var caretArgumentIndex = 0;
        for (var i = 0; i < _model.CommandCaretIndexIncrements.Count && caretPos > _model.CommandCaretIndexIncrements[i]; i++)
        {
            caretArgumentIndex++;
        }

        if (!_model.CheckNeedToRefreshSuggestion(caretArgumentIndex, commandChanged, commandNameOrParametersChanged))
        {
            return;
        }

        if (_model.MatchingCommandSuggestions.Count == 0)
        {
            OnEndEdit(command);
            return;
        }

        OpenSuggestionContainer(caretArgumentIndex);
    }

    /// <summary>
    /// 打开代码补全提示界面
    /// </summary>
    private void OpenSuggestionContainer(int caretArgumentIndex)
    {
        commandSuggestionsContainer.gameObject.SetActive(true); //打开补全提示界面
        var suggestionInstancesCount = _commandSuggestionInstances.Count;
        var suggestionsCount = _model.MatchingCommandSuggestions.Count;

        for (var i = 0; i < suggestionsCount; i++)
        {
            RefreshCommandSuggestionInstance(i, suggestionInstancesCount);
            RefreshRefreshCommandSuggestionInstanceContent(i, caretArgumentIndex);
        }

        for (var i = _visibleCommandSuggestionInstances - 1; i >= suggestionsCount; i--)
        {
            _commandSuggestionInstances[i].gameObject.SetActive(false);
        }

        _visibleCommandSuggestionInstances = suggestionsCount;
    }

    /// <summary>
    /// 刷新单个补全指令提示
    /// </summary>
    private void RefreshCommandSuggestionInstance(int index, int currentMaxCount)
    {
        if (index < _visibleCommandSuggestionInstances)
        {
            return;
        }

        _visibleCommandSuggestionInstances++;

        if (index >= currentMaxCount) //简易对象池
        {
            _commandSuggestionInstances.Add(Instantiate(commandSuggestionPrefab, commandSuggestionsContainer, false));
            return;
        }

        _commandSuggestionInstances[index].gameObject.SetActive(true);
    }

    /// <summary>
    /// 刷新单个补全指令提示的内容
    /// </summary>
    private void RefreshRefreshCommandSuggestionInstanceContent(int index, int caretArgumentIndex)
    {
        PreSetSharedStringBuilder(index, caretArgumentIndex);
        var suggestedCommand = _model.MatchingCommandSuggestions[index];
        //配置参数
        if (suggestedCommand.ParametersData.Length > 0)
        {
            _sharedStringBuilder.Append(" ");

            //高亮显示参数
            var caretParameterIndex = caretArgumentIndex - 1;
            if (caretParameterIndex >= suggestedCommand.ParametersData.Length)
            {
                caretParameterIndex = suggestedCommand.ParametersData.Length - 1;
            }

            for (var j = 0; j < suggestedCommand.ParametersData.Length; j++)
            {
                var parameterName = "[" + suggestedCommand.ParametersData[j].ParameterName + "]";
                if (caretParameterIndex != j)
                {
                    _sharedStringBuilder.Append(parameterName);
                    continue;
                }

                _sharedStringBuilder.Append(commandSuggestionHighlightStart).Append(parameterName).Append(commandSuggestionHighlightEnd);
            }
        }

        _commandSuggestionInstances[index].text = _sharedStringBuilder.ToString();
    }

    /// <summary>
    /// 预设置补全代码提示的内容
    /// </summary>
    private void PreSetSharedStringBuilder(int index, int caretArgumentIndex)
    {
        var suggestedCommand = _model.MatchingCommandSuggestions[index];
        _sharedStringBuilder.Length = 0;
        if (caretArgumentIndex > 0)
        {
            _sharedStringBuilder.Append(suggestedCommand.Method.Name);
            return;
        }

        _sharedStringBuilder.Append(commandSuggestionHighlightStart).Append(_model.MatchingCommandSuggestions[index].Method.Name).Append(commandSuggestionHighlightEnd);
    }
}