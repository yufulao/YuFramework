// ******************************************************************
//@file         GMTypeLanguage.cs
//@brief        语言种类 gm指令参数类型
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 20:06:56
// ******************************************************************

using System;
using Yu;

public class GMTypeLanguage : GmParameterOptionsBase<DefLanguage>
{
    protected override object[] GetOptionsSource()
    {
        var languageArray = (DefLanguage[])Enum.GetValues(typeof(DefLanguage));
        var options = new object[languageArray.Length];
        for (var i = 0; i < languageArray.Length; i++)
        {
            options[i] = languageArray[i];
        }

        return options;
    }

    protected override string SetShowValue(DefLanguage value)
    {
        return value.ToString();
    }
}