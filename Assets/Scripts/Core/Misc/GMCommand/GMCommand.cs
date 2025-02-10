// ******************************************************************
//@file         GMCommand.cs
//@brief        GM核心
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:39:16
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace Yu
{
    public partial class GMCommand
    {
        public static bool HadInit { get; private set; }
        public static List<GMMethodData> MethodDataList { get; } = new List<GMMethodData>(); //所有的GMMethodData


        /// <summary>
        /// 初始化，真并行
        /// </summary>
        public static async Task OnInit()
        {
            var step = 0;
            var tasks = new List<Task>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                tasks.Add(Task.Run(() =>
                {
                    step++;
                    try
                    {
                        foreach (var type in assembly.GetExportedTypes())
                        {
                            step++;
                            foreach (var method in type.GetMethods())
                            {
                                step++;
                                foreach (var attribute in method.GetCustomAttributes(typeof(GMMethodAttribute), false))
                                {
                                    step++;
                                    if (attribute is GMMethodAttribute consoleMethod)
                                    {
                                        AddCommand(method, null, consoleMethod.CommandDesc, consoleMethod.ParametersDesc);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Debug.LogError($"assembly:{assembly.GetName().Name} 查找GM指令时有误:\n{e}");
                    }
                    // Debug.Log(assembly.GetName().Name);
                }));
            }

            // Debug.Log(step);
            await Task.WhenAll(tasks);
            // foreach (var methodData in MethodDataList)
            // {
            //     Debug.Log(methodData.ToString());
            // }
            HadInit = true;
        }

        /// <summary>
        /// 执行指令
        /// </summary>
        public static void ExecuteCommand(GMMethodData methodData, object[] parameters)
        {
            var result = methodData.Method.Invoke(methodData.Instance, parameters);
            LogExecuteReturn(methodData, result);
        }

        /// <summary>
        /// 输出指令执行结果
        /// </summary>
        private static void LogExecuteReturn(GMMethodData methodData, object result)
        {
            if (methodData.Method.ReturnType == typeof(void)) //无返回值
            {
                return;
            }

            if (result == null || result.Equals(null))
            {
                Debug.Log("return: null");
                return;
            }

            Debug.Log("return: " + result);
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        private static void AddCommand(MethodInfo method, object instance, string commandDesc, string[] parametersDesc)
        {
            if (string.IsNullOrEmpty(commandDesc))
            {
                Debug.LogError("指令为空");
                return;
            }

            commandDesc = commandDesc.Trim();
            var parameters = method.GetParameters(); //指令的所有参数项
            var parametersType = new Type[parameters.Length]; //对应参数项的参数类型
            if (!CheckAndSetParamTypeValid(parameters, parametersType))
            {
                return;
            }

            var (commandIndex, commandLastIndex) = GetCommandIndexByFull(method.Name.Trim());
            commandIndex = GetCommandIndex(commandIndex, commandLastIndex, parametersType);
            //var parameterDesc = GetParameterDesc(commandName, parameterNames, parameters, parameterTypes);
            if (parametersType.Length != parametersDesc.Length)
            {
                Debug.LogError($"gm指令->{method.Name}，参数描述的个数有误");
                return;
            }

            var parametersData = CreateParametersData(parameters, parametersDesc);
            var methodData = new GMMethodData(method, instance, commandDesc, parametersData);
            MethodDataList.Insert(commandIndex, methodData);
        }

        /// <summary>
        /// 对指令排序，先比较指令名，再比较参数个数，返回指令指定index
        /// </summary>
        private static int GetCommandIndex(int commandIndex, int commandLastIndex, Type[] parameterTypes)
        {
            var commandFirstIndex = commandIndex;
            for (var i = commandFirstIndex; i < commandLastIndex + 1; i++)
            {
                var method = MethodDataList[i];
                var parameterCountDiff = method.ParametersData.Length - parameterTypes.Length;
                if (parameterCountDiff > 0)
                {
                    continue;
                }

                commandIndex = i + 1;
                if (parameterCountDiff != 0)
                {
                    continue;
                }

                var j = 0;
                var parameterData = method.ParametersData[j];
                while (j < parameterTypes.Length && parameterTypes[j] == parameterData.ParameterType)
                {
                    j++;
                }

                if (j < parameterTypes.Length)
                {
                    continue;
                }

                commandIndex = i;
                commandLastIndex--;
                MethodDataList.RemoveAt(i--);
            }

            return commandIndex;
        }

        /// <summary>
        /// 封装gm指令的parameterData
        /// </summary>
        /// <returns></returns>
        private static GMParameterData[] CreateParametersData(ParameterInfo[] parametersInfo, string[] parametersDesc)
        {
            var parametersData = new GMParameterData[parametersInfo.Length];
            for (var i = 0; i < parametersInfo.Length; i++)
            {
                parametersData[i] = new GMParameterData(parametersInfo[i], parametersDesc[i]);
            }

            return parametersData;
        }

        /// <summary>
        /// 检测参数类型是否支持
        /// </summary>
        /// <returns></returns>
        private static bool CheckAndSetParamTypeValid(IReadOnlyList<ParameterInfo> parameters, IList<Type> parameterTypes)
        {
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameterType = parameters[i].ParameterType;
                if (parameterType.IsByRef)
                {
                    Debug.LogError("指令不支持ref和out");
                    return false;
                }

                if (GMCommandParse.ParseFunctions.ContainsKey(parameterType) //能否string转
                    || typeof(IGmParameterOptions).IsAssignableFrom(parameterType) //是否代理参数
                    || parameterType.IsEnum //枚举类型，枚举类型过多，ParseFunctions不做单独处理
                    || GMCommandParse.IsSupportedArrayType(parameterType))
                {
                    parameterTypes[i] = parameterType;
                    continue;
                }

                Debug.LogError(string.Concat("不支持参数", parameters[i].Name, "的类型：", parameterType));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 查找指令名，负值取反即插入位置
        /// </summary>
        private static int FindCommandIndex(string commandName)
        {
            return GMCommandUtils.FindIndex(MethodDataList, commandName, methodData => methodData.Method.Name);
        }
    }
}