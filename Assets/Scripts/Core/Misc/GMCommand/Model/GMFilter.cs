using System;
using System.Collections.Generic;
using Yu;

public enum GMFilterID
{
    ByCatalog, //按gm指令的分类
    ByIsShow,//按是否UI显示
}

/// <summary>
/// GM指令过滤器
/// </summary>
public class GMFilter : FilterBase<GMFilterID, GMMethodData>
{
    public GMFilter()
    {
        RegisterFilter(GMFilterID.ByCatalog, ByGmCatalog);
        RegisterFilter(GMFilterID.ByIsShow, ByIsShow);
    }
    
    /// <summary>
    /// 按gm指令分类
    /// </summary>
    public void FilterByCatalog(IEnumerable<GMMethodData> source, ref List<GMMethodData> resultRef, string catalog)
    {
        Filter(source, ref resultRef, GMFilterID.ByCatalog, catalog);
    }

    /// <summary>
    /// 按是否UI显示
    /// </summary>
    public void FilterByIsShow(IEnumerable<GMMethodData> source, ref List<GMMethodData> resultRef, bool isShow)
    {
        Filter(source, ref resultRef, GMFilterID.ByIsShow, isShow);
    }
    
    /// <summary>
    /// 按gm指令分类过滤func
    /// </summary>
    private static Func<GMMethodData, bool> ByGmCatalog(object[] customParams)
    {
        if (customParams == null || customParams.Length <= 0)
        {
            return _ => false;
        }

        if (customParams[0] is not string orderCatalog)
        {
            return _ => false;
        }
        
        return x => x.Catalog.Equals(orderCatalog);
    }    
    
    /// <summary>
    /// 按是否UI显示func
    /// </summary>
    private static Func<GMMethodData, bool> ByIsShow(object[] customParams)
    {
        if (customParams == null || customParams.Length <= 0)
        {
            return _ => false;
        }

        if (customParams[0] is not bool isShow)
        {
            return _ => false;
        }
        
        return x => x.UIShow == isShow;
    }
}