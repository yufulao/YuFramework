// ******************************************************************
// @file         FilterBase.cs
// @brief        过滤器基类
// @author       yufulao, yufulao@qq.com
// @createTime   2024.11.13 21:35:54
// ******************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Yu
{
    public class FilterBase<TFilterId, TItem>
    {
        public delegate Func<TItem, bool> FilterFunc(params object[] customParams); //定义过滤条件委托

        private readonly Dictionary<TFilterId, FilterFunc> _filterFuncDict = new(); //所有过滤条件


        /// <summary>
        /// 注册过滤条件
        /// </summary>
        public void RegisterFilter(TFilterId filterId, FilterFunc filterFunc)
        {
            _filterFuncDict.TryAdd(filterId, filterFunc);
        }

        /// <summary>
        /// 过滤，单个条件
        /// </summary>
        /// <param name="sourceList">原始数据</param>
        /// <param name="resultList">过滤结果</param>
        /// <param name="filterId">过滤条件id</param>
        /// <param name="customParam">过滤条件参数</param>
        public void Filter(IEnumerable<TItem> sourceList, ref List<TItem> resultList, TFilterId filterId, params object[] customParam)
        {
            resultList.Clear();
            if (!_filterFuncDict.TryGetValue(filterId, out var filterFunc))
            {
                return;
            }

            //filterFunc创建匿名函数时，解析了customParam并捕获变量值，在该闭包的生命周期内（即整个Where()内部的流程），不会重复解析customParam（即传入值类型时不会重复拆箱）
            var filter = filterFunc(customParam);
            var tempResult = sourceList.Where(filter);
            resultList.AddRange(tempResult);
        }

        /// <summary>
        /// 过滤，多个条件
        /// </summary>
        /// <param name="sourceList"></param>
        /// <param name="resultList"></param>
        /// <param name="isAll">true：全部条件都满足，false：任一条件满足</param>//更复杂的All和Any混合过滤应该归类为查找
        /// <param name="filters"></param>
        public void Filter(IEnumerable<TItem> sourceList, ref List<TItem> resultList, bool isAll, IEnumerable<(TFilterId filterId, object[] customParams)> filters)
        {
            resultList.Clear();
            var filterList = new List<Func<TItem, bool>>();
            foreach (var (filterId, customParams) in filters)
            {
                if (!_filterFuncDict.TryGetValue(filterId, out var filterFunc))
                {
                    continue;
                }

                var filter = filterFunc(customParams);
                filterList.Add(filter);
            }

            var combinedFilter = CombineFilters(isAll, filterList);
            var tempResult = sourceList.Where(combinedFilter);
            resultList.AddRange(tempResult);
        }


        /// <summary>
        /// 组合多个过滤器为一个
        /// </summary>
        private static Func<T, bool> CombineFilters<T>(bool isAll, IReadOnlyCollection<Func<T, bool>> filters)
        {
            if (isAll)
            {
                return item => filters.All(filter => filter(item));
            }

            return item => filters.Any(filter => filter(item));
        }
    }
}