// ******************************************************************
//@file         SorterBase.cs
//@brief        排序器基类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.13 16:49:07
// ******************************************************************

using System;
using System.Collections.Generic;

namespace Yu
{
    public class SorterBase<TSorterId, TItem>
    {
        public delegate Comparison<TItem> ComparisonGenerator(bool isAsc, params object[] customParams); //排序器创建的Func定义

        private readonly Dictionary<TSorterId, ComparisonGenerator> _comparisonFuncDict = new(); //存储 创建Func


        /// <summary>
        /// 注册排序条件
        /// </summary>
        /// <param name="sorterId">排序id</param>
        /// <param name="comparisonGenerator">Comparison创建的Func</param>
        public void RegisterSorter(TSorterId sorterId, ComparisonGenerator comparisonGenerator)
        {
            _comparisonFuncDict.TryAdd(sorterId, comparisonGenerator);
        }

        /// <summary>
        /// 单sortID排序
        /// </summary>
        public void Sort(List<TItem> source, TSorterId sorterId, bool isAsc, params object[] customParams)
        {
            Sort(source, new[] {(sorterId, isAsc, customParams)});
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="source">排序数据</param>
        /// <param name="sorters">排序id列表</param>
        public void Sort(List<TItem> source, IEnumerable<(TSorterId sorterId, bool isAsc, object[] customParams)> sorters)
        {
            var comparisonList = new List<Comparison<TItem>>();
            foreach (var (sorterId, isAsc, customParams) in sorters)
            {
                if (!_comparisonFuncDict.TryGetValue(sorterId, out var func))
                {
                    continue;
                }

                var comparison = func(isAsc, customParams);
                comparisonList.Add(comparison);
            }

            //使用组合排序器
            source.Sort(CombineComparisons(comparisonList));
        }

        /// <summary>
        /// 混合多个排序条件
        /// </summary>
        private static Comparison<T> CombineComparisons<T>(IReadOnlyCollection<Comparison<T>> comparisons)
        {
            return (x, y) =>
            {
                foreach (var comparison in comparisons)
                {
                    var result = comparison(x, y);
                    if (result != 0)
                    {
                        return result;
                    }
                }

                return 0;
            };
        }
    }
}