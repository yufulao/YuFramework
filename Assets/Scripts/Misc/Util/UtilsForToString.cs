// ******************************************************************
//@file         UtilsForToString.cs
//@brief        通用类型转string
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 01:34:18
// ******************************************************************

using System.Collections.Generic;
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
        public static string ToStringArray<T>(T[] arr)
        {
            return arr == null ? "null" : "[" + string.Join(",", arr) + "]";
        }

        /// <summary>
        /// 列表转string
        /// </summary>
        public static string ToStringCollection<T>(IEnumerable<T> arr)
        {
            return arr == null ? "null" : "[" + string.Join(",", arr) + "]";
        }

        /// <summary>
        /// 字典转string
        /// </summary>
        public static string ToStringMap<K, V>(IDictionary<K, V> dic)
        {
            if (dic == null)
            {
                return "null";
            }

            var sb = new StringBuilder();
            sb.Append('{');
            foreach (var kvp in dic)
            {
                sb.Append(kvp.Key).Append(':');
                sb.Append(kvp.Value).Append(',');
            }

            sb.Append('}');
            return sb.ToString();
        }
    }
}