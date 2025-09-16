using System;

namespace Yu
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GMCatalogAttribute : Attribute
    {
        public string Catalog { get; } // 指令分类
        public const string NoCatalog = "未分类"; //无分类的默认类别

        public GMCatalogAttribute(string catalog)
        {
            Catalog = catalog;
        }
    }
}