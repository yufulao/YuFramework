// ******************************************************************
//@file         GMMethodAttribute.cs
//@brief        GM指令函数的Attribute
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:37:35
// ******************************************************************

using System;

namespace Yu
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class GMMethodAttribute : Attribute
    {
        public string CommandDesc { get; } //指令名称
        public string[] ParametersDesc { get; } //参数名称列表

        
        /// <param name="commandDesc">指令名描述</param>
        /// <param name="parametersDesc">参数名描述Array</param>
        public GMMethodAttribute(string commandDesc, params string[] parametersDesc)
        {
            CommandDesc = commandDesc;
            ParametersDesc = parametersDesc;
        }
    }
}