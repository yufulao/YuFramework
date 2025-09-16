// ******************************************************************
//@file         GMParameterData.cs
//@brief        gm指令的参数的数据结构
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 14:33:47
// ******************************************************************

using System;
using System.Reflection;

namespace Yu
{
    public class GMParameterData
    {
        public readonly ParameterInfo ParameterInfo;
        public readonly string ParameterDesc; //参数名描述
        public readonly IGmParameterOptions[] ParameterOptionArray; //参数可选项
        public Type ParameterType => ParameterInfo.ParameterType;
        public string ParameterName => ParameterInfo.Name;

        public GMParameterData(ParameterInfo parameterInfo, string parameterDesc)
        {
            ParameterInfo = parameterInfo;
            ParameterDesc = parameterDesc;
            ParameterOptionArray = GetParameterOptionArray();
        }

        /// <summary>
        /// 获取代理参数的可选项
        /// </summary>
        private IGmParameterOptions[] GetParameterOptionArray()
        {
            //检查是否代理参数
            if (!typeof(IGmParameterOptions).IsAssignableFrom(ParameterType))
            {
                return null;
            }

            var optionsProvider = Activator.CreateInstance(ParameterType) as IGmParameterOptions;
            return optionsProvider?.GetParameterOptionArray();
        }

        public override string ToString()
        {
            return $"type:{ParameterType}, desc:{ParameterDesc}, options:\n{Utils.ToStringCollection(ParameterOptionArray)}";
        }
    }
}