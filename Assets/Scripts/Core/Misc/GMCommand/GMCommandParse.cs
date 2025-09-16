// ******************************************************************
//@file         GMCommandParse.cs
//@brief        GM指令的参数类型转换
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:42:33
// ******************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//转化类型部分
namespace Yu
{
    public static class GMCommandParse
    {
        public delegate bool ParseFunction(string input, out object output);

        public static readonly Dictionary<Type, ParseFunction> ParseFunctions = new()
        {
            { typeof(string), ParseString },
            { typeof(bool), ParseBool },
            { typeof(int), ParseInt },
            { typeof(uint), ParseUInt },
            { typeof(long), ParseLong },
            { typeof(ulong), ParseULong },
            { typeof(byte), ParseByte },
            { typeof(sbyte), ParseSByte },
            { typeof(short), ParseShort },
            { typeof(ushort), ParseUShort },
            { typeof(char), ParseChar },
            { typeof(float), ParseFloat },
            { typeof(double), ParseDouble },
            { typeof(decimal), ParseDecimal },
            { typeof(Vector2), ParseVector2 },
            { typeof(Vector3), ParseVector3 },
            { typeof(Vector4), ParseVector4 },
            { typeof(Quaternion), ParseQuaternion },
            { typeof(Color), ParseColor },
            { typeof(Color32), ParseColor32 },
            { typeof(Rect), ParseRect },
            { typeof(RectOffset), ParseRectOffset },
            { typeof(Bounds), ParseBounds },
            { typeof(GameObject), ParseGameObject },
            { typeof(Vector2Int), ParseVector2Int },
            { typeof(Vector3Int), ParseVector3Int },
            { typeof(RectInt), ParseRectInt },
            { typeof(BoundsInt), ParseBoundsInt },
        };


        /// <summary>
        /// string转参数类型
        /// </summary>
        public static bool ParseArgument(string argument, Type parseType, out object argumentFixed)
        {
            // GameLog.Info(argument+"  "+ parseType);
            if (ParseFunctions.TryGetValue(parseType, out var parseFunction)) //自定义可转类型
            {
                return parseFunction(argument, out argumentFixed);
            }

            if (typeof(IGmParameterOptions).IsAssignableFrom(parseType)) //代理参数类型
            {
                return ParseArgumentProxy(argument, parseType, out argumentFixed);
            }

            if (parseType.IsEnum) //枚举类型
            {
                return ParseEnum(argument, parseType, out argumentFixed);
            }

            if (IsSupportedArrayType(parseType)) //数组类型
            {
                return ParseArray(argument, parseType, out argumentFixed);
            }

            argumentFixed = null;
            return false;
        }

        /// <summary>
        /// 检查数组的类型是否支持
        /// </summary>
        public static bool IsSupportedArrayType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.IsArray)
            {
                return IsSupportedArrayTypeOfArray(type); //具体类型数组
            }

