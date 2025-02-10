// ******************************************************************
//@file         GMMethodData.cs
//@brief        GM指令函数的数据
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:38:54
// ******************************************************************

using System.Reflection;

namespace Yu
{
    public class GMMethodData
    {
        public readonly MethodInfo Method;
        public readonly object Instance;
        public readonly string CommandDesc; //指令名描述
        public readonly GMParameterData[] ParametersData; //参数数据
        public readonly bool UIShow; //是否显示在gm面板

        public GMMethodData(MethodInfo method, object instance, string commandDesc, GMParameterData[] parametersData)
        {
            Method = method;
            Instance = instance;
            CommandDesc = commandDesc;
            ParametersData = parametersData;
            UIShow = method.GetCustomAttribute<GMMethodUIAttribute>() != null;
        }

        public bool IsValid()
        {
            //如果不是静态方法且实例为null则不合法
            return Method.IsStatic || (Instance != null && !Instance.Equals(null));
        }

        public override string ToString()
        {
            return $"name:{Method.Name}, desc:{CommandDesc}, parameters:{Utils.ToStringCollection(ParametersData)}";
        }
    }
}