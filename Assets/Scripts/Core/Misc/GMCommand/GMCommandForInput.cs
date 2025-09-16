// ******************************************************************
//@file         GMCommandForInput.cs
//@brief        GM指令（处理代码输入部分）
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.26 13:23:42
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Yu
{
    public partial class GMCommand
    {
        private static readonly List<GMMethodData> MatchingMethods = new(4); //runtime匹配的函数
        private static readonly List<string> CommandArguments = new(8); //实参数据，[0]是指令名
        private static readonly string[] InputDelimiters = { "\"\"", "''", "{}", "()", "[]" }; //指令的分隔符


        /// <summary>
        /// 通过指令名执行指令
        /// </summary>
        public static void ExecuteCommand(string commandName)
        {
            if (!PrepareBeforeExecute(ref commandName))
            {
                return;
            }

            var parameters = new object[CommandArguments.Count - 1]; //CommandArguments[0]是command
            var methodToExecute = SetCommandParameters(parameters); //装填指令的参数值
            if (methodToExecute == null)
            {
                GameLog.Warn("找不到method->" + commandName);
                return;
            }

            ExecuteCommand(methodToExecute, parameters);
        }

        /// <summary>
        /// 获取下一条自动补全的指令，摁tab时调用
        /// </summary>
        public static string GetAutoCompleteCommand(string commandPrefix, string currentSuggestion)
        {
            // var currentInput = string.IsNullOrEmpty(currentSuggestion) ?commandPrefix  : currentSuggestion;
            var commandIndex = FindCommandIndex(currentSuggestion);
            if (commandIndex < 0)
            {
                return GetCommandSuggestionWhenNotMatch(commandIndex, commandPrefix);
            }

            //如果找到一样的指令，获取下一条补全指令
            for (var i = commandIndex + 1; i < MethodDataList.Count; i++)
            {
                var methodName = MethodDataList[i].Method.Name;
                //和当前选择的补全指令完全相同，就找下一条
                if (GMCommandUtils.IsSame(methodName, currentSuggestion) == 0)
                {
                    continue;
                }

                //和当前选择的补全指令不同，就获取下一条前缀相同的补全指令
                if (GMCommandUtils.IsPrefix(methodName, commandPrefix))
                {
                    return methodName;
                }

                break;
            }

            //找不到下一条，循环回到首条匹配的，没有就只有自己，回到自己
            for (var i = 0; i < commandIndex + 1; i++)
            {
                var methodName = MethodDataList[i].Method.Name;
                if (GMCommandUtils.IsPrefix(methodName, commandPrefix))
                {
                    return methodName;
                }
            }

            return null;
        }

        /// <summary>
        /// 装填代码补全提示列表matchingCommands，并返回经分隔符裁剪好的指令
        /// </summary>
        public static string GetCommandSuggestions(string commandOriginal, List<GMMethodData> matchingCommands, List<int> caretIndexIncrements)
        {
            //commandNameFullyTyped是command是否完全输入，numberOfParameters是指令的参数个数
            var (commandResult, commandNameFullyTyped, numberOfParameters) = GetCommandByDelimiter(commandOriginal, caretIndexIncrements, false);
            if (string.IsNullOrEmpty(commandResult))
            {
                return null;
            }

            var (commandIndex, commandLastIndex) = commandNameFullyTyped ? GetCommandIndexByFull(commandResult) : GetCommandIndexByPrefix(commandResult);
            if (commandLastIndex < 0)
            {
                return commandResult;
            }

            for (var i = commandIndex; i < commandLastIndex + 1; i++)
            {
                //匹配指令的参数个数
                if (MethodDataList[commandIndex].ParametersData.Length >= numberOfParameters)
                {
                    matchingCommands.Add(MethodDataList[i]);
                }
            }

            return commandResult;
        }

        /// <summary>
        /// 执行前的预处理，检测指令是否合规，并设置MatchingMethods
        /// </summary>
        private static bool PrepareBeforeExecute(ref string commandName)
        {
            if (commandName == null)
            {
                return false;
            }

            commandName = commandName.Trim();
            if (commandName.Length == 0)
            {
                return false;
            }

            CommandArguments.Clear();
            FetchArgumentsFromCommand(commandName, CommandArguments); //裁剪指令，拆分传入的参数值，CommandArguments[0]是指令
            MatchingMethods.Clear();
            var parameterCountMismatch = PrepareBeforeExecuteByFull();

            //没有参数个数正确的指令
            if (MatchingMethods.Count != 0)
            {
                return true;
            }

            var command = CommandArguments[0];
            FindCommands(command, MatchingMethods, !parameterCountMismatch); //尝试前缀匹配

            if (MatchingMethods.Count == 0)
            {
                GameLog.Warn("找不到指令" + command);
                return false;
            }

            if (parameterCountMismatch)
            {
                GameLog.Warn("指令参数不匹配" + command);
                return false;
            }

            GameLog.Warn("找不到指令" + command);
            return false;
        }

        /// <summary>
        /// 执行指令前，装填指令的参数值
        /// </summary>
        private static GMMethodData SetCommandParameters(object[] parameters)
        {
            GMMethodData resultMethod = null;
            //装填MatchingMethods最末尾匹配到的指令
            foreach (var method in MatchingMethods)
            {
                if (SetCommandParametersPer(method, parameters)) //装填单条指令的参数值
                {
                    resultMethod = method;
                }
            }

            return resultMethod;
        }

        /// <summary>
        /// 装填单条指令的参数值
        /// </summary>
        private static bool SetCommandParametersPer(GMMethodData method, object[] parameters)
        {
            var success = true;
            for (var i = 0; i < method.ParametersData.Length && success; i++)
            {
                try
                {
                    var argument = CommandArguments[i + 1];
                    var parameterData = method.ParametersData[i];
                    var parameterType = parameterData.ParameterType;
                    if (GMCommandParse.ParseArgument(argument, parameterType, out var value)) //string直接转参数类型
                    {
                        parameters[i] = value;
                        continue;
                    }

                    //转换失败
                    success = false;
                    GameLog.Warn("无法将参数" + argument + "转换为" + parameterType);
                }
                catch (Exception e)
                {
                    success = false;
                    GameLog.Warn("参数获取失败：" + e);
                }
            }

            return success;
        }

        /// <summary>
        /// 完整匹配要执行的指令
        /// </summary>
        private static bool PrepareBeforeExecuteByFull()
        {
            var (commandIndex, commandLastIndex) = GetCommandIndexByFull(CommandArguments[0]);
            if (commandLastIndex < 0)
            {
                return false;
            }

            var parameterCountMismatch = false; //传参的数量是否正确
            while (commandIndex <= commandLastIndex)
            {
                //检查指令是否合法
                if (!MethodDataList[commandIndex].IsValid())
                {
                    // Methods.RemoveAt(commandIndex);
                    commandLastIndex--;
                    continue;
                }

                //检查传参的数量是否正确
                if (MethodDataList[commandIndex].ParametersData.Length == CommandArguments.Count - 1)
                {
                    MatchingMethods.Add(MethodDataList[commandIndex]);
                    commandIndex++; //下一个指令
                    continue;
                }

                parameterCountMismatch = true;
                commandIndex++; //下一个指令
            }

            return parameterCountMismatch;
        }

        /// <summary>
        /// 没有找到commandIndex时，返回查找结束位置的后面一条指令
        /// </summary>
        private static string GetCommandSuggestionWhenNotMatch(int commandIndex, string commandPrefix)
        {
            commandIndex = ~commandIndex; //反推查找结束位置
            if (commandIndex >= MethodDataList.Count) //越界
            {
                return null;
            }

            var methodName = MethodDataList[commandIndex].Method.Name;
            //前缀相同
            return GMCommandUtils.IsPrefix(methodName, commandPrefix) ? methodName : null;
        }

        /// <summary>
        /// 以完整匹配方式，获取匹配的指令的起始和最后的索引
        /// </summary>
        private static (int, int) GetCommandIndexByFull(string commandName)
        {
            var commandIndex = FindCommandIndex(commandName);
            if (commandIndex < 0)
            {
                commandIndex = ~commandIndex;
            }

            var commandLastIndex = commandIndex;
            if (commandIndex >= MethodDataList.Count || GMCommandUtils.IsSame(MethodDataList[commandIndex].Method.Name, commandName) != 0)
            {
                return (commandIndex, -1);
            }

            while (commandIndex > 0 && GMCommandUtils.IsSame(MethodDataList[commandIndex - 1].Method.Name, commandName) == 0)
            {
                commandIndex--;
            }

            while (commandLastIndex < MethodDataList.Count - 1 && GMCommandUtils.IsSame(MethodDataList[commandLastIndex + 1].Method.Name, commandName) == 0)
            {
                commandLastIndex++;
            }

            return (commandIndex, commandLastIndex);
        }

        /// <summary>
        /// 以前缀匹配方式，获取匹配的指令的起始和最后的索引
        /// </summary>
        private static (int, int) GetCommandIndexByPrefix(string commandPrefix)
        {
            var commandIndex = FindCommandIndex(commandPrefix);
            if (commandIndex < 0)
            {
                commandIndex = ~commandIndex;
            }

            var commandLastIndex = commandIndex;
            if (commandIndex >= MethodDataList.Count || !GMCommandUtils.IsPrefix(MethodDataList[commandIndex].Method.Name, commandPrefix))
            {
                return (commandIndex, -1);
            }

            while (commandIndex > 0 && GMCommandUtils.IsPrefix(MethodDataList[commandIndex - 1].Method.Name, commandPrefix))
            {
                commandIndex--;
            }

            while (commandLastIndex < MethodDataList.Count - 1 && GMCommandUtils.IsPrefix(MethodDataList[commandLastIndex + 1].Method.Name, commandPrefix))
            {
                commandLastIndex++;
            }

            return (commandIndex, commandLastIndex);
        }

        /// <summary>
        /// 通过分隔符提取指令名
        /// </summary>
        /// <returns>裁剪好的指令、是否是完整指令、指令的参数数量</returns>
        private static (string, bool, int) GetCommandByDelimiter(string commandOriginal, List<int> caretIndexIncrements, bool commandNameCalculated)
        {
            var commandNameFullyTyped = false; //command是否完全输入
            var numberOfParameters = -1; //参数数量
            string commandResult = null;

            for (var i = 0; i < commandOriginal.Length; i++)
            {
                if (char.IsWhiteSpace(commandOriginal[i]))
                {
                    continue;
                }

                //分隔符
                var startDelimiterIndexOfDelimiterGroup = StartDelimiterIndexOfDelimiterGroup(commandOriginal[i]); //首分隔符在预设分隔符组中的索引
                int endDelimiterIndexOfCommand; //尾分割符在command中的索引
                int commandNameLength;
                if (startDelimiterIndexOfDelimiterGroup >= 0)
                {
                    endDelimiterIndexOfCommand = EndDelimiterIndexOfCommand(commandOriginal, startDelimiterIndexOfDelimiterGroup, i + 1);
                    commandNameFullyTyped = commandOriginal.Length > endDelimiterIndexOfCommand;

                    //提取指令名
                    commandNameLength = endDelimiterIndexOfCommand - i - 1;
                    commandResult = SubstringCommand(commandOriginal, commandNameLength, i + 1);

                    //跳过该指令长度，继续循环
                    i = (endDelimiterIndexOfCommand < commandOriginal.Length - 1 && commandOriginal[endDelimiterIndexOfCommand + 1] == ',')
                        ? endDelimiterIndexOfCommand + 1
                        : endDelimiterIndexOfCommand;
                    caretIndexIncrements.Add(i + 1);
                    numberOfParameters++;
                    continue;
                }

                endDelimiterIndexOfCommand = GMCommandUtils.IndexOfChar(commandOriginal, ' ', i + 1);
                commandNameFullyTyped = commandOriginal.Length > endDelimiterIndexOfCommand;
                commandNameLength = commandOriginal[endDelimiterIndexOfCommand - 1] == ',' ? endDelimiterIndexOfCommand - 1 - i : endDelimiterIndexOfCommand - i;
                commandResult = SubstringCommand(commandOriginal, commandNameLength, i);

                i = endDelimiterIndexOfCommand;
                caretIndexIncrements.Add(i);
                numberOfParameters++;
            }

            return (commandResult, commandNameFullyTyped, numberOfParameters);
        }

        /// <summary>
        /// 裁剪有分隔符的指令，提取指令名
        /// </summary>
        private static string SubstringCommand(string commandOriginal, int commandNameLength, int startIndex)
        {
            if (commandNameLength == 0 || commandOriginal.Length != commandNameLength ||
                GMCommandUtils.StringComparer.IndexOf(commandOriginal, commandOriginal, startIndex, commandNameLength, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace) != startIndex)
            {
                commandOriginal = commandOriginal.Substring(startIndex, commandNameLength);
            }

            return commandOriginal;
        }

        /// <summary>
        /// 查找字符在分隔符组中的索引
        /// </summary>
        private static int StartDelimiterIndexOfDelimiterGroup(char c)
        {
            for (var i = 0; i < InputDelimiters.Length; i++)
            {
                if (c == InputDelimiters[i][0])
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 查找尾分隔符在指令中的索引
        /// </summary>
        private static int EndDelimiterIndexOfCommand(string command, int startDelimiterIndexOfDelimiterGroup, int startDelimiterIndexInCommand)
        {
            var startChar = InputDelimiters[startDelimiterIndexOfDelimiterGroup][0];
            var endChar = InputDelimiters[startDelimiterIndexOfDelimiterGroup][1];
            var depth = 1;
            for (var i = startDelimiterIndexInCommand; i < command.Length; i++)
            {
                var c = command[i];
                if (c == endChar && --depth <= 0)
                {
                    return i;
                }

                if (c == startChar)
                {
                    depth++;
                }
            }

            return command.Length;
        }

        /// <summary>
        /// 匹配指令装填matchingCommands
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="byFull">是否完整匹配</param>
        /// <param name="matchingCommands"></param>
        private static void FindCommands(string commandName, List<GMMethodData> matchingCommands, bool byFull)
        {
            if (byFull)
            {
                FindCommandsByFull(commandName, matchingCommands);
                return;
            }

            FindCommandsByPrefix(commandName, matchingCommands);
        }

        /// <summary>
        /// 完整匹配指令，装填matchingCommands
        /// </summary>
        private static void FindCommandsByFull(string commandName, List<GMMethodData> matchingCommands)
        {
            foreach (var methodInfo in MethodDataList)
            {
                if (methodInfo.IsValid() && GMCommandUtils.FindSubstringIndex(methodInfo.Method.Name, commandName) >= 0)
                {
                    matchingCommands.Add(methodInfo);
                }
            }
        }

        /// <summary>
        /// 前缀匹配指令，装填matchingCommands
        /// </summary>
        private static void FindCommandsByPrefix(string commandName, List<GMMethodData> matchingCommands)
        {
            foreach (var methodInfo in MethodDataList)
            {
                if (methodInfo.IsValid() && GMCommandUtils.IsSame(methodInfo.Method.Name, commandName) == 0)
                {
                    matchingCommands.Add(methodInfo);
                }
            }
        }

        /// <summary>
        /// 拆分指令和传入的参数值，将command裁剪为只有指令设为commandArgumentList[0]，参数值加进commandArgumentList[1~n]，
        /// </summary>
        public static void FetchArgumentsFromCommand(string commandOriginal, List<string> argumentList)
        {
            for (var i = 0; i < commandOriginal.Length; i++)
            {
                if (char.IsWhiteSpace(commandOriginal[i]))
                {
                    continue;
                }

                //检测指令中有没有分隔符
                var startDelimiterIndexOfDelimiterGroup = StartDelimiterIndexOfDelimiterGroup(commandOriginal[i]);
                int endDelimiterIndex;
                if (startDelimiterIndexOfDelimiterGroup >= 0)
                {
                    endDelimiterIndex = EndDelimiterIndexOfCommand(commandOriginal, startDelimiterIndexOfDelimiterGroup, i + 1);
                    argumentList.Add(commandOriginal.Substring(i + 1, endDelimiterIndex - i - 1)); //添加参数值
                    //跳过当前参数名和尾分隔符，继续循环
                    i = (endDelimiterIndex < commandOriginal.Length - 1 && commandOriginal[endDelimiterIndex + 1] == ',') ? endDelimiterIndex + 1 : endDelimiterIndex;
                    continue;
                }

                //没有首分隔符
                endDelimiterIndex = GMCommandUtils.IndexOfChar(commandOriginal, ' ', i + 1);
                argumentList.Add(commandOriginal.Substring(i, commandOriginal[endDelimiterIndex - 1] == ',' ? endDelimiterIndex - 1 - i : endDelimiterIndex - i));
                i = endDelimiterIndex;
            }
        }
    }
}