            return type.IsGenericType && IsSupportedArrayTypeOfGenericType(type); //泛型类型数组
        }

        /// <summary>
        /// string实参转为代理参数对象
        /// </summary>
        private static bool ParseArgumentProxy(string argument, Type parameterTypeProxy, out object argumentFixed)
        {
            var parameterTypeBase = parameterTypeProxy.BaseType;
            if (parameterTypeBase == null)
            {
                argumentFixed = null;
                return false;
            }

            //获取parameterType基类GmParameterOptionsBase<T>的泛型类型
            var genericArguments = parameterTypeBase.GetGenericArguments();
            // GameLog.Info(parameterType);
            // GameLog.Info(parameterTypeBase);
            // GameLog.Info(parameterTypeBase.IsGenericType);
            // GameLog.Info(parameterTypeBase.GetGenericTypeDefinition() == typeof(GmParameterOptionsBase<>));
            // GameLog.Info(Utils.ToStringArray(genericArguments));
            if (genericArguments.Length <= 0)
            {
                argumentFixed = null;
                return false;
            }

            var realType = genericArguments[0]; //第一个泛型类型
            //根据泛型模板生成具体类GmParameterOptionsBase<parameterType>
            // var genericType = typeof(GmParameterOptionsBase<>).MakeGenericType(realType);
            if (!ParseArgument(argument, realType, out argumentFixed)) //多层代理参数也适用
            {
                GameLog.Warn($"无法将str:{argument}，转换为代理参数:{parameterTypeProxy}");
                return true;
            }

            var parameter = Activator.CreateInstance(parameterTypeProxy); //创建代理对象
            var valueField = parameterTypeProxy.GetField("Value"); //类名获取字段Value
            valueField.SetValue(parameter, argumentFixed); //设置Value值
            argumentFixed = parameter;
            return true;
        }

        /// <summary>
        /// 具体类型数组转换
        /// </summary>
        private static bool IsSupportedArrayTypeOfArray(Type type)
        {
            if (type.GetArrayRank() != 1)
            {
                return false;
            }

            type = type.GetElementType();

            if (type == null)
            {
                return false;
            }

            return ParseFunctions.ContainsKey(type) || typeof(Component).IsAssignableFrom(type) || type.IsEnum;
        }

        /// <summary>
        /// 泛型类型数组转换
        /// </summary>
        private static bool IsSupportedArrayTypeOfGenericType(Type type)
        {
            if (type.GetGenericTypeDefinition() != typeof(List<>))
            {
                return false;
            }

            var genericArgument = type.GetGenericArguments();
            if (genericArgument.Length <= 0)
            {
                return false;
            }

            type = genericArgument[0];
            return ParseFunctions.ContainsKey(type) || typeof(Component).IsAssignableFrom(type) || type.IsEnum;
        }

        private static bool ParseString(string input, out object output)
        {
            output = input;
            return true;
        }

        private static bool ParseBool(string input, out object output)
        {
            if (input == "1" || input.ToLowerInvariant() == "true")
            {
                output = true;
                return true;
            }

            if (input == "0" || input.ToLowerInvariant() == "false")
            {
                output = false;
                return true;
            }

            output = false;
            return false;
        }

        private static bool ParseInt(string input, out object output)
        {
            var result = int.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseUInt(string input, out object output)
        {
            var result = uint.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseLong(string input, out object output)
        {
            var result = long.TryParse(!input.EndsWith("L", StringComparison.OrdinalIgnoreCase) ? input : input.Substring(0, input.Length - 1), out var value);
            output = value;
            return result;
        }

        private static bool ParseULong(string input, out object output)
        {
            var result = ulong.TryParse(!input.EndsWith("L", StringComparison.OrdinalIgnoreCase) ? input : input.Substring(0, input.Length - 1), out var value);
            output = value;
            return result;
        }

        private static bool ParseByte(string input, out object output)
        {
            var result = byte.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseSByte(string input, out object output)
        {
            var result = sbyte.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseShort(string input, out object output)
        {
            var result = short.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseUShort(string input, out object output)
        {
            var result = ushort.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseChar(string input, out object output)
        {
            var result = char.TryParse(input, out var value);
            output = value;
            return result;
        }

        private static bool ParseFloat(string input, out object output)
        {
            var result = float.TryParse(!input.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? input : input.Substring(0, input.Length - 1), out var value);
            output = value;
            return result;
        }

        private static bool ParseDouble(string input, out object output)
        {
            var result = double.TryParse(!input.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? input : input.Substring(0, input.Length - 1), out var value);
            output = value;
            return result;
        }

        private static bool ParseDecimal(string input, out object output)
        {
            var result = decimal.TryParse(!input.EndsWith("f", StringComparison.OrdinalIgnoreCase) ? input : input.Substring(0, input.Length - 1), out var value);
            output = value;
            return result;
        }

        private static bool ParseVector2(string input, out object output)
        {
            return ParseVector(input, typeof(Vector2), out output);
        }

        private static bool ParseVector3(string input, out object output)
        {
            return ParseVector(input, typeof(Vector3), out output);
        }

        private static bool ParseVector4(string input, out object output)
        {
            return ParseVector(input, typeof(Vector4), out output);
        }

        private static bool ParseQuaternion(string input, out object output)
        {
            return ParseVector(input, typeof(Quaternion), out output);
        }

        private static bool ParseColor(string input, out object output)
        {
            return ParseVector(input, typeof(Color), out output);
        }

        private static bool ParseColor32(string input, out object output)
        {
            return ParseVector(input, typeof(Color32), out output);
        }

        private static bool ParseRect(string input, out object output)
        {
            return ParseVector(input, typeof(Rect), out output);
        }

        private static bool ParseRectOffset(string input, out object output)
        {
            return ParseVector(input, typeof(RectOffset), out output);
        }

        private static bool ParseBounds(string input, out object output)
        {
            return ParseVector(input, typeof(Bounds), out output);
        }

        private static bool ParseVector2Int(string input, out object output)
        {
            return ParseVector(input, typeof(Vector2Int), out output);
        }

        private static bool ParseVector3Int(string input, out object output)
        {
            return ParseVector(input, typeof(Vector3Int), out output);
        }

        private static bool ParseRectInt(string input, out object output)
        {
            return ParseVector(input, typeof(RectInt), out output);
        }

        private static bool ParseBoundsInt(string input, out object output)
        {
            return ParseVector(input, typeof(BoundsInt), out output);
        }

        private static bool ParseGameObject(string input, out object output)
        {
            output = input == "null" ? null : GameObject.Find(input);
            return true;
        }

        private static bool ParseComponent(string input, Type componentType, out object output)
        {
            var gameObject = input == "null" ? null : GameObject.Find(input);
            output = gameObject ? gameObject.GetComponent(componentType) : null;
            return true;
        }

        /// <summary>
        /// 转枚举使用的操作
        /// </summary>
        private enum ParseEnumOperation
        {
            None,
            Or,
            And
        }

        /// <summary>
        /// 转枚举
        /// </summary>
        private static bool ParseEnum(string input, Type enumType, out object output)
        {
            var outputInt = 0;
            var operation = ParseEnumOperation.None;
            for (var i = 0; i < input.Length; i++)
            {
                var orIndex = input.IndexOf('|', i);
                var andIndex = input.IndexOf('&', i);
                var enumStr = orIndex < 0
                    ? input.Substring(i, (andIndex < 0 ? input.Length : andIndex) - i).Trim()
                    : input.Substring(i, (andIndex < 0 ? orIndex : Mathf.Min(andIndex, orIndex)) - i).Trim();

                if (!int.TryParse(enumStr, out var value))
                {
                    try
                    {
                        // Case-insensitive enum parsing
                        value = Convert.ToInt32(Enum.Parse(enumType, enumStr, true));
                    }
                    catch
                    {
                        output = null;
                        return false;
                    }
                }

                outputInt = GetEnumOutput(outputInt, value, operation);
                var (newI, newOperation) = GetForIndexAndOperation(i, operation, orIndex, andIndex, input);
                i = newI;
                operation = newOperation;
            }

            output = Enum.ToObject(enumType, outputInt);
            return true;
        }

        /// <summary>
        /// 转枚举时获取输出int
        /// </summary>
        private static int GetEnumOutput(int outputInt, int value, ParseEnumOperation operation)
        {
            switch (operation)
            {
                case ParseEnumOperation.None:
                    outputInt = value;
                    break;
                case ParseEnumOperation.Or:
                    outputInt |= value;
                    break;
                case ParseEnumOperation.And:
                    outputInt &= value;
                    break;
            }

            return outputInt;
        }

        /// <summary>
        /// 转枚举时获取循环i和operation
        /// </summary>
        private static (int, ParseEnumOperation) GetForIndexAndOperation(int i, ParseEnumOperation operation, int orIndex, int andIndex, string input)
        {
            if (orIndex >= 0 && (andIndex < 0 || orIndex < andIndex))
            {
                operation = ParseEnumOperation.Or;
                i = orIndex;
            }

            if (andIndex >= 0 && (orIndex < 0 || andIndex < orIndex))
            {
                operation = ParseEnumOperation.And;
                i = andIndex;
            }

            if (orIndex < 0 && andIndex < 0)
            {
                i = input.Length;
            }

            return (i, operation);
        }

        /// <summary>
        /// 转数组
        /// </summary>
        private static bool ParseArray(string input, Type arrayType, out object output)
        {
            var valuesToParse = new List<string>(2);
            //以','来裁切input
            GMCommand.FetchArgumentsFromCommand(input, valuesToParse);
            var result = (IList)Activator.CreateInstance(arrayType, valuesToParse.Count);
            output = result;

            Type elementType;
            if (arrayType.IsArray)
            {
                elementType = arrayType.GetElementType();
                for (var i = 0; i < valuesToParse.Count; i++)
                {
                    if (!ParseArgument(valuesToParse[i], elementType, out var obj))
                    {
                        return false;
                    }

                    result[i] = obj;
                }

                return true;
            }

            elementType = arrayType.GetGenericArguments()[0];
            foreach (var str in valuesToParse)
            {
                if (!ParseArgument(str, elementType, out var obj))
                {
                    return false;
                }

                result.Add(obj);
            }

            return true;
        }

        /// <summary>
        /// 转向量
        /// </summary>
        private static bool ParseVector(string input, Type vectorType, out object output)
        {
            var (processSuccess, tokenValues) = PrepareTokenValuesForParseVector(input, vectorType, out output);
            if (!processSuccess)
            {
                return false;
            }

            if (vectorType == typeof(Vector2))
            {
                output = ParseVectorOfVector2(tokenValues);
                return true;
            }

            if (vectorType == typeof(Vector3))
            {
                output = ParseVectorOfVector3(tokenValues);
                return true;
            }

            if (vectorType == typeof(Vector4))
            {
                output = ParseVectorOfVector4(tokenValues);
                return true;
            }

            if (vectorType == typeof(Quaternion))
            {
                output = ParseVectorOfQuaternion(tokenValues);
                return true;
            }

            if (vectorType == typeof(Color))
            {
                output = ParseVectorOfColor(tokenValues);
                return true;
            }

            if (vectorType == typeof(Color32))
            {
                output = ParseVectorOfColor32(tokenValues);
                return true;
            }

            if (vectorType == typeof(Rect))
            {
                output = ParseVectorOfRect(tokenValues);
                return true;
            }

            if (vectorType == typeof(RectOffset))
            {
                output = ParseVectorOfRectOffset(tokenValues);
                return true;
            }

            if (vectorType == typeof(Bounds))
            {
                output = ParseVectorOfBounds(tokenValues);
                return true;
            }

            if (vectorType == typeof(Vector3Int))
            {
                output = ParseVectorOfVector3Int(tokenValues);
                return true;
            }

            if (vectorType == typeof(Vector2Int))
            {
                output = ParseVectorOfVector2Int(tokenValues);
                return true;
            }

            if (vectorType == typeof(RectInt))
            {
                output = ParseVectorOfRectInt(tokenValues);
                return true;
            }

            if (vectorType == typeof(BoundsInt))
            {
                output = ParseVectorOfBoundsInt(tokenValues);
                return true;
            }

            output = null;
            return false;
        }

        /// <summary>
        /// 转换向量前，对input处理，生成float[] tokenValues
        /// </summary>
        private static (bool, float[]) PrepareTokenValuesForParseVector(string input, Type vectorType, out object output)
        {
            output = null;
            var tokens = new List<string>(input.Replace(',', ' ').Trim().Split(' '));
            for (var i = tokens.Count - 1; i >= 0; i--)
            {
                tokens[i] = tokens[i].Trim();
                if (tokens[i].Length == 0)
                {
                    tokens.RemoveAt(i);
                }
            }

            var tokenValues = new float[tokens.Count];
            for (var i = 0; i < tokens.Count; i++)
            {
                if (!ParseFloat(tokens[i], out var value))
                {
                    if (vectorType == typeof(Vector3))
                    {
                        output = Vector3.zero;
                        return (false, tokenValues);
                    }

                    if (vectorType == typeof(Vector2))
                    {
                        output = Vector2.zero;
                        return (false, tokenValues);
                    }

                    output = Vector4.zero;
                    return (false, tokenValues);
                }

                tokenValues[i] = (float)value;
            }

            return (true, tokenValues);
        }

        private static object ParseVectorOfVector2(IReadOnlyList<float> tokenValues)
        {
            var result = Vector2.zero;

            for (var i = 0; i < tokenValues.Count && i < 2; i++)
            {
                result[i] = tokenValues[i];
            }

            return result;
        }

        private static object ParseVectorOfVector3(IReadOnlyList<float> tokenValues)
        {
            var result = Vector3.zero;

            for (var i = 0; i < tokenValues.Count && i < 3; i++)
            {
                result[i] = tokenValues[i];
            }

            return result;
        }

        private static object ParseVectorOfVector4(IReadOnlyList<float> tokenValues)
        {
            var result = Vector4.zero;

            for (var i = 0; i < tokenValues.Count && i < 4; i++)
            {
                result[i] = tokenValues[i];
            }

            return result;
        }

        private static object ParseVectorOfQuaternion(IReadOnlyList<float> tokenValues)
        {
            var result = Quaternion.identity;

            for (var i = 0; i < tokenValues.Count && i < 4; i++)
            {
                result[i] = tokenValues[i];
            }

            return result;
        }

        private static object ParseVectorOfColor(IReadOnlyList<float> tokenValues)
        {
            var result = Color.black;

            for (var i = 0; i < tokenValues.Count && i < 4; i++)
            {
                result[i] = tokenValues[i];
            }

            return result;
        }

        private static object ParseVectorOfColor32(IReadOnlyList<float> tokenValues)
        {
            var result = new Color32(0, 0, 0, 255);

            if (tokenValues.Count > 0)
            {
                result.r = (byte)Mathf.RoundToInt(tokenValues[0]);
            }

            if (tokenValues.Count > 1)
            {
                result.g = (byte)Mathf.RoundToInt(tokenValues[1]);
            }

            if (tokenValues.Count > 2)
            {
                result.b = (byte)Mathf.RoundToInt(tokenValues[2]);
            }

            if (tokenValues.Count > 3)
            {
                result.a = (byte)Mathf.RoundToInt(tokenValues[3]);
            }

            return result;
        }

        private static object ParseVectorOfRect(IReadOnlyList<float> tokenValues)
        {
            var result = Rect.zero;

            if (tokenValues.Count > 0)
            {
                result.x = tokenValues[0];
            }

            if (tokenValues.Count > 1)
            {
                result.y = tokenValues[1];
            }

            if (tokenValues.Count > 2)
            {
                result.width = tokenValues[2];
            }

            if (tokenValues.Count > 3)
            {
                result.height = tokenValues[3];
            }

            return result;
        }

        private static object ParseVectorOfRectOffset(IReadOnlyList<float> tokenValues)
        {
            var result = new RectOffset();

            if (tokenValues.Count > 0)
            {
                result.left = Mathf.RoundToInt(tokenValues[0]);
            }

            if (tokenValues.Count > 1)
            {
                result.right = Mathf.RoundToInt(tokenValues[1]);
            }

            if (tokenValues.Count > 2)
            {
                result.top = Mathf.RoundToInt(tokenValues[2]);
            }

            if (tokenValues.Count > 3)
            {
                result.bottom = Mathf.RoundToInt(tokenValues[3]);
            }

            return result;
        }

        private static Bounds ParseVectorOfBounds(IReadOnlyList<float> tokenValues)
        {
            var center = Vector3.zero;
            for (var i = 0; i < tokenValues.Count && i < 3; i++)
            {
                center[i] = tokenValues[i];
            }

            var size = Vector3.zero;
            for (var i = 3; i < tokenValues.Count && i < 6; i++)
            {
                size[i - 3] = tokenValues[i];
            }

            return new Bounds(center, size);
        }

        private static object ParseVectorOfVector3Int(IReadOnlyList<float> tokenValues)
        {
            var result = Vector3Int.zero;

            for (var i = 0; i < tokenValues.Count && i < 3; i++)
            {
                result[i] = Mathf.RoundToInt(tokenValues[i]);
            }

            return result;
        }

        private static object ParseVectorOfVector2Int(IReadOnlyList<float> tokenValues)
        {
            var result = Vector2Int.zero;

            for (var i = 0; i < tokenValues.Count && i < 2; i++)
            {
                result[i] = Mathf.RoundToInt(tokenValues[i]);
            }

            return result;
        }

        private static object ParseVectorOfRectInt(IReadOnlyList<float> tokenValues)
        {
            var result = new RectInt();

            if (tokenValues.Count > 0)
            {
                result.x = Mathf.RoundToInt(tokenValues[0]);
            }

            if (tokenValues.Count > 1)
            {
                result.y = Mathf.RoundToInt(tokenValues[1]);
            }

            if (tokenValues.Count > 2)
            {
                result.width = Mathf.RoundToInt(tokenValues[2]);
            }

            if (tokenValues.Count > 3)
            {
                result.height = Mathf.RoundToInt(tokenValues[3]);
            }

            return result;
        }

        private static object ParseVectorOfBoundsInt(IReadOnlyList<float> tokenValues)
        {
            var center = Vector3Int.zero;
            for (var i = 0; i < tokenValues.Count && i < 3; i++)
            {
                center[i] = Mathf.RoundToInt(tokenValues[i]);
            }

            var size = Vector3Int.zero;
            for (var i = 3; i < tokenValues.Count && i < 6; i++)
            {
                size[i - 3] = Mathf.RoundToInt(tokenValues[i]);
            }

            return new BoundsInt(center, size);
        }
    }
}