// ******************************************************************
//@file         GMTypeSceneName.cs
//@brief        场景名 gm指令参数类型
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 15:26:28
// ******************************************************************

using Yu;

public class GMTypeSceneName : GmParameterOptionsBase<string>
{
    protected override object[] GetOptionsSource()
    {
        var sceneData = ConfigManager.Tables.CfgScene.DataList;
        var options = new object[sceneData.Count];
        for (var i = 0; i < sceneData.Count; i++)
        {
            options[i] = sceneData[i].Id;
        }
        
        return options;
    }

    protected override string SetShowValue(string value)
    {
        return ConfigManager.Tables.CfgScene[value].Idx;
    }
}