// ******************************************************************
//@file         UtilsForToString.cs
//@brief        通用类型转string
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 01:34:18
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yu
{
    public static partial class Utils
    {
        /// <summary>
        /// object转string
        /// </summary>
        public static string ToStrObject(object o)
        {
            if (o == null)
            {
                return "null";
            }

            var sb = new StringBuilder();
            foreach (var p in o.GetType().GetFields())
            {
                sb.Append($"{p.Name} = {p.GetValue(o)},");
            }

            foreach (var p in o.GetType().GetProperties())
            {
                sb.Append($"{p.Name} = {p.GetValue(o)},");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 数组转string
        /// </summary>
        public static string ToStringArray<T>(T[] arr, Func<T, string> itemFunc = null)
        {
            if (arr == null)
            {
                return "null";
            }
            
            if (itemFunc != null)
            {
                return "[ " + string.Join(", ", arr.Select(itemFunc)) + " ]";
            }

            return "[ " + string.Join(", ", arr) + " ]";
        }

        /// <summary>
        /// 列表转string
        /// </summary>
        public static string ToStringCollection<T>(IEnumerable<T> arr, Func<T, string> itemFunc = null)
        {
            if (arr == null)
            {
                return "null";
            }
            
            if (itemFunc != null)
            {
                return "[ " + string.Join(", ", arr.Select(itemFunc)) + " ]";
            }

            return "[ " + string.Join(", ", arr) + " ]";
        }

        /// <summary>
        /// 字典转string
        /// </summary>
        public static string ToStringMap<K, V>(IDictionary<K, V> dic, Func<K, string> keyFunc = null, Func<V, string> valueFunc = null)
        {
            if (dic == null)
            {
                return "null";
            }
            
            var sb = new StringBuilder();
            sb.Append('{');
            foreach (var (key, value) in dic)
            {
                var keyStr = keyFunc != null ? keyFunc(key) : key.ToString();
                var valueStr = valueFunc != null ? valueFunc(value) : value.ToString();
                sb.Append(keyStr).Append(": ").Append(valueStr).Append(", ");
            }
            
            if (sb.Length > 1)
            {
                sb.Length--;
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}