using System;
using System.Collections.Generic;

namespace Yu
{
    public static partial class Utils
    {
        /// <summary>
        /// 阿拉伯数字转文字数字工具类
        /// 最大支持亿级数字转化，可通过对_groupUnitDict新增数量级扩充（如兆、京等）
        /// </summary>
        public static class UtilsForNumberToString
        {
            // 数字字典
            private static readonly Dictionary<char, string> Digit2StrDict = new()
            {
                { '0', "零" },
                { '1', "一" },
                { '2', "二" },
                { '3', "三" },
                { '4', "四" },
                { '5', "五" },
                { '6', "六" },
                { '7', "七" },
                { '8', "八" },
                { '9', "九" },
            };

            // 基本单位字典（每组数字的前三位）
            private static readonly Dictionary<int, string> BasicUnit2StrDict = new()
            {
                { 2, "十" },
                { 3, "百" },
                { 4, "千" },
            };

            // 数组单位字典
            private static readonly Dictionary<int, string> GroupUnitDict = new()
            {
                { 1, "个" },
                { 2, "万" },
                { 3, "亿" }
            };

            // 数组字典，存储分组的数串（每四位一组，分组从右到左）
            private static SortedDictionary<int, string> _groupsDict = new();

            public static string Convert(int number)
            {
                _groupsDict.Clear();

                var result = "";
                var numberStr = number.ToString();

                // 将数串按4个数字一组分组
                _groupsDict = SplitNumberStr2Groups(numberStr, _groupsDict);

                // 对每组数串进行转化
                for (int i = _groupsDict.Count; i >= 1; i--)
                {
                    var subNumberStr = _groupsDict[i];
                    var subResult = ConverNumberStrGroup(subNumberStr, GroupUnitDict[i], i == _groupsDict.Count);

                    result += subResult;
                }

                return result;
            }

            /// <summary>
            /// 分组数串
            /// </summary>
            /// <param name="numberStr">要分组的数串</param>
            /// <param name="groupsDict">存储分组结果的字典</param>
            /// <returns></returns>
            private static SortedDictionary<int, string> SplitNumberStr2Groups(string numberStr,
                SortedDictionary<int, string> groupsDict)
            {
                int length = GetLengthOfNumber(numberStr);
                int groupNum = GetGroupCountOfNumber(length);
                int groupStartIndex;
                int groupLength;

                // 当只能分为一组时，直接返回
                if (groupNum == 1)
                {
                    groupsDict.Add(1, numberStr);
                    return groupsDict;
                }

                // 当可以分为多组时，按从右到左的顺序分组
                for (int i = 1; i <= groupNum; i++)
                {
                    // 完全分组情况
                    if (length % 4 != 0)
                    {
                        groupStartIndex = i == groupNum ? 0 : 4 * groupNum - 3 - 4 * (i - 1) - (4 - length % 4) - 1;
                    }
                    // 不完全分组情况（最左边第一组有剩余）
                    else
                    {
                        groupStartIndex = 4 * (groupNum - i);
                    }

                    groupLength = i == groupNum ? length - 4 * (groupNum - 1) : 4;

                    string subNumberStr = numberStr.Substring(groupStartIndex, groupLength);
                    groupsDict.Add(i, subNumberStr);
                }

                return groupsDict;
            }

            /// <summary>
            /// 转化每个字符串组
            /// </summary>
            /// <param name="subNumberArabicStr">子阿拉伯数串</param>
            /// <param name="groupUnit">字符串组单位（个、万、亿等）</param>
            /// <param name="isLeftMostSubNumberStr">是否是左边第一个字符串组（含有多个字符串组时）</param>
            /// <returns></returns>
            private static string ConverNumberStrGroup(string subNumberArabicStr, string groupUnit,
                bool isLeftMostSubNumberStr)
            {
                var result = "";

                for (int i = 0; i < subNumberArabicStr.Length; i++)
                {
                    var arabicDigit = subNumberArabicStr[i];
                    var arabicUnit = subNumberArabicStr.Length - i;

                    // 阿拉伯数字字符转化为文字字符
                    var literalDigit = Digit2StrDict[arabicDigit];
                    // 忽略多个数串的最左边两位数串的十万位字符1（不转化为“一”）。如112345转化为“十一万二千三百四十五个”。而不是“一十一万二千三百四十五个”
                    literalDigit =
                        isLeftMostSubNumberStr && arabicUnit == 2 && literalDigit == "一" &&
                        subNumberArabicStr.Length == 2
                            ? ""
                            : literalDigit;
                    // 处理末尾为0的情况
                    // 合并中间的多个零
                    literalDigit =
                        i + 1 < subNumberArabicStr.Length && subNumberArabicStr[i] == '0' &&
                        subNumberArabicStr[i + 1] == '0'
                            ? ""
                            : literalDigit;
                    // 去掉末尾的0
                    literalDigit =
                        i == subNumberArabicStr.Length - 1 && subNumberArabicStr[i] == '0' &&
                        subNumberArabicStr.Length > 1
                            ? ""
                            : literalDigit;


                    // 阿拉伯单位转化为文字单位
                    var literalUnit =
                        arabicUnit == 1 || arabicDigit == '0'
                            ? ""
                            : BasicUnit2StrDict[arabicUnit]; // 最后一位不增加个单位，否则会出现“一个个”结果

                    result += literalDigit + literalUnit;
                }

                // 增加数组对应的单位（个、万、亿）
                result += groupUnit;

                return result;
            }

            /// <summary>
            /// 获取分组的个数
            /// </summary>
            /// <param name="numberLength"></param>
            /// <returns></returns>
            private static int GetGroupCountOfNumber(int numberLength)
            {
                return (int)Math.Ceiling((float)numberLength / 4);
            }

            /// <summary>
            /// 获取数字的长度
            /// </summary>
            /// <param name="numberStr"></param>
            /// <returns></returns>
            private static int GetLengthOfNumber(string numberStr)
            {
                var digitCount = numberStr.Length;
                return digitCount;
            }
        }
    }
}