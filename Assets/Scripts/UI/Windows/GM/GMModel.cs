// ******************************************************************
//@file         GMModel.cs
//@brief        GM指令界面model
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.27 15:12:47
// ******************************************************************

using System;
using System.Collections.Generic;
using Yu;

public class GMModel
{
    public List<GMMethodData> CurrentCommandList = new();
    public HashSet<string> CommandCatalogSet { get; private set; }

    private List<GMMethodData> _cacheUIShowGmList = new();
    // public readonly List<string> CommandTypeNameList = new(); // GM指令类型名列表
    // public List<GMMethodData> DefaultCommandsList = new(); // 默认GM指令列表

    // private readonly Dictionary<Type, List<GMMethodData>> _commandDict = new(); // GM指令类型字典
    // private readonly List<Type> _commandTypeList = new(); // GM指令类型列表
    // private Type _currentCommandType = default; // 当前GM指令类型
    private bool _hadInit = false;

    /// <summary>
    /// 尝试初始化
    /// </summary>
    public bool TryInit()
    {
        if (_hadInit)
        {
            return true;
        }

        if (!GMCommand.HadInit)
        {
            return false;
        }

        // _sorter = new GMSorter();
        // InitCommandTypeList();
        // InitCommandTypeNameList();
        // InitCommandDict();
        // _currentCommandType = _commandTypeList[0];
        // DefaultCommandsList = _commandDict[_commandTypeList[0]];
        InitCommandList();

        _hadInit = true;
        return true;
    }

    /// <summary>
    /// 打开界面时
    /// </summary>
    public void OnOpen()
    {
    }

    /// <summary>
    /// 通过指令分类过滤
    /// </summary>
    public void FilterByCatalog(string catalog)
    {
        GMCommand.Filter.FilterByCatalog(_cacheUIShowGmList, ref CurrentCommandList, catalog);
    }

    /// <summary>
    /// 初始化指令列表
    /// </summary>
    private void InitCommandList()
    {
        GMCommand.Filter.FilterByIsShow(GMCommand.MethodDataList, ref _cacheUIShowGmList, true);
        CommandCatalogSet = new HashSet<string>();
        foreach (var methodData in _cacheUIShowGmList)
        {
            CommandCatalogSet.Add(methodData.Catalog);
        }
    }

    // public List<GMMethodData> GetCurrentCommandList()
    // {
    //     return _commandDict[_currentCommandType];
    // }
    //
    // public List<GMMethodData> GetCommandsByType(Type commandType)
    // {
    //     return _commandDict[commandType];
    // }
    //
    // public Type GetTypeByTypeName(string typeName)
    // {
    //     var index = CommandTypeNameList.IndexOf(typeName);
    //     var currentCommandType = _commandTypeList[index];
    //     _currentCommandType = currentCommandType;
    //     return currentCommandType;
    // }
    //
    // /// <summary>
    // /// 初始化指令名列表
    // /// </summary>
    // private void InitCommandTypeNameList()
    // {
    //     // Todo 排序
    //     // _sorter.SortByID(_commandTypeList);
    //     
    //     foreach (var type in _commandTypeList)
    //     {
    //         MemberInfo mi = type;
    //         GMTypeAttribute customAttribute = (GMTypeAttribute)mi.GetCustomAttributes(typeof(GMTypeAttribute), true).First(); // 提前取得第一个自定义属性，再进行类型转化，优化程序
    //         String GMTypeName = customAttribute.GMTypeName;
    //         
    //         CommandTypeNameList.Add(GMTypeName);
    //         // GameLog.Info($"key:{type}; value:{GMTypeName}");
    //     }
    //     
    //     
    // }
    //
    // /// <summary>
    // /// 初始化指令类型列表
    // /// </summary>
    // private void InitCommandTypeList()
    // {
    //     foreach (var methodData in GMCommand.MethodDataList)
    //     {
    //         if (!methodData.UIShow)
    //         {
    //             continue;
    //         }
    //         
    //         if (!_commandTypeList.Contains(methodData.CommandType))
    //         {
    //             _commandTypeList.Add(methodData.CommandType);
    //         }
    //     }
    // }
    //
    // /// <summary>
    // /// 初始化指令字典
    // /// </summary>
    // private void InitCommandDict()
    // {
    //     foreach (var type in _commandTypeList)
    //     {
    //         var result = FilterByType(type);
    //         _commandDict.Add(type, result);
    //         
    //         var valueStr = "";
    //         foreach (var gmMethodData in result)
    //         {
    //             valueStr += gmMethodData.CommandDesc;
    //             valueStr += ",";
    //         }
    //         // GameLog.Info($"key:{type}; value:{valueStr}");
    //     }
    // }
}