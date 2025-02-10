// ******************************************************************
//@file         GMMethodUIAttribute.cs
//@brief        标记作用：需要显示在GM面板上的gm指令
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 16:38:50
// ******************************************************************
using System;

namespace Yu
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class GMMethodUIAttribute : Attribute
    {
    }
}