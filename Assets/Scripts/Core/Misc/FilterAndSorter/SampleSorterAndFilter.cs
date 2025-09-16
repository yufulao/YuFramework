// ******************************************************************
//@file         SampleSorterAndFilter.cs
//@brief        过滤器排序器使用示例
//@author       yufulao, yufulao@qq.com
//@createTime   2024.11.14 01:31:45
// ******************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;
using Yu;

//============================================================测试代码Start============================================================

// var sample = new SampleSorterAndFilter();
// sample.SorterByTypeThenType2();
// sample.SorterByName();
// sample.FilterByOneName();
// sample.FilterByTwoName();
// sample.FilterByType();
// sample.FilterByTypeAndType2();

//============================================================测试代码End============================================================

public class SampleSorterAndFilter
{
    private readonly ItemTestSorter _sorter = new();
    private readonly ItemTestFilter _filter = new();

    private readonly List<ItemTest> _data = new()
    {
        new ItemTest() {Name = "3", ItemType = ItemTestType.D, ItemType2 = ItemTestType2.Cc},
        new ItemTest() {Name = "3", ItemType = ItemTestType.C, ItemType2 = ItemTestType2.Aa},
        new ItemTest() {Name = "7", ItemType = ItemTestType.D, ItemType2 = ItemTestType2.Cc},
        new ItemTest() {Name = "4", ItemType = ItemTestType.C, ItemType2 = ItemTestType2.Bb},
        new ItemTest() {Name = "5", ItemType = ItemTestType.C, ItemType2 = ItemTestType2.Cc},
        new ItemTest() {Name = "3", ItemType = ItemTestType.B, ItemType2 = ItemTestType2.Dd},
        new ItemTest() {Name = "6", ItemType = ItemTestType.D, ItemType2 = ItemTestType2.Dd},
        new ItemTest() {Name = "1", ItemType = ItemTestType.A, ItemType2 = ItemTestType2.Aa},
        new ItemTest() {Name = "2", ItemType = ItemTestType.B, ItemType2 = ItemTestType2.Aa},
        new ItemTest() {Name = "2", ItemType = ItemTestType.B, ItemType2 = ItemTestType2.Bb},
        new ItemTest() {Name = "3", ItemType = ItemTestType.A, ItemType2 = ItemTestType2.Aa},
        new ItemTest() {Name = "8", ItemType = ItemTestType.D, ItemType2 = ItemTestType2.Aa},
    };


    /// <summary>
    /// 按name排序
    /// </summary>
    public void SorterByName()
    {
        _sorter.Sort(_data, "SorterByName", true, null);
        GameLog.Info("SorterByName: true");
        LogResult(_data);
    }

    /// <summary>
    /// 按Type再Type2排序
    /// </summary>
    public void SorterByTypeThenType2()
    {
        _sorter.Sort(_data, new List<(string sorterId, bool isAsc, object[] customParams)>()
        {
            ("SorterByType", true, null),
            ("SorterByType2", true, null)
        });

        GameLog.Info("SorterByTypeThenType2: true true");
        LogResult(_data);
    }


    private List<ItemTest> _cacheFilterResult = new List<ItemTest>();

    /// <summary>
    /// 按name过滤 "3"
    /// </summary>
    public void FilterByOneName()
    {
        _filter.Filter(_data, ref _cacheFilterResult, "FilterByOneName", "3");
        GameLog.Info("FilterByOneName: 3");
        LogResult(_cacheFilterResult);
    }
    
    /// <summary>
    /// 按name过滤 "2" 或 "3"
    /// </summary>
    public void FilterByTwoName()
    {
        _filter.Filter(_data, ref _cacheFilterResult,false, new List<(string, object[])>()
        {
            ("FilterByOneName",new object[]{"2"}),
            ("FilterByOneName",new object[]{"3"})
        });
        GameLog.Info("FilterByTwoName: 2 or 3");
        LogResult(_cacheFilterResult);
    }

    /// <summary>
    /// 按Type过滤 D
    /// </summary>
    public void FilterByType()
    {
        _filter.Filter(_data, ref _cacheFilterResult, "FilterByType", ItemTestType.D);
        GameLog.Info("FilterByType: D");
        LogResult(_cacheFilterResult);
    }

