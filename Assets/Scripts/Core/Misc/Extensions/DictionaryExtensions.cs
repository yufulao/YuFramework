// ******************************************************************
//@file         DictionaryExtensions.cs
//@brief        字典拓展类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.06.08 14:22:03 
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using Yu;

public static class DictionaryExtensions
{
    /// <summary>
    /// 深拷贝
    /// </summary>
    public static Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(this Dictionary<TKey, TValue> source)
    {
        return DeepCopy(source, NullDeepCopyValueFunc);
    }
    
    public static Dictionary<TKey, TValue> DeepCopy<TKey, TValue>(this Dictionary<TKey, TValue> source, Func<TValue, TValue> valueCloneFunc)
    {
        var copy = new Dictionary<TKey, TValue>(source.Count, source.Comparer);
        foreach (var (key, value) in source)
        {
            copy[key] = valueCloneFunc(value);
        }

        return copy;
    }

    /// <summary>
    /// 返回原value
    /// </summary>
    private static TValue NullDeepCopyValueFunc<TValue>(TValue value)
    {
        return value;
    }
}