// ******************************************************************
//@file         GMParameterOptionsBase.cs
//@brief        代理参数基类，自定义参数可选数据源
//@author       yufulao, yufulao@qq.com
//@createTime   2025.01.25 14:53:42
// ******************************************************************

using System;
using System.Collections.Generic;

namespace Yu
{
    /// <typeparam name="T">实际参数类型</typeparam>
    public abstract class GmParameterOptionsBase<T> : IGmParameterOptions
    {
        public T Value; //实参值
        
        
        /// <summary>
        /// 继承类提供数据源
        /// </summary>
        protected abstract object[] GetOptionsSource();
        
        /// <summary>
        /// 获取自定义参数的可选项
        /// </summary>
        public virtual object[] GetParameterOptionArray()
        {
            //泛型父类通过GetType()来区分子类类型
            return GmParameterOptionCache.GetOptionArray(GetType(), GetOptions);
        }

        /// <summary>
        /// 根据数据源装载参数可选项
        /// </summary>
        protected virtual object[] GetOptions()
        {
            var optionsData = GetOptionsSource();
            var options = new object[optionsData.Length];
            for (var i = 0; i < optionsData.Length; i++)
            {
                //因为要通过gm指令的参数类型是否继承了IGmParameterOptions接口，所以method.Invoke时也要传一致类型的参数
                //就只能一个数据项创建一个GmParameterOptionsBase对象
                var parameter = (GmParameterOptionsBase<T>)Activator.CreateInstance(GetType());
                parameter.Value = (T)optionsData[i];
                options[i] = parameter;
            }

            return options;
        }
        
        /// <summary>
        /// 参数可选项显示内容
        /// </summary>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    
    
    /// <summary>
    /// 代理参数类接口（反射中检查单继承接口，比检查继承链，更高效）
    /// </summary>
    public interface IGmParameterOptions
    {
        /// <summary>
        /// 获取自定义参数的可选项数组
        /// </summary>
        object[] GetParameterOptionArray();
    }
    
    
    /// <summary>
    /// 代理参数类的 可选项缓存 公共静态区
    /// </summary>
    public static class GmParameterOptionCache
    {
        private static readonly Dictionary<Type, object[]> Cache = new Dictionary<Type, object[]>();
        
        
        /// <summary>
        /// 获取type指定的参数可选项
        /// </summary>
        public static object[] GetOptionArray(Type type, Func<object[]> loadOptionsFunc)
        {
            //优先检查缓存
            if (Cache.TryGetValue(type, out var options))
            {
                return options;
            }
            
            options = loadOptionsFunc();
            Cache[type] = options;
            return options;
        }
    }
}