    /// <summary>
    /// 按Type和Type2过滤 D和Cc
    /// </summary>
    public void FilterByTypeAndType2()
    {
        //也可以使用多条件过滤，比如上面 FilterByTwoName() ，isAll设为true 即多条件同时满足
        //Filter(IEnumerable<ItemTest> sourceList, ref List<ItemTest> resultList, true, IEnumerable<(string filterId, object[] customParams)> filters) 
        _filter.Filter(_data, ref _cacheFilterResult, "FilterByTypeAndType2", ItemTestType.D,ItemTestType2.Cc);
        GameLog.Info("FilterByTypeAndType2: D  Cc");
        LogResult(_cacheFilterResult);
    }

    /// <summary>
    /// 打印结果
    /// </summary>
    /// <param name="data"></param>
    private static void LogResult(IEnumerable<ItemTest> data)
    {
        var result = "";
        foreach (var item in data)
        {
            result += item.Name + "  " + item.ItemType + "  " + item.ItemType2 + "\n";
        }

        GameLog.Info(result);
    }
}

//============================================================定义============================================================

public enum ItemTestType
{
    A,
    B,
    C,
    D,
}

public enum ItemTestType2
{
    Aa,
    Bb,
    Cc,
    Dd,
}

public class ItemTest
{
    public string Name;
    public ItemTestType ItemType;
    public ItemTestType2 ItemType2;
}


//============================================================排序器============================================================

public class ItemTestSorter : SorterBase<string, ItemTest>
{
    public ItemTestSorter()
    {
        RegisterSorter("SorterByType", SorterByType);
        RegisterSorter("SorterByType2", SorterByType2);
        RegisterSorter("SorterByName", SorterByName);
    }

    private static Comparison<ItemTest> SorterByType(bool isAsc, params object[] customParams)
    {
        return (x, y) =>
        {
            if (x.ItemType == y.ItemType)
            {
                return 0;
            }

            return isAsc
                ? string.CompareOrdinal(x.ItemType.ToString(), y.ItemType.ToString())
                : string.CompareOrdinal(y.ItemType.ToString(), x.ItemType.ToString());
        };
    }

    private static Comparison<ItemTest> SorterByType2(bool isAsc, params object[] customParams)
    {
        return (x, y) =>
        {
            if (x.ItemType2 == y.ItemType2)
            {
                return 0;
            }

            return isAsc
                ? string.CompareOrdinal(x.ItemType2.ToString(), y.ItemType2.ToString())
                : string.CompareOrdinal(y.ItemType2.ToString(), x.ItemType2.ToString());
        };
    }

    private static Comparison<ItemTest> SorterByName(bool isAsc, params object[] customParams)
    {
        return (x, y) =>
        {
            if (x.Name.Equals(y.Name))
            {
                return 0;
            }

            return isAsc ? string.CompareOrdinal(x.Name, y.Name) : string.CompareOrdinal(y.Name, x.Name);
        };
    }
}

//============================================================过滤器============================================================

public class ItemTestFilter : FilterBase<string, ItemTest>
{
    public ItemTestFilter()
    {
        RegisterFilter("FilterByType", FilterByType);
        RegisterFilter("FilterByTypeAndType2", FilterByTypeAndType2);
        RegisterFilter("FilterByOneName", FilterByOneName);
    }

    private static Func<ItemTest, bool> FilterByType(object[] customParams)
    {
        if (customParams == null || customParams.Length <= 0)
        {
            return _ => false;
        }

        if (customParams[0] is not ItemTestType orderType)
        {
            return _ => false;
        }

        //闭包的函数体，全局仅一份，即下面这行
        return x => x.ItemType == orderType;
    }

    private static Func<ItemTest, bool> FilterByTypeAndType2(object[] customParams)
    {
        if (customParams == null || customParams.Length <= 1)
        {
            return _ => false;
        }

        if (customParams[0] is not ItemTestType orderType || customParams[1] is not ItemTestType2 orderType2)
        {
            return _ => false;
        }
        
        return x => x.ItemType == orderType && x.ItemType2 == orderType2;
    }

    private static Func<ItemTest, bool> FilterByOneName(object[] customParams)
    {
        if (customParams == null || customParams.Length <= 0)
        {
            return _ => false;
        }

        if (customParams[0] is not string orderName)
        {
            return _ => false;
        }

        return x => x.Name.Equals(orderName);
    }
}