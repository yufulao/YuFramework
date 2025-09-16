// ******************************************************************
//@file         GMCommandTabModel.cs
//@brief        GM界面的model
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:35:55
// ******************************************************************
using System.Collections.Generic;
using Yu;

public class GMCommandTabModel
{
    public List<GMMethodData> MatchingCommandSuggestions;
    public List<int> CommandCaretIndexIncrements;
    private string _commandInputFieldPrevCommand;
    private string _commandInputFieldPrevCommandName;
    private int _commandInputFieldPrevCaretArgumentIndex = -1;

    
    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        MatchingCommandSuggestions = new List<GMMethodData>(8);
        CommandCaretIndexIncrements = new List<int>(8);
    }

    /// <summary>
    /// 设置补全指令提示列表
    /// </summary>
    public (bool,bool) SetCommandSuggestions(string command)
    {
        var commandChanged = command != _commandInputFieldPrevCommand;
        var commandNameOrParametersChanged = false;
        if (!commandChanged)
        {
            return (false, false);
        }
        
        _commandInputFieldPrevCommand = command;
        MatchingCommandSuggestions.Clear();
        CommandCaretIndexIncrements.Clear();

        var prevCommandName = _commandInputFieldPrevCommandName;
        //装填matchingCommandSuggestions
        _commandInputFieldPrevCommandName = GMCommand.GetCommandSuggestions(command, MatchingCommandSuggestions, CommandCaretIndexIncrements);
        if (prevCommandName != _commandInputFieldPrevCommandName /*|| numberOfParameters != commandInputFieldPrevParamCount*/)
        {
            //commandInputFieldPrevParamCount = numberOfParameters;
            commandNameOrParametersChanged = true;
        }

        return (true, commandNameOrParametersChanged);
    }
    
    /// <summary>
    /// 检测是否需要刷新代码补全提示
    /// </summary>
    public bool CheckNeedToRefreshSuggestion(int caretArgumentIndex, bool commandChanged, bool commandNameOrParametersChanged)
    {
        if (caretArgumentIndex == _commandInputFieldPrevCaretArgumentIndex)
        {
            return commandChanged && commandNameOrParametersChanged;
        }

        _commandInputFieldPrevCaretArgumentIndex = caretArgumentIndex;
        return true;
    }
}
