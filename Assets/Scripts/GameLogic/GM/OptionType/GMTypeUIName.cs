 // ******************************************************************
//@file         GMTypeUIName.cs
//@brief        UI界面名 gm指令参数类型
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 21:39:39
// ******************************************************************

using Yu;

public class GMTypeUIName : GmParameterOptionsBase<string>
{
    protected override object[] GetOptionsSource()
    {
        var sceneData = ConfigManager.Tables.CfgUI.DataList;
        var options = new object[sceneData.Count];
        for (var i = 0; i < sceneData.Count; i++)
        {
            options[i] = sceneData[i].Id;
        }
        
        return options;
    }

    protected override string SetShowValue(string value)
    {
        return ConfigManager.Tables.CfgUI[value].Idx;
    }
}