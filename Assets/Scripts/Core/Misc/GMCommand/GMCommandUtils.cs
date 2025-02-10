// ******************************************************************
//@file         GMCommandUtils.cs
//@brief        GMCommand工具类
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.26 13:28:41
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Yu
{
    public static class GMCommandUtils
    {
        public static readonly CompareInfo StringComparer = new CultureInfo("en-US").CompareInfo; //字符串比较器


        /// <summary>
        /// 查找string，正值即查找位置，负值取反即后一个元素，即插入位置
        /// </summary>
        public static int FindIndex<T>(List<T> list, string str, Func<T, string> keySelector)
        {
            //二分
            //[10, 20, 30, 40, 50]
            var min = 0; //0，0，1，2
            var max = list.Count - 1; //4，1，1，1

            while (min <= max)
            {
                var mid = (min + max) / 2; //2，0，1
                var result = IsSame(str, keySelector(list[mid])); //相同为0，a<b时负数，a>b时正数
                switch (result)
                {
                    case 0:
                        return mid;
                    case < 0:
                        max = mid - 1;
                        break;
                    default:
                        min = mid + 1;
                        break;
                }
            }

            return ~min; //~2 = -3，然后~-3 = 2
            //woc巨nb，直接将查找算法和插入算法合二为一
            //返回一个int负数，根据原理([-a]_补 = [a]_补 按位取反 + 1)，就能反推出查找结束的index，还能知道是负数所以是查找失败，高级玩意!
        }

        /// <summary>
        /// 找部分str在完整str的起始下标，无则为-1
        /// </summary>
        public static int FindSubstringIndex(string strFull, string strSub)
        {
            return StringComparer.IndexOf(strFull, strSub, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }

        /// <summary>
        /// 查找字符在字符串中的下标，无则为-1
        /// </summary>
        public static int IndexOfChar(string str, char c, int startIndex)
        {
            var result = str.IndexOf(c, startIndex);
            if (result < 0)
            {
                result = str.Length;
            }

            return result;
        }

        /// <summary>
        /// 字符串是否相同
        /// </summary>
        /// <returns>相同为0，不相同按字母大小输出负数和正数</returns>
        public static int IsSame(string str0, string str1)
        {
            return StringComparer.Compare(str0, str1, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }

        /// <summary>
        /// 是否是str的前缀
        /// </summary>
        public static bool IsPrefix(string str, string prefix)
        {
            return StringComparer.IsPrefix(str, prefix, CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace);
        }
    }
}